using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using kinematiclabs.Droid;
using Android.Content;
using Android.Widget;
using kinematiclabs;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Content.Res;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace kinematiclabs.Droid
{
    public class CustomPickerRenderer : PickerRenderer
    {
        public CustomPickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                Control.SetBackground(gd);
                Control.Gravity = GravityFlags.CenterHorizontal;
            }
        }
    }
}
