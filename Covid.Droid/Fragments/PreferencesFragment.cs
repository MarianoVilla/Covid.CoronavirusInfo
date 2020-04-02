using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using Covid.Droid.Helpers;
using static Android.Support.V7.Preferences.Preference;

namespace Covid.Droid.Fragments
{
    public class PreferencesFragment : PreferenceFragmentCompat
    {
        SwitchPreferenceCompat switchDarkTheme;
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey) 
        {
            AddPreferencesFromResource(Resource.Layout.preferences);

        }
        public override bool OnPreferenceTreeClick(Android.Support.V7.Preferences.Preference preference) 
        {
            if (!(preference is SwitchPreferenceCompat Pref))
                return false;
            SharedPreferencesHandler.SetUseDarkTheme(this.Context, Pref.Checked);
            return true;
        }
    }
}