using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid.Model;

namespace Covid.Droid.Fragments
{
    public class GlobalDataFragment : Android.Support.V4.App.Fragment
    {
        View RootView;
        TextView txtGlobalCases;
        TextView txtGlobalDeaths;
        TextView txtGlobalRecovered;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.global_data, container, false);
            RootView.Visibility = ViewStates.Visible;
            InitControls();
            return RootView;
        }

        private void InitControls()
        {
            this.txtGlobalCases = RootView.FindViewById<TextView>(Resource.Id.txtGlobalCases);
            this.txtGlobalDeaths = RootView.FindViewById<TextView>(Resource.Id.txtGlobalDeaths);
            this.txtGlobalRecovered = RootView.FindViewById<TextView>(Resource.Id.txtGlobalRecovered);
        }
        public void Update(CovidReport Report)
        {
            txtGlobalCases.Text = Report.Cases;
            txtGlobalDeaths.Text = Report.Deaths;
            txtGlobalRecovered.Text = Report.Recovered;
        }
    }
}