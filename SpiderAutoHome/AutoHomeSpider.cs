using System.Collections.Generic;
using System.Text;
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

            var requests = new List<Request>();

            for (var page = 1; page <= 33; page++)
            {
                var formData =
                    $"id=7&j=%7B%22createMan%22%3A%2218273159100%22%2C%22createTime%22%3A1518433690000%2C%22row%22%3A5%2C%22siteUserActivityListId%22%3A8553%2C%22siteUserPageRowModuleId%22%3A84959%2C%22topids%22%3A%22%22%2C%22wherePhase%22%3A%221%22%2C%22wherePreferential%22%3A%220%22%2C%22whereUsertype%22%3A%220%22%7D&page={page}&shopid=83106681";

                var request = new Request(
                    "https://store.mall.autohome.com.cn/shop/ajaxsitemodlecontext.jtml")
                {
                    Method = "POST",
                    Content = new StringContent(
                        formData,
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded")
                };

                requests.Add(request);
            }

            await AddRequestsAsync(requests);
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
