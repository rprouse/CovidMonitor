using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Covid19DataProvider
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var display = new DisplayInterface("com3"))
                        {
                            await display.RequestData();
                        }
                    }
                    catch (FileNotFoundException fnfe)
                    {
                        Console.WriteLine($"Could not connect to display: {fnfe.Message}");
                    }
                    await Task.Delay(60 * 60 * 1000, stoppingToken);
                }
            }
        }
    }
}
