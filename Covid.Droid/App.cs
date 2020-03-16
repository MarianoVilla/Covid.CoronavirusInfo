using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid.Model;

namespace Covid.Droid
{
    [Application]
    public class App : Application
    {
        protected App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void OnCreate()
        {
            Init();
            base.OnCreate();
        }

        private void Init()
        {
            var Cookies = new CookieContainer();
            var Handler = new HttpClientHandler() { CookieContainer = Cookies };
            Const.GlobalHttpClient = new HttpClient(Handler) { BaseAddress = Const.RestUri };
            Const.GlobalHttpClient.DefaultRequestHeaders.Add("accept", "*/*");
        }
    }
}