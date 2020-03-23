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

        public async Task<bool> RequestData(string comPort)
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
                DtrEnable = false
            };

            try
            {
                _serial.Open();

                _logger.LogInformation("Requesting data...");

                var result = await _service.FetchConfirmedCases();
                int all = result.Sum(d => d.Confirmed);

                // Happens when the request quota is reached
                if(all == 0)
                    return false;

                _logger.LogInformation($"ALL: {all}");
                _serial.Write($"ALL: {all}\0");
                char r = (char)_serial.ReadChar();  // We expect an 'r' ACK after each message sent
                _logger.LogDebug($"ALL acknowledged {r}");

                int active = result.Sum(d => d.Active);
                _logger.LogInformation($"ACT: {active}");
                _serial.Write($"ACT: {active}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"ACT acknowledged {r}");

                int canada = result.Where(d => d.Country == "Canada").Sum(d => d.Confirmed);
                _logger.LogInformation($"CAD: {canada}");
                _serial.Write($"CAD: {canada}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"CAD acknowledged {r}");

                int us = result.Where(d => d.Country == "US").Sum(d => d.Confirmed);
                _logger.LogInformation($"USA: {us}");
                _serial.Write($"USA: {us}\0");
                r = (char)_serial.ReadChar();
                _logger.LogDebug($"USA acknowledged {r}");

                // Out of interest, log out some other stats
                var c = result.Where(d => d.Country == "Canada").ToList();
                int dead = result.Sum(d => d.Deaths);
                int caDead = result.Where(d => d.Country == "Canada").Sum(d => d.Deaths);
                int usDead = result.Where(d => d.Country == "US").Sum(d => d.Deaths);
                int uk = result.Where(d => d.Country == "United Kingdom").Sum(d => d.Confirmed);

                string stats = $"Additional stats:\r\n UK: {uk}\r\n Total Dead: {dead}\r\n Canada Dead: {caDead}\r\n US Dead: {usDead}";

                _logger.LogInformation(stats);

                return true;
            }
            finally
            {
                _serial.Close();
            }
        }
    }
}
