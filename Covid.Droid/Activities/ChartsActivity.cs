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
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using Covid.Lib;
using Covid.Droid.Model;

namespace Covid.Droid.Activities
{
    [Activity(Label = "ChartsActivity", MainLauncher = false)]
    public class ChartsActivity : Activity
    {
        CovidCountryReport Report;
        string SelectedChart = ChartTypes.LineChart.ToDescriptionString();

        Spinner howManyDaysSpinner;
        Spinner spinnerChartType;
        TextView txtProgressionTitle;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.charts_view);
            this.Report = Intent.GetStringExtra(nameof(Report)).FromJson<CovidCountryReport>();

            spinnerChartType = FindViewById<Spinner>(Resource.Id.spinnerChartType);
            spinnerChartType.Adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, 
                new[] { ChartTypes.LineChart.ToDescriptionString(), ChartTypes.BarChart.ToDescriptionString(), ChartTypes.PointChart.ToDescriptionString() });
            spinnerChartType.ItemSelected += SpinnerChartType_ItemSelected;


            howManyDaysSpinner = FindViewById<Spinner>(Resource.Id.spinnerDaysSpan);
            howManyDaysSpinner.Adapter = new ArrayAdapter<int>(this, Resource.Layout.list_item, new []{ 5, 10, 20, 30, 60 });
            howManyDaysSpinner.ItemSelected += HowManyDaysSpinner_ItemSelected;

            txtProgressionTitle = FindViewById<TextView>(Resource.Id.txtProgressionTitle);
            txtProgressionTitle.Text = $"Progresión para {Report.RegionalFriendlyName ?? Report.Country}";
        }

        public override void OnBackPressed()
        {
            var intent = new Intent();
            intent.PutExtra(nameof(Report), Report.ToJson());
            SetResult(0, intent);
            Finish();
        }

        private void SpinnerChartType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var TheSpinner = sender as Spinner;
            SelectedChart = (string)TheSpinner.Adapter.GetItem(e.Position);
            SetupCharts((int)howManyDaysSpinner.SelectedItem);
        }

        private void HowManyDaysSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) 
        {
            var TheSpinner = sender as Spinner;
            var SelectedItem = (int)TheSpinner.Adapter.GetItem(e.Position);
            SetupCharts(SelectedItem);
        }
        void SetupCharts(int DaysToShow = 10) 
        {
            SetupCasesChart(DaysToShow);
            SetupDeathsChart(DaysToShow);
            SetupRecoveredChart(DaysToShow);
        }
        void SetupCasesChart(int DaysToShow = 5)
        {
            var CasesTimeseries = new List<Entry>();
            foreach (var day in Report.Timeseries.OrderBy(x => x.Date).Where(x => x.Confirmed != null).TakeLast(DaysToShow))
            {
                CasesTimeseries.Add(new Entry((float)day.Confirmed)
                {
                    Label = day.Date.HasValue ? day.Date.Value.ToShortDateString() : "",
                    ValueLabel = day.Deaths?.ToString(),
                    Color = SKColor.Parse("#f2df0a")
                });
            }
            var Chart = GetChart(CasesTimeseries);
            var CasesProgressionChart = FindViewById<ChartView>(Resource.Id.chartCasesProgression);
            CasesProgressionChart.Chart = Chart;
        }
        void SetupDeathsChart(int DaysToShow = 5)
        {
            var DeathTimeseries = new List<Entry>();
            foreach (var day in Report.Timeseries.OrderBy(x => x.Date).Where(x => x.Deaths != null).TakeLast(DaysToShow))
            {
                DeathTimeseries.Add(new Entry((float)day.Deaths)
                {
                    Label = day.Date.HasValue ? day.Date.Value.ToShortDateString() : "",
                    ValueLabel = day.Deaths?.ToString(),
                    Color = SKColor.Parse("#f54242")
                });
            }
            var Chart = GetChart(DeathTimeseries);
            var DeathProgressionChart = FindViewById<ChartView>(Resource.Id.chartDeathProgression);
            DeathProgressionChart.Chart = Chart;
        }
        void SetupRecoveredChart(int DaysToShow = 5)
        {
            var RecoveredTimeseries = new List<Entry>();
            foreach (var day in Report.Timeseries.OrderBy(x => x.Date).Where(x => x.Recovered != null).TakeLast(DaysToShow))
            {
                RecoveredTimeseries.Add(new Entry((float)day.Recovered)
                {
                    Label = day.Date.HasValue ? day.Date.Value.ToShortDateString() : "",
                    ValueLabel = day.Deaths?.ToString(),
                    Color = SKColor.Parse("#0af24c")
                });
            }
            var Chart = GetChart(RecoveredTimeseries);
            var RecoveredProgressionChart = FindViewById<ChartView>(Resource.Id.chartRecoveredProgression);
            RecoveredProgressionChart.Chart = Chart;
        }
        Chart GetChart(IEnumerable<Entry> Entries)
        {
            switch (SelectedChart)
            {
                case "Líneas": return new LineChart() { Entries = Entries, LineSize = 8, PointSize = 18, LabelTextSize = 18 };
                case "Barras": return new BarChart() { Entries = Entries, LabelTextSize = 18, PointSize = 18 };
                case "Puntos": return new PointChart() { Entries = Entries, LabelTextSize = 18, PointSize = 18 };
                default: throw new NotImplementedException();
            }
        }
    }
}