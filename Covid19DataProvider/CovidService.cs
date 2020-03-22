using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
        const string ARC_GIS_QUERY_URI = "https://services9.arcgis.com/N9p5hsImWXAccRNI/arcgis/rest/services/Z7biAeD8PAkqgmWhxG2A/FeatureServer/1/query?f=json&where=(Confirmed%20%3E%200)&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=*&orderByFields=Confirmed%20desc%2CCountry_Region%20asc%2CProvince_State%20asc&outSR=102100&resultOffset={0}&resultRecordCount={1}&cacheHint=true";
        const int RECORD_COUNT = 250;

        HttpClient _client = new HttpClient();

        public async Task<IList<CovidData>> FetchConfirmedCasesFromGitHub()
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
                        Confirmed = csv.GetField<int>(csv.Context.Record.Length - 1)
                    };
                    records.Add(record);
                }
                return records;
            }
        }

        public async Task<IList<CovidData>> FetchConfirmedCasesFromJohnHopkins()
        {
            var records = new List<CovidData>();

            int offset = 0;
            bool more = true;

            while (more)
            {
                var uri = string.Format(ARC_GIS_QUERY_URI, offset, RECORD_COUNT);

                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36 Edg/80.0.361.69");
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("If-Modified-Since", "Sun, 22 Mar 2020 19:43:35 GMT");
                request.Headers.Add("Origin", "https://gisanddata.maps.arcgis.com");
                request.Headers.Add("Referer", "https://gisanddata.maps.arcgis.com/apps/opsdashboard/index.html");

                HttpResponseMessage response = await _client.SendAsync(request);
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();

                using (MemoryStream compressed = new MemoryStream(bytes))
                using (GZipStream gzip = new GZipStream(compressed, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzip))
                {
                    string json = reader.ReadToEnd();
                    ArcGisCovidData data = Newtonsoft.Json.JsonConvert.DeserializeObject<ArcGisCovidData>(json);

                    IEnumerable<CovidData> covidData = data.features.Select(f => new CovidData {
                        Country = f.attributes.Country_Region,
                        Province = f.attributes.Province_State,
                        Confirmed = f.attributes.Confirmed,
                        Deaths = f.attributes.Deaths,
                        Recovered = f.attributes.Recovered
                    });

                    records.AddRange(covidData);

                    more = data.exceededTransferLimit;
                    offset += RECORD_COUNT;
                }
            }

            return records;
        }
    }
}