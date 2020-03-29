using Android.App;
using Android.Runtime;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.IO;

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

        private void Init() => LoadCountryEquivalences();

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
            using (var Sr = new StreamReader(this.Assets.Open("CountryNames_en_es.json")))
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
            using (var Sr = new StreamReader(this.Assets.Open("CountryCodes.json")))
            {
                CountryCodeJson = Sr.ReadToEnd();
                CodeEquivalences = CountryCodeJson.FromJson<Dictionary<string, string>>();
            }
            Const.CountryEquiv.NamesCodes = CodeEquivalences;
        }
    }
}