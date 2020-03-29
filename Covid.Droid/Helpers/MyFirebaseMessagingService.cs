using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;

namespace Covid.Droid.Helpers
{
    [Service(Name = "com.marianovilla.coronappvirus.MyFirebaseMessagingService")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
        }

        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            base.OnMessageReceived(remoteMessage);
        }
    }
}