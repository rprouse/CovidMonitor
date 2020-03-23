using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Covid19DataProvider
{
    public class DisplayInterface
    {
        private readonly ILogger<DisplayInterface> _logger;
        CovidService _service;

        public DisplayInterface(ILogger<DisplayInterface> logger, CovidService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task RequestData(string comPort)
        {
            SerialPort _serial = null;

            _serial = new SerialPort
            {
                PortName = comPort,
                BaudRate = 57600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                DtrEnable = true
            };

            try
            {
                _serial.Open();

                _logger.LogInformation("Requesting data...");

                var result = await _service.FetchConfirmedCases();
                int all = result.Sum(d => d.Confirmed);

                char r = (char)_serial.ReadChar();
                _logger.LogDebug($"Ready to receive {r}");
                await Task.Delay(100);

                _logger.LogInformation($"ALL: {all}");
                _serial.Write($"ALL: {all}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"ALL received {r}");

                int canada = result.Where(d => d.Country == "Canada").Sum(d => d.Confirmed);
                _logger.LogInformation($"CAD: {canada}");
                _serial.Write($"CAD: {canada}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"CAD received {r}");

                int us = result.Where(d => d.Country == "US").Sum(d => d.Confirmed);
                _logger.LogInformation($"USA: {us}");
                _serial.Write($"USA: {us}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"USA received {r}");
            }
            finally
            {
                _serial.Close();
            }
        }
    }
}
