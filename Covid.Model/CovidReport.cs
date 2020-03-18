using System;
using System.Collections.Generic;
using System.Text;

namespace Covid.Model
{
    public class CovidReport
    {
        public int? Cases { get; set; }
        public int? Deaths { get; set; }
        public int? Recovered { get; set; }
        public string Updated { get; set; }
        public double? DeathRate { 
            get {
                try
                {
                    return Math.Round(((double)Deaths / (double)Cases * 100), 3);
                }
                catch (Exception)
                {
                    return null;
                }
            } 
        }
    }
}
