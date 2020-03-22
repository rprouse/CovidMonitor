using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Covid19DataProvider
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var service = new CovidService();
            var result = await service.FetchConfirmedCasesFromJohnHopkins();
            int all = result.Sum(d => d.Confirmed);
            Console.WriteLine($"Total: {all}");

            int canada = result.Where(d => d.Country == "Canada").Sum(d => d.Confirmed);
            Console.WriteLine($"Canada: {canada}");

            int us = result.Where(d => d.Country == "US").Sum(d => d.Confirmed);
            Console.WriteLine($"US: {us}");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
