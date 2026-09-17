using System.Threading;
using System.Threading.Tasks;
using DotnetSpider;
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

        protected override Task InitializeAsync(
            CancellationToken stoppingToken = default)
        {
            AddDataFlow<AutoHomeParser>();

            return Task.CompletedTask;
        }
    }
}
