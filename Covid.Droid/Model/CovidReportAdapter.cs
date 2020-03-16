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
    public class CovidReportAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<CovidCountryReport> ItemClick;

        List<CovidCountryReport> Reports;

        public CovidReportAdapter(List<CovidCountryReport> Reports)
        {
            this.Reports = Reports;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        { 
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.covid_report_view, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            var vh = new CovidViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CovidViewHolder;
            vh.Update(Reports[position]);
        }

        public override int ItemCount
        {
            get { return Reports.Count; }
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, Reports[position]);
        }
    }
}