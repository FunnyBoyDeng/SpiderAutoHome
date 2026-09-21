using System.Threading;
using DotnetSpider;
using DotnetSpider.DataFlow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpiderAutoLogo;

public sealed class AutoLogoSpider : Spider
{
    public AutoLogoSpider(
        IOptions<SpiderOptions> options,
        DependenceServices services,
        ILogger<Spider> logger)
        : base(options, services, logger)
    {
    }

    protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
    {
        AddDataFlow<LogoParser>();
        AddDataFlow<ConsoleStorage>();
        AddDataFlow<LogoDownloadFlow>();

        var configuredUrl = Environment.GetEnvironmentVariable("SPIDERAUTOHOME_LOGO_URL");
        await AddRequestsAsync(LogoRequestFactory.Create(configuredUrl));
    }

    protected override void ConfigureRequest(DotnetSpider.Http.Request request)
    {
        request.Headers.Accept = "text/html,application/xhtml+xml;q=0.9,*/*;q=0.8";
    }
}
