using DotnetSpider;
using Microsoft.Extensions.Hosting;

namespace SpiderAutoSkuData;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Builder.CreateDefaultBuilder<AutoSkuSpider>(
            args,
            options =>
            {
                options.Speed = 1;
                options.RetriedTimes = 1;
                options.Depth = 2;
            });

        await builder.Build().RunAsync();
    }
}
