using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid.Model;

namespace Covid.Droid.Helpers
{
    public static class ExtensionMethods
    {
        public static void BackAndForthX(this ViewPropertyAnimator TheAnimator, Activity ParentActivity, float First = 1.1F, float Then = 1.0F, long Duration = 300)
        {
            TheAnimator.ScaleX(First).SetDuration(Duration).Start();
            Task.Delay(300).ContinueWith((task) => ParentActivity.RunOnUiThread(() => TheAnimator.ScaleX(Then).Start()));
        }
        public static void LoadFavouritesFromPreferences(this IEnumerable<CovidCountryReport> Reports, Context context)
        {
            var CachedReport = SharedPreferencesHandler.GetCountriesReport(context);
            foreach(var r in Reports)
            {
                r.IsFavourite = CachedReport.FirstOrDefault(x => x.Country.ToLower() == r.Country.ToLower())?.IsFavourite ?? r.IsFavourite;
            }
        }
    }
}