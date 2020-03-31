using Android.App;
using Android.Runtime;
using Android.Support.V4.OS;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Covid.Droid
{
    [Application]
    public class App : Application
    {
        ///
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
        //
        private void LoadCountryNames()
        {
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en")
                return;
            string LocalNamesJson = SwitchLocalNamesJson();
            Dictionary<string, string> NameEnEsEquivalences;
            string CountryEnEs;
            using (var Sr = new StreamReader(this.Assets.Open(LocalNamesJson)))
            {
                CountryEnEs = Sr.ReadToEnd();
                NameEnEsEquivalences = CountryEnEs.FromJson<Dictionary<string, string>>();
            }
            Const.CountryEquiv.NamesEnToLocale = NameEnEsEquivalences;
        }
        string SwitchLocalNamesJson()
        {
            switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            {
                case "es": return "CountryNames_en_es.json";
                default: return string.Empty;
            }
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