using DotnetSpider.Http;

namespace SpiderAutoLogo;

public static class LogoRequestFactory
{
    public const string DefaultUrl = "https://car.m.autohome.com.cn/";

    public static Request Create(string? configuredUrl = null)
    {
        var url = string.IsNullOrWhiteSpace(configuredUrl) ? DefaultUrl : configuredUrl;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                "The logo page URL must be an absolute HTTP or HTTPS URL.",
                nameof(configuredUrl));
        }

        return new Request(uri.AbsoluteUri) { Method = "GET" };
    }
}
