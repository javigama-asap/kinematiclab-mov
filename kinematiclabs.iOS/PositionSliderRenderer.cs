using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kinematiclabs;
using Foundation;
using UIKit;
using kinematiclabs.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PositionSlider), typeof(PositionSliderRenderer))]
namespace kinematiclabs.iOS
{
    public class PositionSliderRenderer : SliderRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                const string colorSlider = "#999999";

                Control.MaximumTrackTintColor = Color.FromHex(colorSlider).ToUIColor();
                Control.MinimumTrackTintColor = Color.FromHex(colorSlider).ToUIColor();
                Control.ThumbTintColor = Color.FromHex(colorSlider).ToUIColor();
            }
        }
    }
}