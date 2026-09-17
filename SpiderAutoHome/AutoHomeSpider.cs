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
            // 注册页面解析器
            AddDataFlow<AutoHomeParser>();

            // 将解析结果输出到控制台
            AddDataFlow<ConsoleStorage>();

            var requests = new List<Request>();

            // 原项目共抓取 33 页
            for (var page = 1; page <= 33; page++)
            {
                var formData =
                    $"id=7&j=%7B%22createMan%22%3A%2218273159100%22%2C%22createTime%22%3A1518433690000%2C%22row%22%3A5%2C%22siteUserActivityListId%22%3A8553%2C%22siteUserPageRowModuleId%22%3A84959%2C%22topids%22%3A%22%22%2C%22wherePhase%22%3A%221%22%2C%22wherePreferential%22%3A%220%22%2C%22whereUsertype%22%3A%220%22%7D&page={page}&shopid=83106681";

                var request = new Request(
                    "https://store.mall.autohome.com.cn/shop/ajaxsitemodlecontext.jtml")
                {
                    Method = "POST",

                    // 使用 DotnetSpider 自己的 StringContent，
                    // 直接指定 UTF-8 和表单 Content-Type。
                    Content = new StringContent(
                        formData,
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded")
                };

                requests.Add(request);
            }

            await AddRequestsAsync(requests);
        }
    }
}
