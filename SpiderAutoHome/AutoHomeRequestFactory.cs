using System.Globalization;
using System.Text;
using DotnetSpider.Http;

namespace SpiderAutoHome;

public static class AutoHomeRequestFactory
{
    public const string DefaultUrl =
        "https://store.mall.autohome.com.cn/shop/ajaxsitemodlecontext.jtml";

    public const int DefaultPageCount = 33;
    public const int MaxPageCount = 100;

    private const string ModuleConfiguration =
        "{\"createMan\":\"18273159100\",\"createTime\":1518433690000," +
        "\"row\":5,\"siteUserActivityListId\":8553," +
        "\"siteUserPageRowModuleId\":84959,\"topids\":\"\"," +
        "\"wherePhase\":\"1\",\"wherePreferential\":\"0\"," +
        "\"whereUsertype\":\"0\"}";

    public static IReadOnlyList<Request> Create(
        string? configuredUrl = null,
        string? configuredPageCount = null)
    {
        var uri = ResolveEndpoint(configuredUrl);
        var pageCount = ResolvePageCount(configuredPageCount);
        var requests = new List<Request>(pageCount);

        for (var page = 1; page <= pageCount; page++)
        {
            requests.Add(new Request(uri.AbsoluteUri)
            {
                Method = "POST",
                Content = new DotnetSpider.Http.StringContent(
                    CreateFormData(page),
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
            });
        }

        return requests;
    }

    public static string CreateFormData(int page)
    {
        if (page is < 1 or > MaxPageCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(page),
                $"Page must be between 1 and {MaxPageCount}.");
        }

        return string.Join(
            '&',
            "id=7",
            $"j={Uri.EscapeDataString(ModuleConfiguration)}",
            $"page={page.ToString(CultureInfo.InvariantCulture)}",
            "shopid=83106681");
    }

    public static int ResolvePageCount(string? configuredPageCount)
    {
        if (string.IsNullOrWhiteSpace(configuredPageCount))
        {
            return DefaultPageCount;
        }

        if (!int.TryParse(
                configuredPageCount,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var pageCount) ||
            pageCount is < 1 or > MaxPageCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(configuredPageCount),
                $"Page count must be an integer between 1 and {MaxPageCount}.");
        }

        return pageCount;
    }

    private static Uri ResolveEndpoint(string? configuredUrl)
    {
        var url = string.IsNullOrWhiteSpace(configuredUrl) ? DefaultUrl : configuredUrl;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                "The list endpoint must be an absolute HTTP or HTTPS URL.",
                nameof(configuredUrl));
        }

        return uri;
    }
}
