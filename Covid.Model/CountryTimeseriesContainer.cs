using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CountryTimeseriesContainer
    {
        public string Country { get; set; }
        //public bool Changed { get; set; }
        public IEnumerable<CountryTimeseriesDay> TimeseriesDays { get; set; }
    }
}
