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

        public async Task GetGlobal()
        {
            try
            {
                using HttpResponseMessage Res = await Const.GlobalHttpClient.GetAsync("All");
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

        public async Task GetDataByCountries()
        {
            try
            {
                using HttpResponseMessage Res = await Const.GlobalHttpClient.GetAsync("Countries");
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

        public void NotifyFailure(Exception e) => OnFailureListeners.ForEach(x => x.OnFailure(e));

        public void NotifySuccess(object Result) => OnSuccessListeners.ForEach(x => x.OnSuccess(Result));
    }
}
