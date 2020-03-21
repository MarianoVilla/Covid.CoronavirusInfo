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
        AutoCompleteTextView txtSearchCountry;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            GetBoundleData();
            InitControls();
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
            RootView = FindViewById<ViewGroup>(Resource.Id.container);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            GlobalFragment = (GlobalDataFragment)SupportFragmentManager.FindFragmentById(Resource.Id.globalDataFragment);

            InitNavigation();
            InitRecyclerView();
            InitInfoFragment();
            InitDetailsFragment();
            InitTxtSearchCountries();

            ToolTips = new ToolTipsManager(this);
        }

        private void InitInfoFragment()
        {
            InfoFrag = (InfoFragment)SupportFragmentManager.FindFragmentById(Resource.Id.infoFragment);
            HideInfo();
        }

        private void InitRecyclerView()
        {
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewMain);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            recyclerLayout = FindViewById<LinearLayout>(Resource.Id.recyclerLayout);
        }

        private void InitDetailsFragment()
        {
            DetailsFragment = (CountryDetailsFragment)SupportFragmentManager.FindFragmentById(Resource.Id.detailsFragment);
            SupportFragmentManager.BeginTransaction().Hide(DetailsFragment).Commit();
            DetailsFragment.OnDestroyCallback += DetailsFragment_OnDestroyCallback;
            DetailsFragment.OnItemClickCallback += DetailsFragment_OnItemClickCallback;
        }

        private void InitNavigation()
        {
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }

        #region txtSearchCountries.
        void InitTxtSearchCountries()
        {
            txtSearchCountry = FindViewById<AutoCompleteTextView>(Resource.Id.txtSearchCountry);
            txtSearchCountry.Click += TxtSearch_Click;
            txtSearchCountry.EditorAction += TxtSearchCountry_EditorAction;
            txtSearchCountry.Touch += TxtSearch_Touch;
            txtSearchCountry.TextChanged += TxtSearch_TextChanged;
            txtSearchCountry.Adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, CountriesReport.Select(x => x.Country).ToArray());
        }

        private void TxtSearchCountry_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchCountry.Text))
                return;
            CountriesAdapter.FilterByName(txtSearchCountry.Text);
        }

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (txtSearchCountry.Text.Length > 0)
            {
                txtSearchCountry.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_cross_81577_32, 0);
            }
            else
            {
                txtSearchCountry.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            }
            txtSearchCountry.ShowDropDown();
        }
        private void TxtSearch_Touch(object sender, View.TouchEventArgs e)
        {
            var rightDrawable = txtSearchCountry.GetCompoundDrawables()[2];
            if (rightDrawable == null || e.Event.Action != MotionEventActions.Up)
            {
                e.Handled = false;
                return;
            }
            if (e.Event.GetX() >= txtSearchCountry.Width - txtSearchCountry.TotalPaddingRight)
            {
                txtSearchCountry.Text = string.Empty;
                CountriesAdapter.Unfilter();
                e.Handled = true;
            }
            (sender as AutoCompleteTextView)?.OnTouchEvent(e.Event);
        }
        private void TxtSearch_Click(object sender, EventArgs e) => txtSearchCountry.ShowDropDown();
        #endregion

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
            ToolTips.Clear();
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
        void SwitchToGlobal()
        {
            HideInfo();
            HideByCountries();
            ShowGlobal();
        }
        void SwitchToCountries()
        {
            HideInfo();
            HideGlobal();
            ShowByCountries();
        }
        void SwitchToInfo()
        {
            HideGlobal();
            HideByCountries();
            HideDetails();
            ShowInfo();
        }



        private void SwitchToAbout()
        {
            throw new NotImplementedException();
        }
        void HideInfo()
        {
            if (InfoFrag.IsHidden)
                return;
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(InfoFrag).Commit();
            ShowMainTitle();
        }
        void ShowInfo()
        {
            if (InfoFrag.IsVisible)
                return;
            HideMainTitle();
            SetTitleDrawable(Resource.Drawable.exclamation);
            var fm = this.SupportFragmentManager;
            fm.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(InfoFrag).Commit();
        }
        void HideMainTitle() 
        { 
            txtTitle.Animate().SetDuration(200).Alpha(0);
            txtTitle.Visibility = ViewStates.Gone;
        }
        void ShowMainTitle(string Title = null) 
        {
            txtTitle.Visibility = ViewStates.Visible;
            txtTitle.Animate().SetDuration(200).Alpha(1);
            txtTitle.Text = Title ?? txtTitle.Text;
        }
 
        void HideGlobal()
        {
            if (GlobalFragment.IsHidden)
                return;
            SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(GlobalFragment).Commit();
        }
        void ShowGlobal()
        {
            if (GlobalFragment.IsVisible)
                return;
            ShowMainTitle("Global");
            SetTitleDrawable(Resource.Drawable.world_earth);
            SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(GlobalFragment).Commit();
        }
        void HideByCountries() 
        {
            recyclerLayout.Animate().SetDuration(200).Alpha(0);
            recyclerLayout.Visibility = ViewStates.Invisible;
            HideDetails();
        }
        void ShowByCountries()
        {
            ShowMainTitle("Países");
            SetTitleDrawable(Resource.Drawable.country_icon);
            recyclerLayout.Animate().SetDuration(300).Alpha(1);
            recyclerLayout.Visibility = ViewStates.Visible;
        }
        void SetTitleDrawable(int TheDrawableId)
        {
            var draw = ApplicationContext.GetDrawable(TheDrawableId);
            txtTitle.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
        private void ShowDetails()
        {
            SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Show(DetailsFragment).Commit();
        }
        private void HideDetails()
        {
            if (DetailsFragment.IsHidden)
                return;
            SupportFragmentManager.BeginTransaction().SetCustomAnimations(Resource.Animation.anim_fade_in, Resource.Animation.anim_fade_out).Hide(DetailsFragment).Commit();
        }

        public void OnTipDismissed(View p0, int p1, bool p2)
        {
            //@ToDo implement.
            //throw new NotImplementedException();
        }
    }
}

