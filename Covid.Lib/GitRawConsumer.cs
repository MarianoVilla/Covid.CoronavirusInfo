using Covid.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Covid.Lib
{
    public class GitRawConsumer
    {
        static string Endpoint = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/";
        static HttpClient Client = new HttpClient() { BaseAddress = new Uri(Endpoint) };

        //public static async Task GetDailyCsv()
        //{
        //    try
        //    {
        //        var Date = DateTime.Now.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
        //        var Result = await Client.GetStringAsync($"{Date}.csv");
        //        var ParsedResult = Result.Split(Environment.NewLine.ToArray())
        //            .Skip(1)
        //            .Select(x => x.Split(',')
        //            .Select(y => new CovidCountryReport { Country = y[0], Country = y[1], LastUpdate = y[2], Confirmed = y[3] })).ToList();

        //        var First = ParsedResult.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

    }
}
