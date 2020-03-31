using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Tomergoldst.Tooltips;
using Covid.Droid.Fragments;
using Covid.Droid.Helpers;
using Covid.Droid.Model;
using Covid.Lib;
using Covid.Model;
using Firebase.Iid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Android.Support.V7.Widget.RecyclerView;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;

namespace Covid.Droid.Activities
{

    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher_foreground", Theme = "@style/AppTheme", MainLauncher = false)]
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
        CovidReport GlobalReport;
        List<CovidCountryReport> CountriesReport;
        ViewGroup RootView;
        AutoCompleteTextView txtSearchCountry;
        FloatingActionButton btnBackToTop;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            Log.Debug(this.Tag, "google app id: " + GetString(Resource.String.google_app_id));
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            GetBoundleData();
            InitControls();
            InitFirebase();
            LoadBoundleData();
        }
        //@ToDo: push stuff.
        string Tag = "CovidToken";
        FirebaseHandler FirebaseHandler;
        void InitFirebase()
        {
            this.FirebaseHandler = new FirebaseHandler(this);
            if (!this.FirebaseHandler.IsPlayServicesAvailable(out string ErrorString))
                return;
            this.FirebaseHandler.CreateNotificationChannel();
            string Token = FirebaseInstanceId.Instance.Token;
            Log.Debug(this.Tag, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
        }

        protected override void OnPause()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            base.OnPause();
            SharedPreferencesHandler.SaveCountriesReport(this, this.CountriesReport);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            CovidCountryReport Report = data.GetStringExtra("Report").FromJson<CovidCountryReport>();
            CountriesAdapter_ItemClick(this, Report);
            base.OnActivityResult(requestCode, resultCode, data);

        }
        void GetBoundleData()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.GlobalReport = JsonConvert.DeserializeObject<CovidReport>(this.Intent.GetStringExtra(nameof(this.GlobalReport)));
            this.CountriesReport = JsonConvert.DeserializeObject<List<CovidCountryReport>>(this.Intent.GetStringExtra(nameof(this.CountriesReport)));
        }

        private void LoadBoundleData()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.GlobalFragment.Update(this.GlobalReport);
            this.CountriesAdapter = new CovidReportAdapter(this.CountriesReport);

            this.CountriesAdapter.ItemClick += CountriesAdapter_ItemClick;
            this.mRecyclerView.SetAdapter(this.CountriesAdapter);
        }
        private void InitControls()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.RootView = FindViewById<ViewGroup>(Resource.Id.container);
            this.txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            this.GlobalFragment = (GlobalDataFragment)this.SupportFragmentManager.FindFragmentById(Resource.Id.globalDataFragment);

            InitNavigation();
            InitRecyclerView();
            InitInfoFragment();
            InitDetailsFragment();
            InitTxtSearchCountries();
            InitFab();

            this.ToolTips = new ToolTipsManager(this);
        }
        void InitFab()
        {
            this.btnBackToTop = FindViewById<FloatingActionButton>(Resource.Id.fabBackToTop);
            this.btnBackToTop.Click += (s, e) => this.mRecyclerView.SmoothScrollToPosition(0);
        }

        private void InitInfoFragment()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.InfoFrag = (InfoFragment)this.SupportFragmentManager.FindFragmentById(Resource.Id.infoFragment);
            HideInfo();
        }

        private void InitRecyclerView()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.mLayoutManager = new LinearLayoutManager(this);
            this.mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewMain);
            this.mRecyclerView.SetLayoutManager(this.mLayoutManager);
            this.recyclerLayout = FindViewById<LinearLayout>(Resource.Id.recyclerLayout);
        }

        private void InitDetailsFragment()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.DetailsFragment = (CountryDetailsFragment)this.SupportFragmentManager.FindFragmentById(Resource.Id.detailsFragment);
            this.SupportFragmentManager.BeginTransaction().Hide(this.DetailsFragment).Commit();
            this.DetailsFragment.OnDestroyCallback += DetailsFragment_OnDestroyCallback;
            this.DetailsFragment.OnItemClickCallback += DetailsFragment_OnItemClickCallback;
        }

        private void InitNavigation()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }

        #region txtSearchCountries.
        void InitTxtSearchCountries()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.txtSearchCountry = FindViewById<AutoCompleteTextView>(Resource.Id.txtSearchCountry);
            this.txtSearchCountry.Click += TxtSearch_Click;
            this.txtSearchCountry.EditorAction += TxtSearchCountry_EditorAction;
            this.txtSearchCountry.Touch += TxtSearch_Touch;
            this.txtSearchCountry.TextChanged += TxtSearch_TextChanged;
            this.txtSearchCountry.Adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, this.CountriesReport.Select(x => x.RegionalFriendlyName ?? x.Country).ToArray());
        }

        private void TxtSearchCountry_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (string.IsNullOrWhiteSpace(this.txtSearchCountry.Text))
                return;
            this.CountriesAdapter.FilterByName(this.txtSearchCountry.Text);
        }

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.txtSearchCountry.Text.Length > 0)
            {
                this.txtSearchCountry.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_cross_81577_32, 0);
            }
            else
            {
                this.txtSearchCountry.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            }
            this.txtSearchCountry.ShowDropDown();
        }
        private void TxtSearch_Touch(object sender, View.TouchEventArgs e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            Drawable rightDrawable = this.txtSearchCountry.GetCompoundDrawables()[2];
            if (rightDrawable == null || e.Event.Action != MotionEventActions.Up)
            {
                e.Handled = false;
                return;
            }
            if (e.Event.GetX() >= this.txtSearchCountry.Width - this.txtSearchCountry.TotalPaddingRight)
            {
                this.txtSearchCountry.Text = string.Empty;
                this.CountriesAdapter.Unfilter();
                e.Handled = true;
            }
            (sender as AutoCompleteTextView)?.OnTouchEvent(e.Event);
        }
        private void TxtSearch_Click(object sender, EventArgs e) => this.txtSearchCountry.ShowDropDown();
        #endregion

        ToolTipsManager ToolTips;
        private void DetailsFragment_OnItemClickCallback(object sender, EventArgs e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            var Args = e as DetailsItemClickEventArgs;
            if (Args is null)
                return;

            var builder = new ToolTip.Builder(this, Args.Anchor, this.RootView, Args.ToolTipText, Args.Position ?? ToolTip.PositionAbove);
            builder.SetAlign(ToolTip.AlignCenter);
            builder.SetBackgroundColor(Resource.Color.material_grey_50);
            this.ToolTips.Show(builder.Build());
            Task.Delay(2000).ContinueWith((task) => RunOnUiThread(() => this.ToolTips.FindAndDismiss(Args.Anchor)));
        }

        private void DetailsFragment_OnDestroyCallback(object sender, EventArgs e)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            HideDetails();
            this.ToolTips.Clear();
            ShowByCountries();
        }

        private void CountriesAdapter_ItemClick(object sender, CovidCountryReport Report)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.DetailsFragment.Update(Report);
            HideByCountries();
            HideMainTitle();
            ShowDetails();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
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
        void SwitchToGlobal()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            HideInfo();
            HideByCountries();
            ShowGlobal();
        }
        void SwitchToCountries()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            HideInfo();
            HideGlobal();
            HideDetails();
            ShowByCountries();
        }
        void SwitchToInfo()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            HideGlobal();
            HideByCountries();
            HideDetails();
            ShowInfo();
        }
        private void SwitchToAbout() => throw new NotImplementedException();
        void HideInfo()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.InfoFrag.IsHidden)
                return;
            Android.Support.V4.App.FragmentManager fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(this.InfoFrag).Commit();
            ShowMainTitle();
        }
        void ShowInfo()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.InfoFrag.IsVisible)
                return;
            HideMainTitle();
            SetTitleDrawable(Resource.Drawable.exclamation);
            Android.Support.V4.App.FragmentManager fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(this.InfoFrag).Commit();
        }
        void HideMainTitle()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.txtTitle.Animate().SetDuration(200).Alpha(0);
            this.txtTitle.Visibility = ViewStates.Gone;
        }
        void ShowMainTitle(string Title = null)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.txtTitle.Visibility = ViewStates.Visible;
            this.txtTitle.Animate().SetDuration(200).Alpha(1);
            this.txtTitle.Text = Title ?? this.txtTitle.Text;
        }
        void HideGlobal()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.GlobalFragment.IsHidden)
                return;
            this.SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(this.GlobalFragment).Commit();
        }
        void ShowGlobal()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.GlobalFragment.IsVisible)
                return;
            ShowMainTitle("Global");
            SetTitleDrawable(Resource.Drawable.world_earth);
            this.SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(this.GlobalFragment).Commit();
        }
        void HideByCountries()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.recyclerLayout.Animate().SetDuration(200).Alpha(0);
            this.recyclerLayout.Visibility = ViewStates.Invisible;
            this.btnBackToTop.Visibility = ViewStates.Invisible;
            HideDetails();
        }
        void ShowByCountries()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            ShowMainTitle("Países");
            SetTitleDrawable(Resource.Drawable.country_icon);
            this.recyclerLayout.Animate().SetDuration(300).Alpha(1);
            this.recyclerLayout.Visibility = ViewStates.Visible;
            this.btnBackToTop.Visibility = ViewStates.Visible;
        }
        void SetTitleDrawable(int TheDrawableId)
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            Drawable draw = this.ApplicationContext.GetDrawable(TheDrawableId);
            this.txtTitle.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
        private void ShowDetails()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            this.SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(this.DetailsFragment).CommitAllowingStateLoss();
        }
        private void HideDetails()
        {
            DebugHelper.Method(MethodBase.GetCurrentMethod());
            if (this.DetailsFragment.IsHidden)
                return;
            this.SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(this.DetailsFragment).CommitAllowingStateLoss();
        }
        public void OnTipDismissed(View p0, int p1, bool p2) => DebugHelper.Method(MethodBase.GetCurrentMethod());//@ToDo implement.
    }
}

