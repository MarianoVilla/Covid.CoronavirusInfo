using System;
using System.Collections.Generic;
using System.Linq;
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
            Task.Run(() => InitApiConsumer());
        }
        private async Task InitApiConsumer()
        {
            Api = new ApiConsumer();
            var ApiListener = new RestCompletionListener(ApiListener_Success, ApiListener_Failure);
            Api.AddOnSuccessListener(ApiListener);
            Api.AddOnFailureListener(ApiListener);
            await Api.GetGlobal();
            await Api.GetDataByCountries();
        }
        CovidReport GlobalReport;
        List<CovidCountryReport> CountriesReport;
        private void ApiListener_Success(object sender, object CovidResult)
        {
            if (CovidResult is CovidReport)
            {
                GlobalReport = (CovidReport)CovidResult;
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("global", GlobalReport.ToJson()).Commit();
            }
            else
            {
                CountriesReport = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
                GetSharedPreferences("cache", FileCreationMode.Private).Edit().PutString("by_countries", CountriesReport.ToJson()).Commit();
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
            GetFromCache();
            Toast.MakeText(this, "Hubo un error al actualizar. Usamos la info de la última actualización.", ToastLength.Long).Show();
            GoToMain();
        }
        void GetFromCache()
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