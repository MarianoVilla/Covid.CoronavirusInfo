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
        ImageView imgCloseDetails;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            RootView = inflater.Inflate(Resource.Layout.country_details_fragment, container, false);
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
            imgCloseDetails = FindViewById<ImageView>(Resource.Id.imgCloseDetails);
            imgCloseDetails.Click += ImgCloseDetails_Click;
            //@ToDo: get country's flag!
            //txtCountryName.SetCompoundDrawables()

        }

        private void ImgCloseDetails_Click(object sender, EventArgs e)
        {
            FragmentManager.BeginTransaction().Hide(this).Commit();
        }

        T FindViewById<T>(int ResourceId) where T: View => RootView.FindViewById<T>(ResourceId);

        public void Update(CovidCountryReport Report)
        {
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            txtCountryCount.Text = Report.Cases;
            txtTodayCases.Text = Report.TodayCases.ToString();
            txtCriticalCases.Text = Report.Critical.ToString();
            txtActiveCases.Text = Report.Active.ToString();
            txtRecoveredCases.Text = Report.Recovered;
            txtDeathCases.Text = Report.Deaths;
            txtTodayDeaths.Text = Report.TodayDeaths.ToString();
            ResolveFlagDrawable(Report.CountryCode);

        }

        void ResolveFlagDrawable(string CountryCode)
        {
            //@ToDo: find a better fallback flag.
            var FlagId = Resources.GetIdentifier(CountryCode.ToLower(), nameof(Resource.Drawable).ToLower(), Activity.PackageName);
            if (FlagId == 0)
                FlagId = Resource.Drawable.marker;
            var draw = Context.GetDrawable(FlagId);
            txtCountryName.SetCompoundDrawablesWithIntrinsicBounds(null, draw, null, null);
        }
    }
}