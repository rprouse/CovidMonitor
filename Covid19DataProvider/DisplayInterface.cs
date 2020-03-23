using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19DataProvider
{
    public class DisplayInterface: IDisposable
    {
        CovidService _service = new CovidService();
        SerialPort _serial;

        public DisplayInterface(string comPort)
        {
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

            _serial.Open();
        }

        public async Task RequestData()
        {
            Console.WriteLine("Requesting data...");

            var result = await _service.FetchConfirmedCasesFromJohnHopkins();
            int all = result.Sum(d => d.Confirmed);

            char r = (char)_serial.ReadChar();
            Console.WriteLine($"Ready to receive {r}");
            await Task.Delay(100);

            Console.WriteLine($"ALL: {all}");
            _serial.Write($"ALL: {all}\0");
            r = (char)_serial.ReadChar();
            Console.WriteLine($"ALL received {r}");

            int canada = result.Where(d => d.Country == "Canada").Sum(d => d.Confirmed);
            Console.WriteLine($"CAD: {canada}");
            _serial.Write($"CAD: {canada}\0");
            r = (char)_serial.ReadChar();
            Console.WriteLine($"CAD received {r}");

            int us = result.Where(d => d.Country == "US").Sum(d => d.Confirmed);
            Console.WriteLine($"USA: {us}");
            _serial.Write($"USA: {us}\0");
            r = (char)_serial.ReadChar();
            Console.WriteLine($"USA received {r}");
        }

        public void Dispose()
        {
            if(_serial != null)
            {
                _serial.Dispose();
                _serial = null;
            }
        }
    }
}
