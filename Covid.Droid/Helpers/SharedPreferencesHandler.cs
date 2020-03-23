using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid.Model;
using Covid.Lib;

namespace Covid.Droid.Helpers
{
    public class SharedPreferencesHandler
    {
        //@ToDo see if we need to make this asychronous.
        //@body Commit is async by default, so the only (presumably) noticeable overhead we would avoid is the ToJson() call.
        public static void SaveGlobalReport(Context context, CovidReport GlobalReport)
        {
            context.GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("global", GlobalReport.ToJson()).Commit();
            context.GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("global_timestamp", DateTime.Now.ToString()).Commit();
        }
        public static void SaveCountriesReport(Context context, IEnumerable<CovidCountryReport> CountriesReport)
        {
            context.GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("by_countries", CountriesReport.ToJson()).Commit();
            context.GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("by_countries_timestamp", DateTime.Now.ToString()).Commit();
        }
        public static List<CovidCountryReport> GetCountriesReport(Context context)
        {
            return context.GetSharedPreferences("cache", FileCreationMode.Private).GetString("by_countries", null).FromJson<List<CovidCountryReport>>();
        }
        public static DateTime? GetCountriesReportStamp(Context context)
        {
            var ByCountriesStamp = context.GetSharedPreferences("cache", FileCreationMode.Private).GetString("by_countries_timestamp", null);
            if (ByCountriesStamp is null)
                return null;
            return DateTime.Parse(ByCountriesStamp);
        }
        public static DateTime? GetGlobalReportStamp(Context context)
        {
            var GlobalStamp = context.GetSharedPreferences("cache", FileCreationMode.Private).GetString("global_timestamp", null);
            if (GlobalStamp is null)
                return null;
            return DateTime.Parse(GlobalStamp);
        }
        public static CovidReport GetCovidReport(Context context)
        {
            return context.GetSharedPreferences("cache", FileCreationMode.Private).GetString("global", null).FromJson<CovidReport>();
        }
    }
}