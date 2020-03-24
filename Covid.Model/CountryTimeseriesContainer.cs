using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CountryTimeseriesContainer
    {
        public bool Changed { get; set; }
        public Dictionary<string, List<CountryTimeseriesDay>> Timeseries { get; set; }
    }
}
