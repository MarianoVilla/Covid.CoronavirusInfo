using Android.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Covid.Droid.Helpers
{
    public class AnimationHandler
    {
        View Control;
        Dictionary<View, bool> IsAnimating = new Dictionary<View, bool>();
        #region Constructors.
        public AnimationHandler(View Control) => this.Control = Control;
        public AnimationHandler() { }
        #endregion

        public void FadeIn(View Control = null, float Alpha = 100, long Duration = 1000)
        {
            if (this.IsAnimating[Control])
                return;
            this.IsAnimating.TryAdd(Control, true);
            Control = Control ?? this.Control;
            Control.Animate().Alpha(100).SetDuration(Duration).Start();
            _ = Task.Delay((int)Duration).ContinueWith((task) => this.IsAnimating[Control] = false);
        }
        public void FadeOut(View Control = null, float Alpha = 0, long Duration = 1000)
        {
            Control = Control ?? this.Control;
            Control.Animate().Alpha(Alpha).SetDuration(Duration).Start();
        }
    }
}