/*SOS Perito es un software propiedad de Soluciones Perin S.C.P.*/

using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using Android.Content;
using Android.Widget;
using kinematiclabs;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Content.Res;
using Android.Views;
using kinematiclabs.Droid;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace kinematiclabs.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }


        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                //gd.SetCornerRadius(10);
                //gd.SetStroke(2, global::Android.Graphics.Color.LightGray);
                Control.Gravity = GravityFlags.Center;
                Control.SetBackground(gd);
                Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                //Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.White));
                Control.SetPadding(0,3,0,0);


            }
        }
    }
}