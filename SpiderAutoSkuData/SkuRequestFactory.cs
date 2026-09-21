using DotnetSpider.Http;

namespace SpiderAutoSkuData;

public static class SkuRequestFactory
{
    public const string DefaultDetailUrl =
        "https://mall.autohome.com.cn/detail/284641-0-0.html";

    private const string BasicInfoEndpoint =
        "//car.api.autohome.com.cn/v1/carprice/spec_paramsinglebyspecid.ashx";

    private const string ConfigurationEndpoint =
        "//car.api.autohome.com.cn/v2/carprice/Config_GetListBySpecId.ashx";

    public static Request CreateDetailRequest(string? configuredUrl = null)
    {
        var url = string.IsNullOrWhiteSpace(configuredUrl)
            ? DefaultDetailUrl
            : configuredUrl;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                "The detail URL must be an absolute HTTP or HTTPS URL.",
                nameof(configuredUrl));
        }

        return new Request(uri.AbsoluteUri) { Method = "GET", Depth = 1 };
    }

    public static IReadOnlyList<Request> CreateApiRequests(string skuId)
    {
        if (string.IsNullOrWhiteSpace(skuId))
        {
            return [];
        }

        return
        [
            CreateApiRequest(BasicInfoEndpoint, skuId),
            CreateApiRequest(ConfigurationEndpoint, skuId)
        ];
    }

    private static Request CreateApiRequest(string endpoint, string skuId)
    {
        var query =
            $"data%5B_host%5D={Uri.EscapeDataString(endpoint)}" +
            "&data%5B_appid%5D=mall" +
            $"&data%5Bspecid%5D={Uri.EscapeDataString(skuId.Trim())}";

        return new Request($"https://mall.autohome.com.cn/http/data.html?{query}")
        {
            Method = "GET",
            Depth = 2
        };
    }
}
