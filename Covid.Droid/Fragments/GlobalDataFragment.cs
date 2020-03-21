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
using Covid.Lib;
using Android.Support.V7.Widget;
using Android.Support.Design.Animation;
using System.Threading.Tasks;
using Covid.Droid.Helpers;

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
            this.cardGlobalCases = RootView.FindViewById<CardView>(Resource.Id.cardGlobalCases);
            this.cardGlobalDeaths = RootView.FindViewById<CardView>(Resource.Id.cardGlobalDeaths);
            this.cardGlobalRecovered = RootView.FindViewById<CardView>(Resource.Id.cardGlobalRecovered);

            this.cardGlobalCases.Click += CardGlobalCases_Click; ;
            this.cardGlobalDeaths.Click += CardGlobalCases_Click;
            this.cardGlobalRecovered.Click += CardGlobalCases_Click;
        }
        //@ToDo countries searchbar.
        //bool Animating;
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
            txtGlobalCases.Text = Report.Cases.ToKMB();
            txtGlobalDeaths.Text = Report.Deaths.ToKMB();
            txtGlobalRecovered.Text = Report.Recovered.ToKMB();
        }

    }
}