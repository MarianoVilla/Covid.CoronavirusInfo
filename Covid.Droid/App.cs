using Android.App;
using Android.Runtime;
using Covid.Lib;
using Covid.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using NetDebug = System.Diagnostics.Debug;

namespace Covid.Droid
{
    [Application]
    public class App : Application
    {
        protected App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void OnCreate()
        {
            Init();
            base.OnCreate();
        }

        private void Init()
        {
            //GitRawConsumer.GetDailyCsv();
            LoadCountryEquivalences();
        }

        void LoadCountryEquivalences()
        {
            Const.CountryEquiv = new CountryEquivalences();
            LoadCountryCodes();
            LoadCountryNames();
            //@ToDo: review. Loading the whole timeseries on start feels like an overkill. This could be done on demand.
            LoadTimeseries();
        }

        private void LoadCountryNames()
        {
            Dictionary<string, string> NameEnEsEquivalences;
            string CountryEnEs;
            using (var Sr = new StreamReader(Assets.Open("CountryNames_en_es.json")))
            {
                CountryEnEs = Sr.ReadToEnd();
                NameEnEsEquivalences = JsonConvert.DeserializeObject<Dictionary<string, string>>(CountryEnEs);
            }
            Const.CountryEquiv.NamesEnEs = NameEnEsEquivalences;
        }

        private void LoadCountryCodes()
        {
            Dictionary<string, string> CodeEquivalences;
            string CountryCodeJson;
            using (var Sr = new StreamReader(Assets.Open("CountryCodes.json")))
            {
                CountryCodeJson = Sr.ReadToEnd();
                CodeEquivalences = JsonConvert.DeserializeObject<Dictionary<string, string>>(CountryCodeJson);
            }
            //It may be faster to cache this in the SharedPreferences:
            //GetSharedPreferences("country_equivalences", FileCreationMode.Private).Edit().PutString("CountryCodeJson", CountryCodeJson).Commit();
            Const.CountryEquiv.NamesCodes = CodeEquivalences;
        }
        private void LoadTimeseries()
        {
            CountryTimeseriesRoot CountryTimeseries;
            string TimeseriesJson;
            using(var Sr = new StreamReader(Assets.Open("timeseries_22_03.json")))
            {
                TimeseriesJson = Sr.ReadToEnd();
                CountryTimeseries = JsonConvert.DeserializeObject<CountryTimeseriesRoot>(TimeseriesJson);
                //Flag the change ones.
                var CountryTimeseries2 = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<CountryTimeseriesDay>>>(TimeseriesJson);
                var DynamicTimeseries = JsonConvert.DeserializeObject<dynamic>(TimeseriesJson);
                var SomeResult = DynamicTimeseries["Thailand"];
            }
            //Const.Timeseries = CountryTimeseries;
        }
    }
}