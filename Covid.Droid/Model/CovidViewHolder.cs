using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Covid.Model;
using Covid.Lib;

namespace Covid.Droid.Model
{
    public class CovidViewHolder : RecyclerView.ViewHolder
    {
        public CovidCountryReport Report { get; set; }
        TextView txtCountryName { get; }
        TextView txtCases;
        public CovidViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            txtCountryName = itemView.FindViewById<TextView>(Resource.Id.txtCountryName);
            txtCases = itemView.FindViewById<TextView>(Resource.Id.txtCountryCount);
            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
        public void Update(CovidCountryReport Report)
        {
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            txtCases.Text = $"{Report.Cases.ToKMB()}";
        }
    }
}