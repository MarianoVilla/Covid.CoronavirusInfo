using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CovidCountryReport : CovidReport
    {
        public string Country { get; set; }
        public string RegionalFriendlyName { get; set; }
        public bool IsFavourite { get; set; }
        public string CountryCode { get; set; }
        public int? TodayCases { get; set; }
        public int? TodayDeaths { get; set; }
        public int? Active { get; set; }
        public int? Critical { get; set; }
        public int? CasesPerOneMillion { get; set; }
        /// <summary>
        /// Some of the APIs provide this nested object. I already have most of the data, but since it's being downloaded anyways, we might as well deserialize it.
        /// </summary>
        public CountryInfo CountryInfo { get; set; }
        public IEnumerable<CountryTimeseriesDay> Timeseries { get; set; }
    }
}
