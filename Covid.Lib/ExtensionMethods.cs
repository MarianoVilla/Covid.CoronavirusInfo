using Covid.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Loads the regional-friendly names for the countries returned by the APIs (which are in english). Right now, the asset containing the equivalences is a hardcoded JSON with the EN/ES match.
        /// If we need to make the app other-cultures-friendly, we would have to make a dynamic asset. As for now, YAGNI.
        /// </summary>
        /// <param name="TheCountries"></param>
        public static void LoadRegionalFriendlyNames(this IEnumerable<CovidCountryReport> TheCountries)
        {
            foreach(var c in TheCountries)
            {
                c.RegionalFriendlyName = Const.CountryEquiv.NamesEnEs.FirstOrDefault(x => x.Key == c.Country).Value;
            }
        }
        public static void LoadCountryCodes(this IEnumerable<CovidCountryReport> TheCountries)
        {
            foreach(var c in TheCountries)
            {
                c.CountryCode = Const.CountryEquiv.NamesCodes.FirstOrDefault(x => x.Key == c.Country).Value;
            }
        }
        public static string ToJson(this object TheObject) => JsonConvert.SerializeObject(TheObject);
        public static T FromJson<T>(this string TheJson) => JsonConvert.DeserializeObject<T>(TheJson);
    }
}
