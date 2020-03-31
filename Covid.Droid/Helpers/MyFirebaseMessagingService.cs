using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Covid.Droid.Activities;
using Firebase.Iid;
using Firebase.Messaging;
using Java.Net;

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
        const string TAG = "FirebaseNotificationService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            var ss = message.Data.Keys;

            // Pull message body out of the template
            var messageBody = message.GetNotification();
            if (message == null)
                return;

            Log.Debug(TAG, "Notification message body: " + messageBody);
            SendNotification(messageBody);
        }
        public const string URGENT_CHANNEL = "com.xamarin.myapp.urgent";
        public const int NOTIFY_ID = 1100;
        private void SendNotification(RemoteMessage.Notification messageBody)
        {
            var importance = NotificationImportance.High;
            NotificationChannel chan = new NotificationChannel(URGENT_CHANNEL, "Urgent", importance);
            chan.EnableVibration(true);
            chan.LockscreenVisibility = NotificationVisibility.Public;

            // Choose the activity you want to push
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_home_black_24dp)
                .SetContentTitle(messageBody.Title)
                .SetContentText(messageBody.Body)
                .SetContentIntent(pendingIntent)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true)
                .SetChannelId(URGENT_CHANNEL);

            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(chan);

            notificationManager.Notify(NOTIFY_ID, notificationBuilder.Build());
        }

    }
}