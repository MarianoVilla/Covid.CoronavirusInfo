using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Covid.Model;
using Covid.Lib;

namespace Covid.Droid.Model
{
    public class CovidViewHolder : RecyclerView.ViewHolder
    {
        public CovidCountryReport Report { get; set; }
        TextView txtCountryName { get; }
        TextView txtCases;
        public CovidViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            txtCountryName = itemView.FindViewById<TextView>(Resource.Id.txtCountryName);
            txtCases = itemView.FindViewById<TextView>(Resource.Id.txtCountryCount);
            itemView.Click += (sender, e) => listener(LayoutPosition);
            txtCountryName.Touch += TxtCountryName_Touch;
        }
        public void Update(CovidCountryReport Report)
        {
            this.Report = Report;
            txtCountryName.Text = Report.RegionalFriendlyName ?? Report.Country;
            ResolveFavouriteStar();
            txtCases.Text = $"{Report.Cases.ToKMB()}";

        }
        private void TxtCountryName_Touch(object sender, View.TouchEventArgs e)
        {
            var rightDrawable = txtCountryName.GetCompoundDrawables()[2];
            if (rightDrawable == null || e.Event.Action != MotionEventActions.Down)
            {
                e.Handled = false;
                return;
            }
            if (e.Event.GetX() >= txtCountryName.Width - txtCountryName.TotalPaddingRight)
            {
                Report.IsFavourite = Report.IsFavourite ? false : true;
                ResolveFavouriteStar();
                NotifyFavouriteState();
                e.Handled = true;
            }
            (sender as View)?.OnTouchEvent(e.Event);
        }

        void ResolveFavouriteStar()
        {
            var Id = Report.IsFavourite ? Resource.Drawable.star_filled : Resource.Drawable.star_empty;
            var Draw = txtCountryName.Context.GetDrawable(Id);
            txtCountryName.SetCompoundDrawablesWithIntrinsicBounds(null, null, Draw, null);
        }
        void NotifyFavouriteState() => Toast.MakeText(txtCountryName.Context, FavouriteStateMessage, ToastLength.Short).Show();
        string FavouriteStateMessage => Report.IsFavourite ? "Se mostrará primero en la lista." : "Ya no se mostrará primero en la lista.";

    }
}