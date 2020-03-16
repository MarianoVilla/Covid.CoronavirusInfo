using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Covid.Droid.Fragments;
using Covid.Droid.Model;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        ApiConsumer Api;
        GlobalDataFragment GlobalFragment;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        CovidReportAdapter CountriesAdapter;
        TextView txtTitle;
        LinearLayout recyclerLayout;
        CountryDetailsFragment DetailsFragment;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            InitControls();
            InitApiConsumer();
            await Api.GetGlobal();
            await Api.GetDataByCountries();
        }

        private void InitControls()
        {
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            navigation.SetOnNavigationItemSelectedListener(this);
            GlobalFragment = (GlobalDataFragment)SupportFragmentManager.FindFragmentById(Resource.Id.globalDataFragment);
            DetailsFragment = (CountryDetailsFragment)SupportFragmentManager.FindFragmentById(Resource.Id.detailsFragment);
            SupportFragmentManager.BeginTransaction().Hide(DetailsFragment).Commit();
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            recyclerLayout = FindViewById<LinearLayout>(Resource.Id.recyclerLayout);
        }

        private void CountriesAdapter_ItemClick(object sender, CovidCountryReport Report)
        {
            DetailsFragment.Update(Report);
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Show(DetailsFragment).Commit();
        }

        private void InitApiConsumer()
        {
            Api = new ApiConsumer();
            var ApiListener = new RestCompletionListener(ApiListener_Success, ApiListener_Failure);
            Api.AddOnSuccessListener(ApiListener);
            Api.AddOnFailureListener(ApiListener);
        }

        //@ToDo: find a better way to do this!
        int CachedLevel;
        private void ApiListener_Success(object sender, object CovidResult)
        {
            if (CachedLevel >= 2)
                return; 
            if (CovidResult is CovidReport)
            {
                GlobalFragment.Update(CovidResult as CovidReport);
                CachedLevel++;
            }
            else
            {
                var AsList = ((IEnumerable<CovidCountryReport>)CovidResult).ToList();
                CountriesAdapter = new CovidReportAdapter(AsList);
                CountriesAdapter.ItemClick += CountriesAdapter_ItemClick;
                mRecyclerView.SetAdapter(CountriesAdapter);
                CachedLevel++;
            }
        }

        private void ApiListener_Failure(object sender, Exception e)
        {
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_global:
                    HideByCountries();
                    ShowGlobal();
                    return true;
                case Resource.Id.navigation_bycountries:
                    HideGlobal();
                    ShowByCountries();
                    return true;
                case Resource.Id.navigation_about:
                    return true;
            }
            return false;
        }
        void HideGlobal()
        {
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Hide(GlobalFragment).Commit();
        }
        void ShowGlobal()
        {
            txtTitle.Text = "Global";
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Show(GlobalFragment).Commit();
        }
        void HideByCountries()
        {
            recyclerLayout.Visibility = ViewStates.Invisible;
        }
        void ShowByCountries()
        {
            txtTitle.Text = "Países";
            recyclerLayout.Visibility = ViewStates.Visible;
        }

    }
}

