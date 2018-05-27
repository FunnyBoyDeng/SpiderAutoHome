using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using System;
using System.Collections.Generic;

namespace SpiderAutoHome
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
                    { "Accept","text/html, */*; q=0.01" },
                    { "Referer", "https://store.mall.autohome.com.cn/83106681.html"},
                    { "Cache-Control","no-cache" },
                    { "Connection","keep-alive" },
                    { "Content-Type","application/x-www-form-urlencoded; charset=UTF-8" },
                    { "User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36"}
                    //{ "Cookie","fvlid=1496991192019sTVlIox4; sessionid=792A6590-B1C1-4292-902E-BAB469F6B66A%7C%7C2017-06-09+14%3A52%3A46.308%7C%7Cwww.baidu.com; mallsfvi=1501164251817G4Yjfly2%7Cstore.mall.autohome.com.cn%7C0; ag_fid=P3aAofwpa4LibiRF; Hm_lvt_765ecde8c11b85f1ac5f168fa6e6821f=1507955352; cookieCityId=110100; __utma=1.675003556.1498310668.1509877694.1512100363.4; __utmz=1.1512100363.4.4.utmcsr=autohome.com.cn|utmccn=(referral)|utmcmd=referral|utmcct=/beijing/; sessionuid=792A6590-B1C1-4292-902E-BAB469F6B66A%7C%7C2017-06-09+14%3A52%3A46.308%7C%7Cwww.baidu.com; UM_distinctid=15d58caae69692-00e158964a2cc1-8383667-100200-15d58caae6a8f3; cn_1262640694_dplus=%7B%22distinct_id%22%3A%20%2215d58caae69692-00e158964a2cc1-8383667-100200-15d58caae6a8f3%22%2C%22sp%22%3A%20%7B%22%24_sessionid%22%3A%200%2C%22%24_sessionTime%22%3A%201513783651%2C%22%24dp%22%3A%200%2C%22%24_sessionPVTime%22%3A%201513783651%7D%7D; _ga=GA1.3.675003556.1498310668; ahsids=588; Hm_lvt_9924a05a5a75caf05dbbfb51af638b07=1513782253,1514425536,1516346152; ahpau=1; o2oPlatform_user_info_new=\"2PEXHhEpMSe8Ab9wePPfBM/cPTSA+LEhouYSvKqf21ifTs8v3Wa7+5ML9XHTa4PIxdohDy/pfgZn+7jU57K8QJk6GqJpuAFmGdo4AAkQj1s=\"; providerLogin=\"2PEXHhEpMSe8Ab9wePPfBM/cPTSA+LEhouYSvKqf21ifTs8v3Wa7+5ML9XHTa4PIxdohDy/pfgZn+7jU57K8QJk6GqJpuAFmGdo4AAkQj1s=\"; o2oPlatform_company_user_info=2PEXHhEpMSco9C5zzw6CuOMB4aAqz2tF4FWkU/d1pCIKwHFVei9AI0tef+vYWhLhKK6S2blgHM0hg0WFK8FWIX+0p68SYs23; area=431099; mallslvi=0%7C%7C15187909317891KBbjcg5; sessionip=223.152.110.108; mallCityId=999999; ahpvno=21; sessionvid=554FA305-FACF-4617-B002-CEBC69D55AE3; ref=www.mangoauto.com.cn%7C0%7C0%7Cwww.baidu.com%7C2018-02-17+09%3A45%3A31.833%7C2018-02-11+21%3A19%3A03.217; ahrlid=1518831923227kTwIQZVJ-1518832004398" },


                }

            };


            List<Request> resList = new List<Request>();
            for (int i = 1; i <= 33; i++)
            {
                Request res = new Request();
                res.PostBody = $"id=7&j=%7B%22createMan%22%3A%2218273159100%22%2C%22createTime%22%3A1518433690000%2C%22row%22%3A5%2C%22siteUserActivityListId%22%3A8553%2C%22siteUserPageRowModuleId%22%3A84959%2C%22topids%22%3A%22%22%2C%22wherePhase%22%3A%221%22%2C%22wherePreferential%22%3A%220%22%2C%22whereUsertype%22%3A%220%22%7D&page={i}&shopid=83106681";
                res.Url = "https://store.mall.autohome.com.cn/shop/ajaxsitemodlecontext.jtml";
                res.Method = System.Net.Http.HttpMethod.Post;

                resList.Add(res);
            }


            var spider = Spider.Create(site, new QueueDuplicateRemovedScheduler(), new AutoHomeProcessor())
                .AddStartRequests(resList.ToArray())
                .AddPipeline(new AutoHomePipe());
            spider.ThreadNum = 1;
            spider.Run();
            Console.Read();
        }

        private class AutoHomeProcessor : BasePageProcessor
        {
            protected override void Handle(Page page)
            {
                List<AutoHomeShopListEntity> list = new List<AutoHomeShopListEntity>();
                var modelHtmlList = page.Selectable.XPath(".//div[@class='list']/ul[@class='fn-clear']/li[@class='carbox']").Nodes();
                foreach (var modelHtml in modelHtmlList)
                {
                    AutoHomeShopListEntity entity = new AutoHomeShopListEntity();
                    entity.DetailUrl = modelHtml.XPath(".//a/@href").GetValue();
                    entity.CarImg = modelHtml.XPath(".//a/div[@class='carbox-carimg']/img/@src").GetValue();
                    var price = modelHtml.XPath(".//a/div[@class='carbox-info']").GetValue(DotnetSpider.Core.Selector.ValueOption.InnerText).Trim().Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).TrimStart('¥').Split("¥");
                    if (price.Length > 1)
                    {
                        entity.Price = price[0];
                        entity.DelPrice = price[1];
                    }
                    else
                    {
                        entity.Price = price[0];
                        entity.DelPrice = price[0];
                    }
                    entity.Title = modelHtml.XPath(".//a/div[@class='carbox-title']").GetValue();
                    entity.Tip = modelHtml.XPath(".//a/div[@class='carbox-tip']").GetValue();
                    entity.BuyNum = modelHtml.XPath(".//a/div[@class='carbox-number']/span").GetValue();
                    list.Add(entity);
                }
                page.AddResultItem("CarList", list);
            }

        }
        private class AutoHomePipe : BasePipeline
        {

            public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
            {
                foreach (var resultItem in resultItems)
                {
                    Console.WriteLine((resultItem.Results["CarList"] as List<AutoHomeShopListEntity>).Count);
                    foreach (var item in (resultItem.Results["CarList"] as List<AutoHomeShopListEntity>))
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }


        class AutoHomeShopListEntity : SpiderEntity
        {
            public string DetailUrl { get; set; }
            public string CarImg { get; set; }
            public string Price { get; set; }
            public string DelPrice { get; set; }
            public string Title { get; set; }
            public string Tip { get; set; }
            public string BuyNum { get; set; }

            public override string ToString()
            {
                return $"{Title}|{Price}|{DelPrice}|{BuyNum}";
            }
        }
    }


}
