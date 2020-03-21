using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Covid.Lib;
using Covid.Model;
using Newtonsoft.Json;
using NetDebug = System.Diagnostics.Debug;

namespace Covid.Droid.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme.Splash", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {
        ApiConsumer Api;
        ImageView imgSplash;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Drawable.splash);
            imgSplash = FindViewById<ImageView>(Resource.Id.imgSplash);
            RandomAdvice();
        }
        void RandomAdvice()
        {
            int[] Splashes = { 
                Resource.Drawable.avoid_crowds,
                Resource.Drawable.wash_hands, 
                Resource.Drawable.wash_hand_soap, 
                Resource.Drawable.caugh, 
                Resource.Drawable.distance,
                Resource.Drawable.wash_hands_water,
                Resource.Drawable.soap_alcohol};
            Random Rnd = new Random();
            int RandomSplashId = Splashes[Rnd.Next(Splashes.Count())];
            imgSplash.SetImageResource(RandomSplashId);
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (CacheIsRecent())
            {
                LoadDataFromCache();
                GoToMain();
                return;
            }
            Task.Run(() => InitApiConsumer());
        }

        private bool CacheIsRecent()
        {
            var GlobalStamp = GetSharedPreferences("cache", FileCreationMode.Private).GetString("global_timestamp", null);
            var ByCountriesStamp = GetSharedPreferences("cache", FileCreationMode.Private).GetString("by_countries_timestamp", null);
            if (GlobalStamp is null || ByCountriesStamp is null)
                return false;
            var ParsedGlobal = DateTime.Parse(GlobalStamp);
            var ParsedByCountries = DateTime.Parse(ByCountriesStamp);
            return LessThanNDaysAgo(ParsedGlobal) && LessThanNDaysAgo(ParsedByCountries);

        }
        bool LessThanNDaysAgo(DateTime Date, int n = 1) => (DateTime.Now - Date).TotalDays < n;

        private async Task InitApiConsumer()
        {
            InitHttpClient();
            Api = new ApiConsumer();
            var ApiListener = new RestCompletionListener(ApiListener_Success, ApiListener_Failure);
            Api.AddOnSuccessListener(ApiListener);
            Api.AddOnFailureListener(ApiListener);
            await Api.GetGlobal();
            await Api.GetDataByCountries();
        }
        private static void InitHttpClient()
        {
            var Cookies = new CookieContainer();
            var Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Const.GlobalHttpClient = new HttpClient(Handler) { BaseAddress = Const.Endpoints.FirstOrDefault(x => x.IsWorking("All")) };
            Const.GlobalHttpClient.DefaultRequestHeaders.Add("accept", "*/*");
        }
        CovidReport GlobalReport;
        List<CovidCountryReport> CountriesReport;
        private void ApiListener_Success(object sender, object CovidResult)
        {
            if (CovidResult is CovidReport)
            {
                GlobalReport = (CovidReport)CovidResult;
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("global", GlobalReport.ToJson()).Commit();
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("global_timestamp", DateTime.Now.ToString()).Commit();
            }
            else
            {
                CountriesReport = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("by_countries", CountriesReport.ToJson()).Commit();
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("by_countries_timestamp", DateTime.Now.ToString()).Commit();
            }
            if (AllDone())
            {
                GoToMain();
            }
        }
        bool AllDone() => GlobalReport != null && CountriesReport != null;

        private void GoToMain()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra(nameof(GlobalReport), JsonConvert.SerializeObject(GlobalReport));
            intent.PutExtra(nameof(CountriesReport), JsonConvert.SerializeObject(CountriesReport));
            StartActivity(intent);
            Finish();
        }
        private void ApiListener_Failure(object sender, Exception e)
        {
            LoadDataFromCache();
            Toast.MakeText(this, "Hubo un error al actualizar. Usamos la info de la última actualización.", ToastLength.Long).Show();
            GoToMain();
        }
        void LoadDataFromCache()
        {
            try
            {
                GlobalReport = GetSharedPreferences("cache", FileCreationMode.Private).GetString("global", null).FromJson<CovidReport>();
                CountriesReport = GetSharedPreferences("cache", FileCreationMode.Private).GetString("by_countries", null).FromJson<List<CovidCountryReport>>();
            }
            catch (Exception ex)
            {
                NetDebug.WriteLine(ex);
                Toast.MakeText(this, "Imposible iniciar, vuelva a intentar", ToastLength.Short);
                Finish();
            }
        }
    }
}