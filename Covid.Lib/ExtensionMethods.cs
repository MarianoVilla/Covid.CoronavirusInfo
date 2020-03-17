using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Covid.Lib
{
    public static class ExtensionMethods
    {
        public static bool IsWorking(this Uri TheUri, string Action = null)
        {
            TheUri = Action is null ? TheUri : new Uri($"{TheUri}{Action}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TheUri);
            request.Timeout = 10000;
            request.Method = "HEAD";
            try
            {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
    }
}
