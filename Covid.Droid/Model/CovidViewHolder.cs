using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Covid.Model;

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

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
        public void Update(CovidCountryReport Report)
        {
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            txtCases.Text = Report.Cases;
        }
    }
}