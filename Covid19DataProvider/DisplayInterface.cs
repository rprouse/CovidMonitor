using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

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
                Handshake = Handshake.None
            };

            _serial.DataReceived += DataReceiveHandler;

            _serial.Open();
        }

        private async void DataReceiveHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string data = _serial.ReadExisting();
            Console.WriteLine($"Data received {data}");
            Console.WriteLine("Fetching data");

            var result = await _service.FetchConfirmedCasesFromJohnHopkins();
            int all = result.Sum(d => d.Confirmed);
            Console.WriteLine($"ALL: {all}");
            _serial.WriteLine($"ALL: {all}");

            int canada = result.Where(d => d.Country == "Canada").Sum(d => d.Confirmed);
            Console.WriteLine($"CAD: {canada}");
            _serial.WriteLine($"CAD: {canada}");

            int us = result.Where(d => d.Country == "US").Sum(d => d.Confirmed);
            Console.WriteLine($"USA: {us}");
            _serial.WriteLine($"USA: {us}");
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
