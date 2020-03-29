
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;

namespace Covid.Droid.Helpers
{
    public class FirebaseHandler
    {
        readonly Context context;
        public FirebaseHandler(Context context) => this.context = context;
        public bool IsPlayServicesAvailable(out string ErrorString)
        {
            ErrorString = null;
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    ErrorString = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    ErrorString = "This device is not supported";
                }
                return false;
            }
            else
            {
                return true;
            }
        }
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        public static string Token { get; internal set; }

        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };
            var notificationManager = (NotificationManager)this.context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}