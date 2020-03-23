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
        const string COM_PORT = "com3";
        const int LONG_DELAY = 15 * SHORT_DELAY;  // 15 Minutes
        const int SHORT_DELAY = 60 * 1000;        // 1 minute

        private readonly ILogger<Worker> _logger;
        private readonly DisplayInterface _display;

        public Worker(ILogger<Worker> logger, DisplayInterface display)
        {
            _logger = logger;
            _display = display;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                while (!stoppingToken.IsCancellationRequested)
                {
                    int delay = LONG_DELAY;
                    try
                    {
                        if(!await _display.RequestData(COM_PORT))
                            delay = SHORT_DELAY;
                    }
                    catch (FileNotFoundException fnfe)
                    {
                        _logger.LogWarning($"Could not connect to display: {fnfe.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Unknown error");
                    }
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
    }
}
