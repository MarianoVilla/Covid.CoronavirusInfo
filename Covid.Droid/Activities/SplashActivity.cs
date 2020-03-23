using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Covid.Droid.Helpers;
using Covid.Lib;
using Covid.Model;
using Newtonsoft.Json;
using NetDebug = System.Diagnostics.Debug;

namespace Covid.Droid.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@mipmap/ic_launcher_foreground", Theme = "@style/AppTheme.Splash", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {
        ApiConsumer Api;
        ImageView imgSplash;
        CovidReport GlobalReport;
        List<CovidCountryReport> CountriesReport;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Drawable.splash);
            imgSplash = FindViewById<ImageView>(Resource.Id.imgSplash);
            try
            {
                RandomAdvice();
                InitHttpClient();
                Task.Run(() => GetData());
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
            }
            
        }
        void GetData()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (CacheIsRecent())
            {
                DebugHelper.Info("Loading from cache.");
                LoadDataFromCache();
                GoToMain();
                return;
            }
            DebugHelper.Info("Loading from REST.");
            GetFromApi();
        }

        #region Debug stuff.
        private void MockData()
        {
            CountriesReport = new List<CovidCountryReport>();
            GlobalReport = new CovidReport();
        }
        #endregion

        void RandomAdvice()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
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
        private bool CacheIsRecent()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            var GlobalStamp = GetSharedPreferences("cache", FileCreationMode.Private).GetString("global_timestamp", null);
            var ByCountriesStamp = GetSharedPreferences("cache", FileCreationMode.Private).GetString("by_countries_timestamp", null);
            if (GlobalStamp is null || ByCountriesStamp is null)
                return false;
            var ParsedGlobal = DateTime.Parse(GlobalStamp);
            var ParsedByCountries = DateTime.Parse(ByCountriesStamp);
            return LessThanNDaysAgo(ParsedGlobal) && LessThanNDaysAgo(ParsedByCountries);
        }
        bool LessThanNDaysAgo(DateTime Date, int n = 1) => (DateTime.Now - Date).TotalDays < n;

        private async Task GetFromApi()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            InitHttpClient();
            InitApiConsumer();

            Api.GetDataByCountriesAsync();
            Api.GetGlobalAsync();
        }
        void InitApiConsumer()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            Api = new ApiConsumer();
            var ApiListener = new RestCompletionListener(ApiListener_Success, ApiListener_Failure);
            Api.AddOnSuccessListener(ApiListener);
            Api.AddOnFailureListener(ApiListener);
        }
        void InitHttpClient()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            var Cookies = new CookieContainer();
            var Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Const.GlobalHttpClient = new HttpClient(Handler) { BaseAddress = Const.GlobalEndpoints.FirstOrDefault(x => x.IsWorking("All")) };
            Const.GlobalHttpClient.DefaultRequestHeaders.Add("accept", "*/*");
        }
        private void ApiListener_Success(object sender, object CovidResult)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (CovidResult is CovidReport)
            {
                GlobalReport = (CovidReport)CovidResult;
                SharedPreferencesHandler.SaveGlobalReport(this, GlobalReport);
            }
            else
            {
                CountriesReport = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
                if (HasCachedReport) 
                {
                    CountriesReport.LoadFavouritesFromPreferences(this);
                }
                SharedPreferencesHandler.SaveCountriesReport(this, CountriesReport);
            }
            if (AllDone)
            {
                GoToMain();
            }
        }

        private bool HasCachedReport => SharedPreferencesHandler.GetCountriesReportStamp(this) != null;
        bool AllDone => GlobalReport != null && CountriesReport != null;

        private void GoToMain()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            CountriesReport = CountriesReport.OrderByDescending(x => x.IsFavourite).ToList();
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra(nameof(GlobalReport), JsonConvert.SerializeObject(GlobalReport));
            intent.PutExtra(nameof(CountriesReport), JsonConvert.SerializeObject(CountriesReport));
            StartActivity(intent);
            Finish();
        }
        private void ApiListener_Failure(object sender, Exception e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            LoadDataFromCache();
            Toast.MakeText(this, "Hubo un error al actualizar. Usamos la info de la última actualización.", ToastLength.Long).Show();
            GoToMain();
        }
        void LoadDataFromCache()
        {
            try
            {
                DebugHelper.Method(MethodBase.GetCurrentMethod());
                GlobalReport = SharedPreferencesHandler.GetCovidReport(this);
                CountriesReport = SharedPreferencesHandler.GetCountriesReport(this);
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