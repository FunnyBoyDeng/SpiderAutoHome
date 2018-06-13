using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Scheduler;
using System;
using System.Collections.Generic;
using DotnetSpider.Core.Downloader;
using System.Net.Http;
using System.IO;
using DotnetSpider.Core.Redial;

namespace SpiderAutoLogo
{
    class Program
    {
        static void Main(string[] args)
        {
            var site = new Site
            {
                CycleRetryTimes = 1,
                SleepTime = 200,
                //DownloadFiles = true,     DotNetSpider中设置是否下载文件
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
            res.Url = "https://car.m.autohome.com.cn/";
            res.Method = System.Net.Http.HttpMethod.Get;
            resList.Add(res);
            var spider = Spider.Create(site, new QueueDuplicateRemovedScheduler(), new GetLogoInfoProcessor()) //因为我们有多个Processor，所以都要添加进来
                .AddStartRequests(resList.ToArray())
                .AddPipeline(new PrintLogInfoPipe());
            spider.ThreadNum = 1;
            spider.Run();
            Console.Read();
        }

        private class GetLogoInfoProcessor : BasePageProcessor //获取Logo信息
        {
            public GetLogoInfoProcessor()
            {
            }
            protected override void Handle(Page page)
            {
                List<LogoInfoModel> logoInfoList = new List<LogoInfoModel>();
                var logoInfoNodes = page.Selectable.XPath(".//div[@id='div_ListBrand']//div[@class='item']").Nodes();
                foreach (var logoInfo in logoInfoNodes)
                {
                    LogoInfoModel model = new LogoInfoModel();
                    model.BrandName = logoInfo.XPath("./strong").GetValue();
                    model.ImgPath = logoInfo.XPath("./img/@src").GetValue();
                    if (model.ImgPath == null)
                    {
                        model.ImgPath = logoInfo.XPath("./img/@data-src").GetValue();
                    }
                    if (model.ImgPath.IndexOf("https") == -1)
                    {
                        model.ImgPath = "https:" + model.ImgPath;
                    }
                    logoInfoList.Add(model);
                    //page.AddTargetRequest(model.ImgPath); //Site设置DownloadFiles为TRUE就可以自动下载文件
                }
                page.AddResultItem("LogoInfoList", logoInfoList);

            }

        }

        private class PrintLogInfoPipe : BasePipeline
        {

            public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
            {

                foreach (var resultItem in resultItems)
                {
                    var logoInfoList = resultItem.GetResultItem("LogoInfoList") as List<LogoInfoModel>;
                    foreach (var logoInfo in logoInfoList)
                    {
                        Console.WriteLine($"brand:{logoInfo.BrandName} path:{logoInfo.ImgPath}");
                        SaveFile(logoInfo.ImgPath, logoInfo.BrandName);
                    }
                }
            }
            private void SaveFile(string url, string filename)
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.RequestUri = new Uri(url);
                httpRequestMessage.Method = HttpMethod.Get;
                HttpClient httpClient = new HttpClient();
                var httpResponse = httpClient.SendAsync(httpRequestMessage);
                string filePath = Environment.CurrentDirectory + "/img/"+ filename + ".jpg";
                if (!File.Exists(filePath))
                {
                    try
                    {
                        string folder = Path.GetDirectoryName(filePath);
                        if (!string.IsNullOrWhiteSpace(folder))
                        {
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                        }

                        File.WriteAllBytes(filePath, httpResponse.Result.Content.ReadAsByteArrayAsync().Result);
                    }
                    catch
                    {
                    }
                }
                httpClient.Dispose();
            }
        }

        private class LogoInfoModel
        {
            public string BrandName { get; set; }
            public string ImgPath { get; set; }
        }
    }

}
