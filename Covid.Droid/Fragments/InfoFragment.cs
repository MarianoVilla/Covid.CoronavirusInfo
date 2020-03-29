using Android.OS;
using Android.Views;

namespace Covid.Droid.Fragments
{
    public class InfoFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState) => base.OnCreate(savedInstanceState);// Create your fragment here

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) =>
            // Use this to return your custom view for this Fragment
            inflater.Inflate(Resource.Layout.info_fragment, container, false);
    }
}