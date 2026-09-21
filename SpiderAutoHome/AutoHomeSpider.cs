using System.Threading;
using System.Threading.Tasks;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpiderAutoHome
{
    public class AutoHomeSpider : Spider
    {
        public AutoHomeSpider(
            IOptions<SpiderOptions> options,
            DependenceServices services,
            ILogger<Spider> logger)
            : base(options, services, logger)
        {
        }

        protected override async Task InitializeAsync(
            CancellationToken stoppingToken = default)
        {
            AddDataFlow<AutoHomeParser>();
            AddDataFlow<ConsoleStorage>();

            var configuredUrl =
                Environment.GetEnvironmentVariable("SPIDERAUTOHOME_LIST_URL");
            var configuredPageCount =
                Environment.GetEnvironmentVariable("SPIDERAUTOHOME_PAGE_COUNT");

            await AddRequestsAsync(
                AutoHomeRequestFactory.Create(configuredUrl, configuredPageCount));
        }

        protected override void ConfigureRequest(Request request)
        {
            // Preserve only the non-sensitive headers that were
            // relevant to the original sample.
            request.Headers.Accept = "text/html, */*; q=0.01";
            request.Headers.Referrer =
                "https://store.mall.autohome.com.cn/83106681.html";
        }
    }
}
