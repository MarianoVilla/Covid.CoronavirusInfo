using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Tomergoldst.Tooltips;
using Covid.Droid.Fragments;
using Covid.Droid.Helpers;
using Covid.Droid.Model;
using Covid.Lib;
using Covid.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;

namespace Covid.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener, ITipListener
    {
        GlobalDataFragment GlobalFragment;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        CovidReportAdapter CountriesAdapter;
        TextView txtTitle;
        LinearLayout recyclerLayout;
        CountryDetailsFragment DetailsFragment;
        InfoFragment InfoFrag;
        //ProgressBar progressBar;
        //AnimationHandler Animator;
        CovidReport GlobalReport;
        List<CovidCountryReport> CountriesReport;
        ViewGroup RootView;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            InitControls();
            GetBoundleData();
            LoadBoundleData();
        }
        void GetBoundleData()
        {
            GlobalReport = JsonConvert.DeserializeObject<CovidReport>(Intent.GetStringExtra(nameof(GlobalReport)));
            CountriesReport = JsonConvert.DeserializeObject<List<CovidCountryReport>>(Intent.GetStringExtra(nameof(CountriesReport)));
        }

        private void LoadBoundleData()
        {
            GlobalFragment.Update(GlobalReport);
            CountriesAdapter = new CovidReportAdapter(CountriesReport);

            CountriesAdapter.ItemClick += CountriesAdapter_ItemClick;
            mRecyclerView.SetAdapter(CountriesAdapter);
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
            InfoFrag = (InfoFragment)SupportFragmentManager.FindFragmentById(Resource.Id.infoFragment);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            HideInfo();
            recyclerLayout = FindViewById<LinearLayout>(Resource.Id.recyclerLayout);
            //progressBar = FindViewById<ProgressBar>(Resource.Id.loadingProgress);
            //Animator = new AnimationHandler(progressBar);
            DetailsFragment.OnDestroyCallback += DetailsFragment_OnDestroyCallback;
            DetailsFragment.OnItemClickCallback += DetailsFragment_OnItemClickCallback;

            this.RootView = FindViewById<ViewGroup>(Resource.Id.container);
            ToolTips = new ToolTipsManager(this);
        }
        ToolTipsManager ToolTips;
        private void DetailsFragment_OnItemClickCallback(object sender, EventArgs e)
        {
            var Args = e as DetailsItemClickEventArgs;
            if(Args is null)
                return;

            var builder = new ToolTip.Builder(this, Args.Anchor, RootView, Args.ToolTipText, Args.Position ?? ToolTip.PositionAbove);
            builder.SetAlign(ToolTip.AlignCenter);
            builder.SetBackgroundColor(Resource.Color.material_grey_50);
            ToolTips.Show(builder.Build());
            Task.Delay(2000).ContinueWith((task) => RunOnUiThread(() => ToolTips.FindAndDismiss(Args.Anchor)));
        }

        private void DetailsFragment_OnDestroyCallback(object sender, EventArgs e)
        {
            HideDetails();
            ShowByCountries();
        }

        private void CountriesAdapter_ItemClick(object sender, CovidCountryReport Report)
        {
            DetailsFragment.Update(Report);
            HideByCountries();
            HideMainTitle();
            ShowDetails();
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
                    SwitchToGlobal();
                    return true;
                case Resource.Id.navigation_bycountries:
                    SwitchToCountries();
                    return true;
                case Resource.Id.navigation_info:
                    SwitchToInfo();
                    return true;
                //case Resource.Id.navigation_about:
                //    SwitchToAbout();
                //    return true;
            }
            return false;
        }

        private void SwitchToAbout()
        {
            throw new NotImplementedException();
        }
        void SwitchToInfo()
        {
            HideGlobal();
            HideByCountries();
            HideDetails();
            ShowInfo();
        }
        void HideInfo()
        {
            if (InfoFrag.IsHidden)
                return;
            ShowMainTitle();
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Hide(InfoFrag).Commit();
        }
        void ShowInfo()
        {
            if (InfoFrag.IsVisible)
                return;
            HideMainTitle();
            SetTitleDrawable(Resource.Drawable.exclamation);
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Show(InfoFrag).Commit();
        }
        void HideMainTitle() => txtTitle.Visibility = ViewStates.Gone;
        void ShowMainTitle() => txtTitle.Visibility = ViewStates.Visible;
        void SwitchToCountries()
        {
            HideInfo();
            HideGlobal();
            ShowByCountries();
        }
        void HideGlobal()
        {
            if (GlobalFragment.IsHidden)
                return;
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Hide(GlobalFragment).Commit();
        }
        void SwitchToGlobal()
        {
            HideInfo();
            HideByCountries();
            ShowGlobal();
        }
        void ShowGlobal()
        {
            if (GlobalFragment.IsVisible)
                return;
            txtTitle.Text = "Global";
            SetTitleDrawable(Resource.Drawable.world_earth);
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Show(GlobalFragment).Commit();
        }
        void HideByCountries() => recyclerLayout.Visibility = ViewStates.Invisible;
        void ShowByCountries()
        {
            txtTitle.Text = "Países";
            SetTitleDrawable(Resource.Drawable.country_icon);
            recyclerLayout.Visibility = ViewStates.Visible;
        }
        void SetTitleDrawable(int TheDrawableId)
        {
            var draw = ApplicationContext.GetDrawable(TheDrawableId);
            txtTitle.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
        private void ShowDetails()
        {
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Show(DetailsFragment).Commit();
        }
        private void HideDetails()
        {
            if (DetailsFragment.IsHidden)
                return;
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).Hide(DetailsFragment).Commit();
        }

        public void OnTipDismissed(View p0, int p1, bool p2)
        {
            //@ToDo implement.
            //throw new NotImplementedException();
        }
    }
}

