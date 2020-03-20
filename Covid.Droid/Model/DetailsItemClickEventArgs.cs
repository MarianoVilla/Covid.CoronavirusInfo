using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Covid.Droid.Model
{
    public class DetailsItemClickEventArgs : EventArgs
    {
        public DetailsItemClickEventArgs(View anchor, string toolTipText, int? position = null)
        {
            this.Anchor = anchor;
            this.ToolTipText = toolTipText;
            this.Position = position;
        }

        public View Anchor { get; set; }
        public string ToolTipText { get; internal set; }
        public int? Position { get; internal set; }
    }
}