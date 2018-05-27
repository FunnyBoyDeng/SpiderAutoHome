using System;
using System.Collections.Generic;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Extension.Model;
using Newtonsoft.Json;

namespace SpiderAutoSkuData
{
    class Program
    {
        static void Main(string[] args)
        {

            var site = new Site
            {
                CycleRetryTimes = 1,
                SleepTime = 200,
                Headers = new Dictionary<string, string>()
                {
                    { "Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
                    { "Cache-Control","no-cache" },
                    { "Connection","keep-alive" },
                    { "Content-Type","application/x-www-form-urlencoded; charset=UTF-8" },
                    { "User-Agent","Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36"}
                }

            };
            List<Request> resList = new List<Request>();
            Request res = new Request();
            res.Url = "https://mall.autohome.com.cn/detail/284641-0-0.html";
            res.Method = System.Net.Http.HttpMethod.Get;
            resList.Add(res);
            var spider = Spider.Create(site, new QueueDuplicateRemovedScheduler(), new GetSkuProcessor(),new GetBasicInfoProcessor(),new GetExtInfoProcessor()) //因为我们有多个Processor，所以都要添加进来
                .AddStartRequests(resList.ToArray())
                .AddPipeline(new PrintSkuPipe());
            spider.ThreadNum = 1;
            spider.Run();
            Console.Read();
        }

        private class GetSkuProcessor : BasePageProcessor //获取skuid
        {
            public GetSkuProcessor()
            {
                TargetUrlsExtractor = new RegionAndPatternTargetUrlsExtractor(".", @"^https://mall\.autohome\.com\.cn/detail/*");
            }
            protected override void Handle(Page page)
            {
                string skuid = string.Empty;
                skuid = page.Selectable.XPath(".//a[@class='carbox-compare_detail']/@link").GetValue();
                page.AddResultItem("skuid", skuid);
                page.AddTargetRequest(@"https://mall.autohome.com.cn/http/data.html?data[_host]=//car.api.autohome.com.cn/v1/carprice/spec_paramsinglebyspecid.ashx&data[_appid]=mall&data[specid]=" + skuid);
                page.AddTargetRequest(@"https://mall.autohome.com.cn/http/data.html?data[_host]=//car.api.autohome.com.cn/v2/carprice/Config_GetListBySpecId.ashx&data[_appid]=mall&data[specid]=" + skuid);
            }

        }
        private class GetBasicInfoProcessor : BasePageProcessor //获取车型基本参数
        {
            public GetBasicInfoProcessor()
            {
                TargetUrlsExtractor = new RegionAndPatternTargetUrlsExtractor(".", @"^https://mall\.autohome\.com\.cn/http/data\.html\?data\[_host\]=//car\.api\.autohome\.com\.cn/v1/carprice/spec_paramsinglebyspecid\.ashx*");
            }
            protected override void Handle(Page page)
            {
                page.AddResultItem("BaseInfo", page.Content);
            }
        }

        private class GetExtInfoProcessor : BasePageProcessor //获取车型配置
        {
            public GetExtInfoProcessor()
            {
                TargetUrlsExtractor = new RegionAndPatternTargetUrlsExtractor(".", @"^https://mall\.autohome\.com\.cn\/http\/data\.html\?data\[_host\]=//car\.api\.autohome\.com\.cn/v2/carprice/Config_GetListBySpecId\.ashx*");
            }
            protected override void Handle(Page page)
            {
                page.AddResultItem("ExtInfo", page.Content);
            }
        }

        private class PrintSkuPipe : BasePipeline
        {

            public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
            {
                foreach (var resultItem in resultItems)
                {
                    if (resultItem.GetResultItem("skuid") != null)
                    {
                        Console.WriteLine(resultItem.Results["skuid"] as string);
                    }
                    if (resultItem.GetResultItem("BaseInfo") != null)
                    {
                        var t = JsonConvert.DeserializeObject<AutoCarParam>(resultItem.Results["BaseInfo"]);
                        //Console.WriteLine(resultItem.Results["BaseInfo"]);
                    }
                    if (resultItem.GetResultItem("ExtInfo") != null)
                    {
                        var t = JsonConvert.DeserializeObject<AutoCarConfig>(resultItem.Results["ExtInfo"]);
                        //Console.WriteLine(resultItem.Results["ExtInfo"]);
                    }

                }
                
            }
        }
    }
}
