using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Covid.Model;
using Covid.Lib;
using Com.Tomergoldst.Tooltips;
using Covid.Droid.Model;
using Android.Support.V7.Widget;

namespace Covid.Droid.Fragments
{
    public class CountryDetailsFragment : Android.Support.V4.App.DialogFragment
    {
        View RootView;
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

        ImageView btnCloseDetails;

        public event EventHandler OnDestroyCallback;
        public event EventHandler OnItemClickCallback;
        public CountryDetailsFragment() { }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.country_details_fragment_grid, container, false);
            InitControls();
            return RootView;
        }

        private void InitControls()
        {
            txtCountryName = FindViewById<TextView>(Resource.Id.txtCountryName);
            txtCountryCount = FindViewById<TextView>(Resource.Id.txtCountryCount);
            txtTodayCases = FindViewById<TextView>(Resource.Id.txtTodayCases);
            txtCriticalCases = FindViewById<TextView>(Resource.Id.txtCriticalCases);
            txtActiveCases = FindViewById<TextView>(Resource.Id.txtActiveCases);
            txtRecoveredCases = FindViewById<TextView>(Resource.Id.txtRecoveredCases);
            txtDeathCases = FindViewById<TextView>(Resource.Id.txtDeathCases);
            txtTodayDeaths = FindViewById<TextView>(Resource.Id.txtTodayDeaths);
            txtDeathRate = FindViewById<TextView>(Resource.Id.txtDeathRate);
            btnCloseDetails = FindViewById<ImageButton>(Resource.Id.btnCloseDetails);

            cardCountryCount = FindViewById<CardView>(Resource.Id.cardCountryCount);
            cardTodayCases = FindViewById<CardView>(Resource.Id.cardTodayCases);
            cardTodayDeaths = FindViewById<CardView>(Resource.Id.cardTodayDeaths);
            cardCriticalCases = FindViewById<CardView>(Resource.Id.cardCriticalCases);
            cardActiveCases = FindViewById<CardView>(Resource.Id.cardActiveCases);
            cardRecoveredCases = FindViewById<CardView>(Resource.Id.cardRecoveredCases);
            cardDeathCases = FindViewById<CardView>(Resource.Id.cardDeathCases);
            cardDeathRate = FindViewById<CardView>(Resource.Id.cardDeathRate);

            cardRecoveredCases.Click += Card_Click;
            cardCountryCount.Click += Card_Click;
            cardTodayCases.Click += Card_Click;
            cardTodayDeaths.Click += Card_Click;
            cardCriticalCases.Click += Card_Click;
            cardActiveCases.Click += Card_Click;
            cardDeathCases.Click += Card_Click;
            cardDeathRate.Click += Card_Click;


            btnCloseDetails.Click += ImgCloseDetails_Click;

        }

        private void Card_Click(object sender, EventArgs e)
        {
            var SenderView = sender as CardView;
            if (SenderView is null)
                return;
            var XY = SenderView.Tag.ToString().Split(',');
            OnItemClickCallback?.Invoke(this, new DetailsItemClickEventArgs(SenderView, SenderView.ContentDescription, XY[1] == "0" ?  ToolTip.PositionRightTo : ToolTip.PositionLeftTo));
        }

        private void TxtCountryName_Click(object sender, EventArgs e)
        {

        }


        private void ImgCloseDetails_Click(object sender, EventArgs e)
        {
            OnDestroyCallback?.Invoke(this, EventArgs.Empty);
        }
        T FindViewById<T>(int ResourceId) where T : View => RootView.FindViewById<T>(ResourceId);

        public void Update(CovidCountryReport Report)
        {
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            txtCountryCount.Text = Report.Cases.ToKMB();
            txtTodayCases.Text = Report.TodayCases.ToString().TryToLongKMB();
            txtCriticalCases.Text = Report.Critical.ToString().TryToLongKMB();
            txtActiveCases.Text = Report.Active.ToString().TryToLongKMB();
            txtRecoveredCases.Text = Report.Recovered.ToKMB();
            txtDeathCases.Text = Report.Deaths.ToKMB();
            txtTodayDeaths.Text = Report.TodayDeaths.ToString().TryToLongKMB();
            txtDeathRate.Text = Report.DeathRate?.ToString() ?? "N/A";
            ResolveFlagDrawable(Report.CountryCode);
        }

        void ResolveFlagDrawable(string CountryCode)
        {
            if (CountryCode is null)
                return;
            var FlagId = Resources.GetIdentifier(CountryCode.ToLower(), nameof(Resource.Drawable).ToLower(), Activity.PackageName);
            if (FlagId == 0)
                FlagId = Resource.Drawable.flag_purple;    
            var draw = Context.GetDrawable(FlagId);
            txtCountryName.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
    }
}