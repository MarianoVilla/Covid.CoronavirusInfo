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
            var Cookies = new CookieContainer();
            var Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Const.GlobalHttpClient = new HttpClient(Handler) { BaseAddress = Const.Endpoints.FirstOrDefault(x => x.IsWorking("All")) };
            Const.GlobalHttpClient.DefaultRequestHeaders.Add("accept", "*/*");
            LoadCountryEquivalences();
        }

        void LoadCountryEquivalences()
        {
            Const.CountryEquiv = new CountryEquivalences();
            LoadCountryCodes();
            LoadCountryNames();
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
    }
}