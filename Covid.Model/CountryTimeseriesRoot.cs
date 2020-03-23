using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CountryTimeseriesRoot
    {
        public Dictionary<string, IEnumerable<CountryTimeseriesDay>> TimeseriesContainers { get; set; }
    }
}
