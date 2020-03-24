using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Covid.Model
{
    public class Const
    {
        public static readonly Uri[] GlobalEndpoints = { new Uri(@"https://coronavirus-19-api.herokuapp.com/All"), new Uri(@"https://corona.lmao.ninja/All") };
        public static readonly Uri[] ByCountriesEndpoints = { new Uri(@"https://coronavirus-19-api.herokuapp.com/Countries"), new Uri(@"https://corona.lmao.ninja/Countries") };
        public static readonly Uri[] TimeseriesEndpoints = { new Uri(@"https://pomber.github.io/covid19/timeseries.json") };
        public static HttpClient GlobalHttpClient;

        //@ToDo: review this. The equivalences and the timeseries are not constants, they're global state.
        public static CountryEquivalences CountryEquiv;
        public static CountryTimeseriesContainer TimeseriesContainer { get; set; } = new CountryTimeseriesContainer();

    }
}
