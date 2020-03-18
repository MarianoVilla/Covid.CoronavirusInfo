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
        ImageView imgCloseDetails;

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
            //imgCloseDetails = FindViewById<ImageView>(Resource.Id.imgCloseDetails);
            //imgCloseDetails.Click += ImgCloseDetails_Click;

        }
        public CountryDetailsFragment()
        {
        }

        private void ImgCloseDetails_Click(object sender, EventArgs e)
        {
            FragmentManager.BeginTransaction().Hide(this).Commit();
        }
        T FindViewById<T>(int ResourceId) where T: View => RootView.FindViewById<T>(ResourceId);

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
            //@ToDo: find a better fallback flag.
            var FlagId = Resources.GetIdentifier(CountryCode.ToLower(), nameof(Resource.Drawable).ToLower(), Activity.PackageName);
            if (FlagId == 0)
                FlagId = Resource.Drawable.marker;
            var draw = Context.GetDrawable(FlagId);
            txtCountryName.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
    }
}