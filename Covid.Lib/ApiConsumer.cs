using Covid.Interfaces;
using Covid.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Covid.Lib
{
    public class ApiConsumer : IObservableRestConsumer
    {
        public List<IOnSuccessListener> OnSuccessListeners { get; } = new List<IOnSuccessListener>();
        public List<IOnFailureListener> OnFailureListeners { get; } = new List<IOnFailureListener>();
        public void AddOnFailureListener(IOnFailureListener Listener) => OnFailureListeners.Add(Listener);
        public void AddOnSuccessListener(IOnSuccessListener Listener) => OnSuccessListeners.Add(Listener);

        public static readonly Uri GlobalEndpoint;
        public static readonly Uri ByCountriesEndpoint;
        public static readonly Uri TimeseriesEndpoint;

        public async Task GetGlobalAsync()
        {
            try
            {
                using HttpResponseMessage Res = await Const.GlobalHttpClient.GetAsync(GlobalEndpoint);
                if (Res.IsSuccessStatusCode)
                {
                    using HttpContent content = Res.Content;
                    var Report = await content.ReadAsAsync<CovidReport>();
                    NotifySuccess(Report);
                }
                else
                {
                    throw new Exception("Failed to get global report.");
                }
            }
            catch (Exception ex)
            {
                NotifyFailure(ex);
            }
        }
        public async Task GetDataByCountriesAsync()
        {
            try
            {
                using HttpResponseMessage Res = await Const.GlobalHttpClient.GetAsync(ByCountriesEndpoint);
                if (Res.IsSuccessStatusCode)
                {
                    using HttpContent content = Res.Content;
                    var Report = await content.ReadAsAsync<IEnumerable<CovidCountryReport>>();
                    Report.LoadRegionalFriendlyNames();
                    Report.LoadCountryCodes();
                    NotifySuccess(Report);
                }
                else
                {
                    throw new Exception("Failed to get report by countries.");
                }
            }
            catch (Exception ex)
            {
                NotifyFailure(ex);
            }
        }
        [Obsolete("I ended up downloading the timeseries up to 22/03, then updating it as the days go by.")]
        public async Task GetTimeseriesAsync()
        {
            try
            {
                using HttpResponseMessage Res = await Const.GlobalHttpClient.GetAsync(TimeseriesEndpoint);
                if (Res.IsSuccessStatusCode)
                {
                    using HttpContent content = Res.Content;
                    var Timeseries = await content.ReadAsAsync<CountryTimeseriesContainer>();
                    NotifySuccess(Timeseries);
                }
                else
                {
                    throw new Exception("Failed to get timeseries.");
                }
            }
            catch (Exception ex)
            {
                NotifyFailure(ex);
            }
        }

        public void NotifyFailure(Exception e) => OnFailureListeners.ForEach(x => x.OnFailure(e));

        public void NotifySuccess(object Result) => OnSuccessListeners.ForEach(x => x.OnSuccess(Result));
    }
}
