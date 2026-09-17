using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using DotnetSpider.Selector;

namespace SpiderAutoHome
{
    public class AutoHomeParser : DataParser
    {
        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task ParseAsync(DataFlowContext context)
        {
            var list = new List<AutoHomeShopListEntity>();

            var modelHtmlList = context.Selectable.SelectList(
                Selectors.XPath(
                    ".//div[@class='list']/ul[@class='fn-clear']/li[@class='carbox']"));

            if (modelHtmlList == null)
            {
                context.AddData("CarList", list);
                return Task.CompletedTask;
            }

            foreach (var modelHtml in modelHtmlList)
            {
                var entity = new AutoHomeShopListEntity
                {
                    DetailUrl = modelHtml
                        .XPath(".//a/@href")
                        ?.Value,

                    CarImg = modelHtml
                        .XPath(".//a/div[@class='carbox-carimg']/img/@src")
                        ?.Value,

                    Title = modelHtml
                        .XPath(".//a/div[@class='carbox-title']")
                        ?.Value,

                    Tip = modelHtml
                        .XPath(".//a/div[@class='carbox-tip']")
                        ?.Value,

                    BuyNum = modelHtml
                        .XPath(".//a/div[@class='carbox-number']/span")
                        ?.Value
                };

                var priceText = modelHtml
                    .XPath(".//a/div[@class='carbox-info']")
                    ?.Value;

                if (!string.IsNullOrWhiteSpace(priceText))
                {
                    var price = priceText
                        .Trim()
                        .Replace(" ", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace("\t", string.Empty)
                        .TrimStart('¥')
                        .Split('¥');

                    entity.Price = price[0];

                    entity.DelPrice =
                        price.Length > 1
                            ? price[1]
                            : price[0];
                }

                list.Add(entity);
            }

            context.AddData("CarList", list);

            return Task.CompletedTask;
        }
    }

    public class AutoHomeShopListEntity
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
