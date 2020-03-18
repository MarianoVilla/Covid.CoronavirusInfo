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
        //@ToDo: find a way to get a Drawable here.
        //@body The design doesn't seem to be helping. I thought about putting the flag as a property on the CovidCountryReport, but that makes caching harder.
        //The next choice would be a pointer to an image, but that's what the CountryCode is for, so it kind of defeats the purpose of that.
        public void Update(CovidCountryReport Report)
        {
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            //txtCountryName.SetCompoundDrawablesWithIntrinsicBounds(CountryNameDrawable, null, null, null);
            txtCases.Text = Report.Cases;
        }
    }
}