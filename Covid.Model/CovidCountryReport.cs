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
    }
}
