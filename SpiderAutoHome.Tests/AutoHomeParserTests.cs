using System.Text;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using Xunit;

namespace SpiderAutoHome.Tests;

public class AutoHomeParserTests
{
    [Fact]
    public void RequestFactoryCreatesBoundedPostRequests()
    {
        var requests = AutoHomeRequestFactory.Create(
            "https://example.com/list",
            "3");

        Assert.Collection(
            requests,
            request => AssertRequest(request, 1),
            request => AssertRequest(request, 2),
            request => AssertRequest(request, 3));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("101")]
    [InlineData("1.5")]
    [InlineData("many")]
    public void RequestFactoryRejectsInvalidPageCounts(string pageCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AutoHomeRequestFactory.Create(null, pageCount));
    }

    [Fact]
    public void RequestFactoryRejectsNonHttpEndpoints()
    {
        Assert.Throws<ArgumentException>(
            () => AutoHomeRequestFactory.Create("file:///tmp/list.html", "1"));
    }

    [Fact]
    public void FormDataIsDeterministicAndUrlEncoded()
    {
        var formData = AutoHomeRequestFactory.CreateFormData(7);

        Assert.StartsWith("id=7&j=%7B%22createMan%22", formData);
        Assert.Contains("&page=7&shopid=83106681", formData);
        Assert.DoesNotContain('{', formData);
        Assert.DoesNotContain('"', formData);
    }

    [Fact]
    public async Task ParseAsyncExtractsCarInformation()
    {
        const string html = """
            <div class="list">
                <ul class="fn-clear">
                    <li class="carbox">
                        <a href="https://example.com/car/1">
                            <div class="carbox-carimg"><img src="https://example.com/car.jpg" /></div>
                            <div class="carbox-title">Test Car</div>
                            <div class="carbox-tip">Test Tip</div>
                            <div class="carbox-number"><span>10</span></div>
                            <div class="carbox-info">¥100 ¥120</div>
                        </a>
                    </li>
                </ul>
            </div>
            """;

        var cars = await ParseAsync(html);

        var car = Assert.Single(cars);
        Assert.Equal("https://example.com/car/1", car.DetailUrl);
        Assert.Equal("https://example.com/car.jpg", car.CarImg);
        Assert.Equal("Test Car", car.Title?.Trim());
        Assert.Equal("Test Tip", car.Tip?.Trim());
        Assert.Equal("10", car.BuyNum?.Trim());
        Assert.Equal("100", car.Price);
        Assert.Equal("120", car.DelPrice);
    }

    [Fact]
    public async Task ParseAsyncUsesCurrentPriceWhenListPriceIsMissing()
    {
        const string html = """
            <div class="list">
                <ul class="fn-clear">
                    <li class="carbox">
                        <a href="https://example.com/car/2">
                            <div class="carbox-title">One Price Car</div>
                            <div class="carbox-info"> ¥88 </div>
                        </a>
                    </li>
                </ul>
            </div>
            """;

        var car = Assert.Single(await ParseAsync(html));

        Assert.Equal("88", car.Price);
        Assert.Equal("88", car.DelPrice);
    }

    [Fact]
    public async Task ParseAsyncReturnsEmptyCollectionForUnrelatedMarkup()
    {
        var cars = await ParseAsync("<html><body><p>No car list</p></body></html>");

        Assert.Empty(cars);
    }

    private static async Task<List<AutoHomeShopListEntity>> ParseAsync(string html)
    {
        var request = new Request("https://example.com/fixture");
        var response = new Response
        {
            Content = new DotnetSpider.Http.ByteArrayContent(Encoding.UTF8.GetBytes(html))
        };

        using var context = new DataFlowContext(
            null,
            new SpiderOptions(),
            request,
            response);

        var parser = new AutoHomeParser();
        await parser.HandleAsync(context, _ => Task.CompletedTask);

        var cars = context.GetData("CarList") as List<AutoHomeShopListEntity>;
        return Assert.IsType<List<AutoHomeShopListEntity>>(cars);
    }

    private static void AssertRequest(Request request, int expectedPage)
    {
        Assert.Equal("https://example.com/list", request.RequestUri.AbsoluteUri);
        Assert.Equal("POST", request.Method);
        var content = Assert.IsType<DotnetSpider.Http.StringContent>(request.Content);
        Assert.Equal(
            AutoHomeRequestFactory.CreateFormData(expectedPage),
            content.Content);
        Assert.Equal("application/x-www-form-urlencoded", content.MediaType);
    }
}
