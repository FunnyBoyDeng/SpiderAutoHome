using System.Text;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using SpiderAutoLogo;
using Xunit;

namespace SpiderAutoHome.Tests;

public class LogoParserTests
{
    [Fact]
    public async Task ParserExtractsAndNormalizesLogoUrls()
    {
        const string html = """
            <div id="div_ListBrand">
              <div class="item">
                <strong>Alpha</strong>
                <img src="https://cdn.example.com/alpha.png" />
              </div>
              <div class="item">
                <strong>Beta</strong>
                <img data-src="//cdn.example.com/beta.jpg" />
              </div>
              <div class="item">
                <strong>Gamma</strong>
                <img src="/assets/gamma.svg" />
              </div>
              <div class="item">
                <strong>Ignored</strong>
                <img src="javascript:alert(1)" />
              </div>
            </div>
            """;
        using var context = CreateContext("https://example.com/brands/", html);

        var parser = new LogoParser();
        await parser.HandleAsync(context, _ => Task.CompletedTask);

        var logos = Assert.IsType<List<LogoInfo>>(
            (object?)context.GetData("LogoInfoList"));
        Assert.Collection(
            logos,
            logo => Assert.Equal(
                new LogoInfo("Alpha", "https://cdn.example.com/alpha.png"),
                logo),
            logo => Assert.Equal(
                new LogoInfo("Beta", "https://cdn.example.com/beta.jpg"),
                logo),
            logo => Assert.Equal(
                new LogoInfo("Gamma", "https://example.com/assets/gamma.svg"),
                logo));
    }

    [Fact]
    public async Task ParserReturnsEmptyCollectionForUnrelatedMarkup()
    {
        using var context = CreateContext(
            "https://example.com/brands/",
            "<html><body>No brands</body></html>");

        var parser = new LogoParser();
        await parser.HandleAsync(context, _ => Task.CompletedTask);

        var logos = Assert.IsType<List<LogoInfo>>(
            (object?)context.GetData("LogoInfoList"));
        Assert.Empty(logos);
    }

    [Theory]
    [InlineData("ACME/Auto", "ACME_Auto")]
    [InlineData("Brand:*?", "Brand___")]
    [InlineData("...", "unknown-brand")]
    public void DownloadFlowCreatesPortableFileNames(string input, string expected)
    {
        Assert.Equal(expected, LogoDownloadFlow.CreateSafeFileName(input));
    }

    [Fact]
    public void RequestFactoryRejectsNonHttpUrls()
    {
        Assert.Throws<ArgumentException>(
            () => LogoRequestFactory.Create("file:///tmp/brands.html"));
    }

    private static DataFlowContext CreateContext(string url, string html)
    {
        var response = new Response
        {
            Content = new DotnetSpider.Http.ByteArrayContent(
                Encoding.UTF8.GetBytes(html))
        };

        return new DataFlowContext(
            null,
            new SpiderOptions(),
            new Request(url),
            response);
    }
}
