using System.Threading.Tasks;
using DotnetSpider;
using Microsoft.Extensions.Hosting;

namespace SpiderAutoHome
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Builder.CreateDefaultBuilder<AutoHomeSpider>(
                args,
                options =>
                {
                    // Legacy Site.SleepTime = 200 ms ≈ 5 requests/second.
                    options.Speed = 5;

                    // Legacy Site.CycleRetryTimes = 1.
                    options.RetriedTimes = 1;
                });

            await builder.Build().RunAsync();
        }
    }
}
