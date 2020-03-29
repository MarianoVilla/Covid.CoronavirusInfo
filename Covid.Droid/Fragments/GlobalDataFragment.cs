using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Covid.Lib;
using Covid.Model;
using System;

namespace Covid.Droid.Fragments
{
    public class GlobalDataFragment : Android.Support.V4.App.Fragment
    {
        View RootView;
        TextView txtGlobalCases;
        TextView txtGlobalDeaths;
        TextView txtGlobalRecovered;
        CardView cardGlobalCases;
        CardView cardGlobalDeaths;
        CardView cardGlobalRecovered;
        public override void OnCreate(Bundle savedInstanceState) => base.OnCreate(savedInstanceState);
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.RootView = inflater.Inflate(Resource.Layout.global_data, container, false);
            this.RootView.Visibility = ViewStates.Visible;
            InitControls();
            return this.RootView;
        }

        private void InitControls()
        {
            this.txtGlobalCases = this.RootView.FindViewById<TextView>(Resource.Id.txtGlobalCases);
            this.txtGlobalDeaths = this.RootView.FindViewById<TextView>(Resource.Id.txtGlobalDeaths);
            this.txtGlobalRecovered = this.RootView.FindViewById<TextView>(Resource.Id.txtGlobalRecovered);
            this.cardGlobalCases = this.RootView.FindViewById<CardView>(Resource.Id.cardGlobalCases);
            this.cardGlobalDeaths = this.RootView.FindViewById<CardView>(Resource.Id.cardGlobalDeaths);
            this.cardGlobalRecovered = this.RootView.FindViewById<CardView>(Resource.Id.cardGlobalRecovered);

            this.cardGlobalCases.Click += CardGlobalCases_Click; ;
            this.cardGlobalDeaths.Click += CardGlobalCases_Click;
            this.cardGlobalRecovered.Click += CardGlobalCases_Click;
        }
        //@ToDo: add a tooltip to the GlobalData cards.
        private void CardGlobalCases_Click(object sender, EventArgs e)
        {
            //if(Animating)
            //    return;
            //Animating = true; 
            //(sender as CardView).Animate().BackAndForthX(Activity);
            //Task.Delay(600).ContinueWith((task) => Animating = false);
        }

        public void Update(CovidReport Report)
        {
            this.txtGlobalCases.Text = Report.Cases.ToKMB();
            this.txtGlobalDeaths.Text = Report.Deaths.ToKMB();
            this.txtGlobalRecovered.Text = Report.Recovered.ToKMB();
        }

    }
}