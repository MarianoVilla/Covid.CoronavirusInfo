using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Tomergoldst.Tooltips;
using Covid.Droid.Activities;
using Covid.Droid.Model;
using Covid.Lib;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;

namespace Covid.Droid.Fragments
{
    public class CountryDetailsFragment : Android.Support.V4.App.DialogFragment
    {
        CovidCountryReport Report;

        View RootView;
        LinearLayout LinearRoot;
        TextView txtCountryName;
        TextView txtCountryCount;
        TextView txtTodayCases;
        TextView txtTodayDeaths;
        TextView txtCriticalCases;
        TextView txtActiveCases;
        TextView txtRecoveredCases;
        TextView txtDeathCases;
        TextView txtDeathRate;

        CardView cardCountryCount;
        CardView cardTodayCases;
        CardView cardTodayDeaths;
        CardView cardCriticalCases;
        CardView cardActiveCases;
        CardView cardRecoveredCases;
        CardView cardDeathCases;
        CardView cardDeathRate;

        ImageButton btnCloseDetails;
        ImageButton btnCountryCharts;

        public event EventHandler OnDestroyCallback;
        public event EventHandler OnItemClickCallback;
        public CountryDetailsFragment() { }

        public override void OnCreate(Bundle savedInstanceState) => base.OnCreate(savedInstanceState);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.RootView = inflater.Inflate(Resource.Layout.country_details_fragment_grid, container, false);
            InitControls();
            return this.RootView;
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data) 
        { 
            base.OnActivityResult(requestCode, resultCode, data); 
        }

        private void InitControls()
        {
            this.txtCountryName = FindViewById<TextView>(Resource.Id.txtCountryName);
            this.txtCountryCount = FindViewById<TextView>(Resource.Id.txtCountryCount);
            this.txtTodayCases = FindViewById<TextView>(Resource.Id.txtTodayCases);
            this.txtCriticalCases = FindViewById<TextView>(Resource.Id.txtCriticalCases);
            this.txtActiveCases = FindViewById<TextView>(Resource.Id.txtActiveCases);
            this.txtRecoveredCases = FindViewById<TextView>(Resource.Id.txtRecoveredCases);
            this.txtDeathCases = FindViewById<TextView>(Resource.Id.txtDeathCases);
            this.txtTodayDeaths = FindViewById<TextView>(Resource.Id.txtTodayDeaths);
            this.txtDeathRate = FindViewById<TextView>(Resource.Id.txtDeathRate);
            this.btnCloseDetails = FindViewById<ImageButton>(Resource.Id.btnCloseDetails);
            this.btnCountryCharts = FindViewById<ImageButton>(Resource.Id.btnCountryCharts);
            this.LinearRoot = FindViewById<LinearLayout>(Resource.Id.countryDetailsTitleRoot);

            this.cardCountryCount = FindViewById<CardView>(Resource.Id.cardCountryCount);
            this.cardTodayCases = FindViewById<CardView>(Resource.Id.cardTodayCases);
            this.cardTodayDeaths = FindViewById<CardView>(Resource.Id.cardTodayDeaths);
            this.cardCriticalCases = FindViewById<CardView>(Resource.Id.cardCriticalCases);
            this.cardActiveCases = FindViewById<CardView>(Resource.Id.cardActiveCases);
            this.cardRecoveredCases = FindViewById<CardView>(Resource.Id.cardRecoveredCases);
            this.cardDeathCases = FindViewById<CardView>(Resource.Id.cardDeathCases);
            this.cardDeathRate = FindViewById<CardView>(Resource.Id.cardDeathRate);

            this.cardRecoveredCases.Click += Card_Click;
            this.cardCountryCount.Click += Card_Click;
            this.cardTodayCases.Click += Card_Click;
            this.cardTodayDeaths.Click += Card_Click;
            this.cardCriticalCases.Click += Card_Click;
            this.cardActiveCases.Click += Card_Click;
            this.cardDeathCases.Click += Card_Click;
            this.cardDeathRate.Click += Card_Click;


            this.btnCloseDetails.Click += ImgCloseDetails_Click;
            this.btnCountryCharts.Click += BtnCountryCharts_Click;

        }
        IEnumerable<CountryTimeseriesDay> CountryTimeseries;
        void BtnCountryCharts_Click(object sender, EventArgs e)
        {
            var Btn = sender as ImageButton;
            Report.LoadTimeseries();
            if (Report.Timeseries is null)
            {
                Toast.MakeText(Btn.Context, "Información no disponible.", ToastLength.Short);
                return;
            }
            GoToCharts();
        }
        void GoToCharts()
        {
            var intent = new Intent(btnCountryCharts.Context, typeof(ChartsActivity));
            intent.PutExtra(nameof(Report), Report.ToJson());
            StartActivityForResult(intent, 0);
        }

        private void Card_Click(object sender, EventArgs e)
        {
            var SenderView = sender as CardView;
            if (SenderView is null)
                return;
            string[] XY = SenderView.Tag.ToString().Split(',');
            OnItemClickCallback?.Invoke(this, new DetailsItemClickEventArgs(SenderView, SenderView.ContentDescription, XY[1] == "0" ? ToolTip.PositionRightTo : ToolTip.PositionLeftTo));
        }

        private void TxtCountryName_Click(object sender, EventArgs e)
        {

        }


        private void ImgCloseDetails_Click(object sender, EventArgs e) => OnDestroyCallback?.Invoke(this, EventArgs.Empty);
        T FindViewById<T>(int ResourceId) where T : View => this.RootView.FindViewById<T>(ResourceId);

        public void Update(CovidCountryReport Report)
        {
            this.Report = Report;
            this.txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            this.txtCountryCount.Text = Report.Cases.ToKMB();
            this.txtTodayCases.Text = Report.TodayCases.ToString().TryToLongKMB();
            this.txtCriticalCases.Text = Report.Critical.ToString().TryToLongKMB();
            this.txtActiveCases.Text = Report.Active.ToString().TryToLongKMB();
            this.txtRecoveredCases.Text = Report.Recovered.ToKMB();
            this.txtDeathCases.Text = Report.Deaths.ToKMB();
            this.txtTodayDeaths.Text = Report.TodayDeaths.ToString().TryToLongKMB();
            this.txtDeathRate.Text = Report.DeathRate?.ToString() ?? "N/A";
            ResolveFlagDrawable(Report.CountryCode);
            //ShowChartsTooltip();
        }
        void ShowChartsTooltip()
        {
            var Listener = Activity as ITipListener;
            var ToolTips = new ToolTipsManager();
            var builder = new ToolTip.Builder(btnCountryCharts.Context, btnCountryCharts, LinearRoot, "¡Nuevo!", ToolTip.PositionBelow);
            builder.SetAlign(ToolTip.AlignCenter);
            builder.SetBackgroundColor(Resource.Color.material_grey_50);
            Task.Delay(2000).ContinueWith((task) => Activity.RunOnUiThread(() => ToolTips.Show(builder.Build())));
        }

        void ResolveFlagDrawable(string CountryCode)
        {
            if (CountryCode is null)
                return;
            int FlagId = this.Resources.GetIdentifier(CountryCode.ToLower(), nameof(Resource.Drawable).ToLower(), this.Activity.PackageName);
            if (FlagId == 0)
                FlagId = Resource.Drawable.flag_purple;
            Android.Graphics.Drawables.Drawable draw = this.Context.GetDrawable(FlagId);
            this.txtCountryName.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, draw, null, null);
        }
    }
}