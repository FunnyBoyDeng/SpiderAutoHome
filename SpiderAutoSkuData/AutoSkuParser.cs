using System.Text.Json;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;

namespace SpiderAutoSkuData;

public sealed class AutoSkuParser : DataParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public override Task InitializeAsync() => Task.CompletedTask;

    protected override Task ParseAsync(DataFlowContext context)
    {
        var url = context.Request.RequestUri.AbsoluteUri;

        if (IsBasicInfoRequest(url))
        {
            AddJsonResult<AutoCarParam>(context, "BaseInfo");
        }
        else if (IsConfigurationRequest(url))
        {
            AddJsonResult<AutoCarConfig>(context, "ExtInfo");
        }
        else
        {
            ParseDetailPage(context);
        }

        return Task.CompletedTask;
    }

    private static void ParseDetailPage(DataFlowContext context)
    {
        var skuId = context.Selectable
            .XPath(".//a[@class='carbox-compare_detail']/@link")
            ?.Value
            ?.Trim();

        if (string.IsNullOrWhiteSpace(skuId))
        {
            return;
        }

        context.AddData("SkuId", skuId);
        context.AddFollowRequests(SkuRequestFactory.CreateApiRequests(skuId));
    }

    private static void AddJsonResult<T>(DataFlowContext context, string key)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(
                context.Response.ReadAsString(),
                JsonOptions);

            if (result is not null)
            {
                context.AddData(key, result);
            }
        }
        catch (JsonException exception)
        {
            context.AddData($"{key}Error", exception.Message);
        }
    }

    private static bool IsBasicInfoRequest(string url) =>
        url.Contains("spec_paramsinglebyspecid.ashx", StringComparison.OrdinalIgnoreCase);

    private static bool IsConfigurationRequest(string url) =>
        url.Contains("Config_GetListBySpecId.ashx", StringComparison.OrdinalIgnoreCase);
}
