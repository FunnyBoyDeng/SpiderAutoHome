using System.Threading;
using DotnetSpider;
using DotnetSpider.DataFlow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpiderAutoSkuData;

public sealed class AutoSkuSpider : Spider
{
    public AutoSkuSpider(
        IOptions<SpiderOptions> options,
        DependenceServices services,
        ILogger<Spider> logger)
        : base(options, services, logger)
    {
    }

    protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
    {
        AddDataFlow<AutoSkuParser>();
        AddDataFlow<ConsoleStorage>();

        var configuredUrl = Environment.GetEnvironmentVariable("SPIDERAUTOHOME_SKU_URL");
        await AddRequestsAsync(SkuRequestFactory.CreateDetailRequest(configuredUrl));
    }

    protected override void ConfigureRequest(DotnetSpider.Http.Request request)
    {
        request.Headers.Accept = "text/html,application/json;q=0.9,*/*;q=0.8";
    }
}
