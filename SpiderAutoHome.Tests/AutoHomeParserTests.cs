using System.Text;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using Xunit;

namespace SpiderAutoHome.Tests;

public class AutoHomeParserTests
{
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
}
