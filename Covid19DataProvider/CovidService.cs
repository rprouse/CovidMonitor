using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;

namespace Covid19DataProvider
{
    public class CovidService
    {
        const string CONFIRMED_URI = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv";

        HttpClient _client = new HttpClient();

        public async Task<IList<CovidData>> FetchConfirmedCases()
        {
            string raw = await _client.GetStringAsync(CONFIRMED_URI);

            var regex = new Regex("\\\"(.*?)\\\"");
            var cleaned = regex.Replace(raw, m => m.Value.Replace(",", " in "));

            File.WriteAllText("./Confirmed.csv", cleaned);

            using(var reader = new StreamReader("./Confirmed.csv"))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<CovidData>();
                csv.Read();
                csv.ReadHeader();
                while(csv.Read())
                {
                    var record = new CovidData
                    {
                        Province = csv.GetField(0),
                        Country = csv.GetField(1),
                        Count = csv.GetField<int>(csv.Context.Record.Length - 1)
                    };
                    records.Add(record);
                }
                return records;
            }
        }
    }
}