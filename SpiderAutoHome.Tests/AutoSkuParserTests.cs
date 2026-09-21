using System.Text;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using SpiderAutoSkuData;
using Xunit;

namespace SpiderAutoHome.Tests;

public class AutoSkuParserTests
{
    [Fact]
    public async Task DetailPageCreatesTwoApiRequests()
    {
        const string html = """
            <html><body>
              <a class="carbox-compare_detail" link="12345">Compare</a>
            </body></html>
            """;

        using var context = CreateContext(SkuRequestFactory.DefaultDetailUrl, html);

        await ParseAsync(context);

        Assert.Equal("12345", context.GetData("SkuId"));
        var followRequests = SkuRequestFactory.CreateApiRequests("12345");
        Assert.Equal(2, followRequests.Count);
        Assert.All(
            followRequests,
            request => Assert.Contains("12345", request.RequestUri.AbsoluteUri));
        Assert.Contains(
            followRequests,
            request => request.RequestUri.AbsoluteUri.Contains(
                "spec_paramsinglebyspecid.ashx",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            followRequests,
            request => request.RequestUri.AbsoluteUri.Contains(
                "Config_GetListBySpecId.ashx",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DetailPageWithoutSkuDoesNotCreateFollowRequests()
    {
        using var context = CreateContext(
            SkuRequestFactory.DefaultDetailUrl,
            "<html><body><p>Missing link</p></body></html>");

        await ParseAsync(context);

        Assert.Null(context.GetData("SkuId"));
        Assert.Empty(SkuRequestFactory.CreateApiRequests(string.Empty));
    }

    [Fact]
    public async Task BasicInfoResponseDeserializesToTypedModel()
    {
        const string json = """
            {
              "message": "ok",
              "returncode": "0",
              "result": {
                "specid": "12345",
                "paramtypeitems": [
                  {
                    "name": "Body",
                    "paramitems": [{ "name": "Doors", "value": "4" }]
                  }
                ]
              }
            }
            """;
        var request = SkuRequestFactory.CreateApiRequests("12345")[0];
        using var context = CreateContext(request.RequestUri.AbsoluteUri, json);

        await ParseAsync(context);

        var model = Assert.IsType<AutoCarParam>((object?)context.GetData("BaseInfo"));
        Assert.NotNull(model.Result);
        Assert.Equal("12345", model.Result.SpecId);
        var group = Assert.Single(model.Result.ParamTypeItems);
        Assert.Equal("Body", group.Name);
        Assert.Equal("4", Assert.Single(group.ParamItems).Value);
    }

    [Fact]
    public async Task ConfigurationResponseDeserializesToTypedModel()
    {
        const string json = """
            {
              "message": "ok",
              "returncode": "0",
              "result": {
                "specid": "12345",
                "configtypeitems": [
                  {
                    "name": "Safety",
                    "configitems": [{ "name": "ABS", "value": "standard" }]
                  }
                ]
              }
            }
            """;
        var request = SkuRequestFactory.CreateApiRequests("12345")[1];
        using var context = CreateContext(request.RequestUri.AbsoluteUri, json);

        await ParseAsync(context);

        var model = Assert.IsType<AutoCarConfig>((object?)context.GetData("ExtInfo"));
        Assert.NotNull(model.Result);
        Assert.Equal("12345", model.Result.SpecId);
        var group = Assert.Single(model.Result.ConfigTypeItems);
        Assert.Equal("Safety", group.Name);
        Assert.Equal("ABS", Assert.Single(group.ConfigItems).Name);
    }

    [Fact]
    public async Task MalformedJsonProducesNoTypedResult()
    {
        var request = SkuRequestFactory.CreateApiRequests("12345")[0];
        using var context = CreateContext(request.RequestUri.AbsoluteUri, "{not-json");

        await ParseAsync(context);

        Assert.Null(context.GetData("BaseInfo"));
        Assert.NotNull(context.GetData("BaseInfoError"));
    }

    [Fact]
    public void RequestFactoryRejectsNonHttpDetailUrls()
    {
        Assert.Throws<ArgumentException>(
            () => SkuRequestFactory.CreateDetailRequest("file:///tmp/example.html"));
    }

    private static async Task ParseAsync(DataFlowContext context)
    {
        var parser = new AutoSkuParser();
        await parser.HandleAsync(context, _ => Task.CompletedTask);
    }

    private static DataFlowContext CreateContext(string url, string content)
    {
        var request = new Request(url);
        var response = new Response
        {
            Content = new DotnetSpider.Http.ByteArrayContent(
                Encoding.UTF8.GetBytes(content))
        };

        return new DataFlowContext(
            null,
            new SpiderOptions(),
            request,
            response);
    }
}
