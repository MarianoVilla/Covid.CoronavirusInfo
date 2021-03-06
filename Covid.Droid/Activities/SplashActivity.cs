﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Covid.Droid.Helpers;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Covid.Droid.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/ic_launcher_foreground", Theme = "@style/AppTheme.Splash", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
            this.imgSplash = FindViewById<ImageView>(Resource.Id.imgSplash);
            try
            {
                RandomAdvice();
                GetData();
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
            this.CountriesReport = new List<CovidCountryReport>();
            this.GlobalReport = new CovidReport();
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
                Resource.Drawable.distance_alt,
                Resource.Drawable.wash_hands_water_alt,
                Resource.Drawable.wash_hands_water,
                Resource.Drawable.soap_alcohol,
                Resource.Drawable.soap};
            var Rnd = new Random();
            int RandomSplashId = Splashes[Rnd.Next(Splashes.Count())];
            this.imgSplash.SetImageResource(RandomSplashId);
        }
        private bool CacheIsRecent()
        {
            return false;
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            DateTime? GlobalStamp = SharedPreferencesHandler.GetGlobalReportStamp(this);
            DateTime? ByCountriesStamp = SharedPreferencesHandler.GetCountriesReportStamp(this);
            DateTime? TimeseriesStamp = SharedPreferencesHandler.GetTimeseriesStamp(this);
            if (GlobalStamp is null || ByCountriesStamp is null || TimeseriesStamp is null)
                return false;
            return LessThanNDaysAgo((DateTime)GlobalStamp) && LessThanNDaysAgo((DateTime)ByCountriesStamp) && LessThanNDaysAgo((DateTime)TimeseriesStamp);
        }
        bool LessThanNDaysAgo(DateTime Date, int n = 1) => (DateTime.Now - Date).TotalDays < n;

        private async Task GetFromApi()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            InitHttpClient();
            InitApiConsumer();

            this.Api.GetDataByCountriesAsync();
            this.Api.GetGlobalAsync();
            this.Api.GetTimeseriesAsync();
        }
        void InitApiConsumer()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.Api = new ApiConsumer();
            var ApiListener = new RestCompletionListener(ApiListener_Success, ApiListener_Failure);
            this.Api.AddOnSuccessListener(ApiListener);
            this.Api.AddOnFailureListener(ApiListener);
            this.Api.GlobalEndpoint = Const.GlobalEndpoints.FirstOrDefault(x => x.IsWorking());
            this.Api.ByCountriesEndpoint = Const.ByCountriesEndpoints.FirstOrDefault(x => x.IsWorking());
            this.Api.TimeseriesEndpoint = Const.TimeseriesEndpoints.FirstOrDefault();
        }
        void InitHttpClient()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            var Cookies = new CookieContainer();
            var Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Const.GlobalHttpClient = new HttpClient(Handler);
            Const.GlobalHttpClient.DefaultRequestHeaders.Add("accept", "*/*");
        }
        private void ApiListener_Success(object sender, object CovidResult)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (CovidResult is CovidReport)
            {
                this.GlobalReport = (CovidReport)CovidResult;
                SharedPreferencesHandler.SaveGlobalReport(this, this.GlobalReport);
            }
            else if (CovidResult is IEnumerable<CovidCountryReport>)
            {
                this.CountriesReport = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
                if (this.HasCachedReport)
                {
                    this.CountriesReport.LoadFavouritesFromPreferences(this);
                }
                SharedPreferencesHandler.SaveCountriesReport(this, this.CountriesReport);
            }
            else if (CovidResult is CountryTimeseriesContainer)
            {
                //@ToDo cache the timeseries. It's too big for SharedPreferences. Probably SQLite.
                //SharedPreferencesHandler.SaveCountryTimeseriesContainer(this, (CountryTimeseriesContainer)CovidResult);
            }
            if (this.AllDone)
            {
                GoToMain();
            }
        }

        private bool HasCachedReport => SharedPreferencesHandler.GetCountriesReportStamp(this) != null;
        bool AllDone => this.GlobalReport != null && this.CountriesReport != null;

        private void GoToMain()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.CountriesReport = this.CountriesReport.OrderByDescending(x => x.IsFavourite).ToList();
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra(nameof(this.GlobalReport), this.GlobalReport.ToJson());
            intent.PutExtra(nameof(this.CountriesReport), this.CountriesReport.ToJson());
            StartActivity(intent);
            Finish();
        }
        private void ApiListener_Failure(object sender, Exception e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            LoadDataFromCache();
            Toast.MakeText(this, Resources.GetString(Resource.String.update_error), ToastLength.Long).Show();
            GoToMain();
        }
        void LoadDataFromCache()
        {
            try
            {
                DebugHelper.Method(MethodBase.GetCurrentMethod());
                this.GlobalReport = SharedPreferencesHandler.GetCovidReport(this);
                this.CountriesReport = SharedPreferencesHandler.GetCountriesReport(this);
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                Toast.MakeText(this, Resources.GetString(Resource.String.impossible_to_start), ToastLength.Short);
                Finish();
            }
        }
    }
}