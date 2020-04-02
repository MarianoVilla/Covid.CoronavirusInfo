using Android.Content;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;

namespace Covid.Droid.Helpers
{
    public class SharedPreferencesHandler
    {
        //@ToDo see if we need to make this asychronous.
        //@body Commit is async by default, so the only (presumably) noticeable overhead we would avoid is the ToJson() call.
        static string CacheKey = "cache";
        public static void SaveGlobalReport(Context context, CovidReport GlobalReport)
        {
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("global", GlobalReport.ToJson()).Commit();
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("global_timestamp", DateTime.Now.ToString()).Commit();
        }
        public static void SaveCountriesReport(Context context, IEnumerable<CovidCountryReport> CountriesReport)
        {
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("by_countries", CountriesReport.ToJson()).Commit();
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("by_countries_timestamp", DateTime.Now.ToString()).Commit();
        }

        public static void SetUseDarkTheme(Context context, bool ShouldUse) 
            => context.GetSharedPreferences("preferences", FileCreationMode.Private).Edit().PutBoolean("UseDarkTheme", ShouldUse).Commit();
        public static bool ShouldUseDarkTheme(Context context)
            => context.GetSharedPreferences("preferences", FileCreationMode.Private).GetBoolean("UseDarkTheme", false);

        public static void SaveCountryTimeseriesContainer(Context context, CountryTimeseriesContainer CountryTimeseries)
        {
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("timeseries", CountryTimeseries.ToJson()).Commit();
            context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().PutString("timeseries_timestamp", DateTime.Now.ToString()).Commit();
        }
        public static List<CovidCountryReport> GetCountriesReport(Context context)
        {
            return context.GetSharedPreferences(CacheKey, FileCreationMode.Private).GetString("by_countries", null).FromJson<List<CovidCountryReport>>();
        }
        public static DateTime? GetCountriesReportStamp(Context context)
        {
            string ByCountriesStamp = context.GetSharedPreferences(CacheKey, FileCreationMode.Private).GetString("by_countries_timestamp", null);
            if (ByCountriesStamp is null)
                return null;
            return DateTime.Parse(ByCountriesStamp);
        }
        public static DateTime? GetGlobalReportStamp(Context context)
        {
            string GlobalStamp = context.GetSharedPreferences(CacheKey, FileCreationMode.Private).GetString("global_timestamp", null);
            if (GlobalStamp is null)
                return null;
            return DateTime.Parse(GlobalStamp);
        }
        public static DateTime? GetTimeseriesStamp(Context context)
        {
            string TimeseriesStamp = context.GetSharedPreferences(CacheKey, FileCreationMode.Private).GetString("timeseries_timestamp", null);
            if (TimeseriesStamp is null)
                return null;
            return DateTime.Parse(TimeseriesStamp);
        }
        public static CovidReport GetCovidReport(Context context)
        {
            return context.GetSharedPreferences(CacheKey, FileCreationMode.Private).GetString("global", null).FromJson<CovidReport>();
        }
        public static void ClearCache(Context context) => context.GetSharedPreferences(CacheKey, FileCreationMode.Private).Edit().Clear().Commit();
    }
}