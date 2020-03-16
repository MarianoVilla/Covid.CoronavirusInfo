using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Covid.Model
{
    public class Const
    {
        public static readonly string BaseEndpoint = @"https://coronavirus-19-api.herokuapp.com/";
        public static readonly Uri RestUri = new Uri(BaseEndpoint);
        public static HttpClient GlobalHttpClient;
    }
}
