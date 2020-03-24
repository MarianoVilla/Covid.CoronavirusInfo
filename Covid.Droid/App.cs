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
                NameEnEsEquivalences = CountryEnEs.FromJson<Dictionary<string, string>>();
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
                CodeEquivalences = CountryCodeJson.FromJson<Dictionary<string, string>>();
            }
            //It may be faster to cache this in the SharedPreferences:
            //GetSharedPreferences("country_equivalences", FileCreationMode.Private).Edit().PutString("CountryCodeJson", CountryCodeJson).Commit();
            Const.CountryEquiv.NamesCodes = CodeEquivalences;
        }
        private void LoadTimeseries()
        {
            CountryTimeseriesContainer CountryTimeseriesContainer;
            string TimeseriesJson;
            using(var Sr = new StreamReader(Assets.Open("timeseries.json")))
            {
                TimeseriesJson = Sr.ReadToEnd();
                var Timeseries = TimeseriesJson.FromJson<Dictionary<string, List<CountryTimeseriesDay>>>();;
                Const.TimeseriesContainer.Timeseries = Timeseries;
            }
        }
    }
}