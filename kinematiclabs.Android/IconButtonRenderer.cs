/*SOS Perito es un software propiedad de Soluciones Perin S.C.P.*/

using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using kinematiclabs.Droid;
using Android.Content;

[assembly: ExportRenderer(typeof(Button), typeof(IconButtonRenderer))]
namespace kinematiclabs.Droid
{
	public class IconButtonRenderer : ButtonRenderer
	{
        public IconButtonRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);
			var label = Control;
			Typeface font;
			try
			{

				var text = label.Text;
				if (text.Length > 1 || text[0] < 0xf000)
				{
					return;
				}

				font = Typeface.CreateFromAsset(Context.ApplicationContext.Assets, "FontAwesome.otf");
				label.Typeface = font;

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("TTF file not found. Make sure the Android project contains it at '/Assets/fontawesome-.ttf'.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            Control.SetPadding(0, 0, 0, 0);
            Control.SetIncludeFontPadding(false);

        }
	}
}
