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

namespace Covid.Droid.Helpers
{
    public class AnimationHandler
    {
        View Control;
        Dictionary<View, bool> IsAnimating = new Dictionary<View, bool>();
        #region Constructors.
        public AnimationHandler(View Control)
        {
            this.Control = Control;
        }
        public AnimationHandler() { }
        #endregion

        public void FadeIn(View Control = null, float Alpha = 100, long Duration = 1000)
        {
            if (IsAnimating[Control])
                return;
            IsAnimating.TryAdd(Control, true);
            Control = Control ?? this.Control;
            Control.Animate().Alpha(100).SetDuration(Duration).Start();
            _ = Task.Delay((int)Duration).ContinueWith((task) => IsAnimating[Control] = false);
        }
        public void FadeOut(View Control = null, float Alpha = 0, long Duration = 1000)
        {
            Control = Control ?? this.Control;
            Control.Animate().Alpha(Alpha).SetDuration(Duration).Start();
        }
    }
}