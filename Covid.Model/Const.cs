using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Covid.Model
{
    public class Const
    {
        public static readonly Uri[] Endpoints = { new Uri(@"https://coronavirus-19-api.herokuapp.com/"), new Uri(@"https://corona.lmao.ninja/") };
        public static HttpClient GlobalHttpClient;
    }
}
