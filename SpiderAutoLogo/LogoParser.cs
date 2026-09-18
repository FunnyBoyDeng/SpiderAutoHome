using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;

namespace SpiderAutoLogo;

public sealed class LogoParser : DataParser
{
    public override Task InitializeAsync() => Task.CompletedTask;

    protected override Task ParseAsync(DataFlowContext context)
    {
        var logos = new List<LogoInfo>();
        var nodes = context.Selectable.SelectList(
            DotnetSpider.Selector.Selectors.XPath(
                ".//div[@id='div_ListBrand']//div[@class='item']"));

        if (nodes is not null)
        {
            foreach (var node in nodes)
            {
                var brandName = node.XPath(".//strong")?.Value?.Trim();
                var rawImageUrl = node.XPath(".//img/@src")?.Value;

                if (string.IsNullOrWhiteSpace(rawImageUrl))
                {
                    rawImageUrl = node.XPath(".//img/@data-src")?.Value;
                }

                var imageUrl = NormalizeImageUrl(rawImageUrl, context.Request.RequestUri);

                if (!string.IsNullOrWhiteSpace(brandName) && imageUrl is not null)
                {
                    logos.Add(new LogoInfo(brandName, imageUrl));
                }
            }
        }

        context.AddData("LogoInfoList", logos);
        return Task.CompletedTask;
    }

    public static string? NormalizeImageUrl(string? rawUrl, Uri pageUri)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
        {
            return null;
        }

        var value = rawUrl.Trim();
        if (value.StartsWith("//", StringComparison.Ordinal))
        {
            value = $"https:{value}";
        }

        Uri? absoluteUri;
        var isRootRelative = value[0] == '/' &&
                             !value.StartsWith("//", StringComparison.Ordinal);

        if (isRootRelative)
        {
            if (!Uri.TryCreate(pageUri, value, out absoluteUri))
            {
                return null;
            }
        }
        else if (!Uri.TryCreate(value, UriKind.Absolute, out absoluteUri) &&
                 !Uri.TryCreate(pageUri, value, out absoluteUri))
        {
            return null;
        }

        return absoluteUri is not null && absoluteUri.Scheme is "http" or "https"
            ? absoluteUri.AbsoluteUri
            : null;
    }
}

public sealed record LogoInfo(string BrandName, string ImageUrl);
