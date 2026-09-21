using DotnetSpider;
using Microsoft.Extensions.Hosting;

namespace SpiderAutoLogo;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Builder.CreateDefaultBuilder<AutoLogoSpider>(
            args,
            options =>
            {
                options.Speed = 1;
                options.RetriedTimes = 1;
            });

        await builder.Build().RunAsync();
    }
}
