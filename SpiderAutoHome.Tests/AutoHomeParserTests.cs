using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using Xunit;

namespace SpiderAutoHome.Tests
{
    public class AutoHomeParserTests
    {
        [Fact]
        public async Task ParseAsync_ShouldExtractCarInformation()
        {
            const string html = """
                <html>
                <body>
                    <div class="list">
                        <ul class="fn-clear">
                            <li class="carbox">
                                <a href="https://example.com/car/1">
                                    <div class="carbox-carimg">
                                        <img src="https://example.com/car.jpg" />
                                    </div>

                                    <div class="carbox-title">
                                        Test Car
                                    </div>

                                    <div class="carbox-tip">
                                        Test Tip
                                    </div>

                                    <div class="carbox-number">
                                        <span>10</span>
                                    </div>

                                    <div class="carbox-info">
                                        ¥100 ¥120
                                    </div>
                                </a>
                            </li>
                        </ul>
                    </div>
                </body>
                </html>
                """;

            var request = new Request(
                "https://store.mall.autohome.com.cn/test");

            var response = new Response
            {
                Content = new ByteArrayContent(
                    Encoding.UTF8.GetBytes(html))
            };

            using var context = new DataFlowContext(
                null,
                new SpiderOptions(),
                request,
                response);

            var parser = new AutoHomeParser();

            await parser.HandleAsync(
                context,
                _ => Task.CompletedTask);

            var cars =
                context.GetData("CarList")
                as List<AutoHomeShopListEntity>;

            Assert.NotNull(cars);
            Assert.Single(cars);

            var car = cars[0];

            Assert.Equal(
                "https://example.com/car/1",
                car.DetailUrl);

            Assert.Equal(
                "https://example.com/car.jpg",
                car.CarImg);

            Assert.Equal("Test Car", car.Title.Trim());
            Assert.Equal("Test Tip", car.Tip.Trim());
            Assert.Equal("10", car.BuyNum.Trim());

            Assert.Equal("100", car.Price);
            Assert.Equal("120", car.DelPrice);
        }
    }
}
