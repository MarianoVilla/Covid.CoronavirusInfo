using Covid.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Covid.Lib
{
    public static class ExtensionMethods
    {
        public static bool IsWorking(this Uri TheUri, string Action = null)
        {
            TheUri = Action is null ? TheUri : new Uri($"{TheUri}{Action}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TheUri);
            request.Timeout = 10000;
            request.Method = "HEAD";
            try
            {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
        public static string ToDescriptionString<T>(this T val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        /// <summary>
        /// Loads the regional-friendly names for the countries returned by the APIs (which are in english). Right now, the asset containing the equivalences is a hardcoded JSON with the EN/ES match.
        /// If we need to make the app other-cultures-friendly, we would have to make a dynamic asset. As for now, YAGNI.
        /// </summary>
        /// <param name="Countries"></param>
        public static void LoadRegionalFriendlyNames(this IEnumerable<CovidCountryReport> Countries)
        {
            if (Const.CountryEquiv.NamesEnToLocale == null || !Const.CountryEquiv.NamesEnToLocale.Any())
                return;
            foreach(var c in Countries)
            {
                c.RegionalFriendlyName = Const.CountryEquiv.NamesEnToLocale.FirstOrDefault(x => x.Key == c.Country).Value;
            }
        }
        public static void LoadCountryCodes(this IEnumerable<CovidCountryReport> Countries)
        {
            foreach(var c in Countries)
            {
                c.CountryCode = Const.CountryEquiv.NamesCodes.FirstOrDefault(x => x.Key == c.Country).Value;
            }
        }
        public static void LoadTimeseries(this IEnumerable<CovidCountryReport> Countries)
        {
            foreach (var c in Countries)
            {
                Const.TimeseriesContainer.Timeseries.TryGetValue(c.Country, out List<CountryTimeseriesDay> Timeseries);
                c.Timeseries = Timeseries;
            }
        }
        public static void LoadTimeseries(this CovidCountryReport TheCountry)
        {
            Const.TimeseriesContainer.Timeseries.TryGetValueMultiKey(out List<CountryTimeseriesDay> Timeseries, TheCountry.Country, TheCountry.RegionalFriendlyName, TheCountry.CountryCode);
            TheCountry.Timeseries = Timeseries;
        }
        public static bool TryGetValueMultiKey<T>(this Dictionary<string, T> TheDictionary, out T TheValue, params string[] Keys) where T : class
        {
            TheValue = null;
            if (Keys is null)
                return false;
            foreach (var k in Keys)
            {
                if (TheDictionary.TryGetValueCaseInsensitive(out TheValue, k))
                    return true;
            }
            return false;
        }
        public static bool TryGetValueCaseInsensitive<T>(this Dictionary<string, T> TheDictionary, out T TheValue, string Key) where T : class
        {
            TheValue = null;
            if (Key is null)
                return false;
            foreach (var dk in TheDictionary.Keys)
            {
                if (Key.ToLower() == dk.ToLower())
                {
                    TheValue = TheDictionary[dk];
                    return true;
                }
            }
            return false;
        }
        public static string ToJson(this object TheObject) => JsonConvert.SerializeObject(TheObject);
        public static T FromJson<T>(this string TheJson) => JsonConvert.DeserializeObject<T>(TheJson);
        public static T FromJsonIntoAnonymous<T>(this string TheJson) where T : new() => JsonConvert.DeserializeAnonymousType(TheJson, new T());
        public static string ToKMB(this decimal num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
        public static long AsLong(this string TheString)
        {
            if (long.TryParse(TheString, out long TheResult))
                return TheResult;
            return 0;
        }
        public static string TryToLongKMB(this string TheString)
        {
            var AsLong = TheString.AsLong();
            return AsLong == 0 ? TheString : AsLong.ToKMB();
        }
        public static string ToKMB(this long num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
        public static string ToKMB(this int num)
        {
            return ((long)num).ToKMB();
        }
        public static string ToKMB(this int? num)
        {
            return num is null ? null : ((int)num).ToKMB();
        }
    }
}
