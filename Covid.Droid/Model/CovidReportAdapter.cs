using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
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
        public event EventHandler<CovidCountryReport> ItemClick;
        List<CovidCountryReport> Reports;
        List<CovidCountryReport> InitialCache;

        public CovidReportAdapter(List<CovidCountryReport> Reports)
        {
            this.Reports = Reports;
            this.InitialCache = Reports.ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.covid_report_view, parent, false);
            var vh = new CovidViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CovidViewHolder;
            vh.Update(Reports[position]);
        }

        public override int ItemCount
        {
            get { return Reports.Count; }
        }
        public void Filter(IEnumerable<CovidCountryReport> Reports)
        {
            this.Reports = Reports.ToList();
        }
        public void FilterByName(string SearchCriteria)
        {
            this.Reports = InitialCache
                .Where(x => x.RegionalFriendlyName != null && x.RegionalFriendlyName.ToLower().Contains(SearchCriteria.ToLower()))
                .Union(Reports.Where(x => x.Country != null && x.Country.ToLower().Contains(SearchCriteria.ToLower())))
                .Union(Reports.Where(x => x.CountryCode != null && x.CountryCode.ToLower().Contains(SearchCriteria.ToLower())))
                .ToList();
            NotifyDataSetChanged();
        }
        public void Unfilter() => this.Reports = InitialCache;
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, Reports[position]);
        }
    }
}