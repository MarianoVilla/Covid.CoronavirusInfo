using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CountryInfo
    {
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public int _Id { get; set; }
        public int Lat { get; set; }
        [JsonProperty("long")]
        public int Longitude { get; set; }
        [JsonProperty("flag")]
        public string FlagUri { get; set; }
    }
}
