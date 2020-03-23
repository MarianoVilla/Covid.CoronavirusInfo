using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CountryTimeseriesDay
    {
        public DateTime? Date { get; set; }
        public int? Confirmed { get; set; }
        public int? Deaths { get; set; }
        public int? Recovered { get; set; }
    }
}
