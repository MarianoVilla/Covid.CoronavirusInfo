using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid.Lib;
using Covid.Model;
using Newtonsoft.Json;

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
            //@ToDo: fix stretching.
            int[] Splashes = { Resource.Drawable.avoid_crowds, Resource.Drawable.wash_hands, Resource.Drawable.wash_hand_soap };
            Random Rnd = new Random();
            int RandomSplashId = Splashes[Rnd.Next(Splashes.Count())];
            imgSplash.SetImageResource(RandomSplashId);
        }
        protected override async void OnResume()
        {
            base.OnResume();
            InitApiConsumer();
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
            }
            else
            {
                CountriesReport = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
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
            //@ToDo: do something with failure. (Such as using a backup API!)
        }
    }
}