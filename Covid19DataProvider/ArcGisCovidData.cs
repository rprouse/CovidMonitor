using System;

namespace Covid19DataProvider
{
    public class ArcGisCovidData
    {
        public bool exceededTransferLimit { get; set; }
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public int OBJECTID { get; set; }
        public string Province_State { get; set; }
        public string Country_Region { get; set; }
        public float? Lat { get; set; }
        public float? Long_ { get; set; }
        public int Confirmed { get; set; }
        public int Recovered { get; set; }
        public int Deaths { get; set; }
        public int Active { get; set; }
    }
}
