﻿using Foundation;
using QuickLook;
using System;
using System.IO;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using kinematiclabs.iOS;
using kinematiclabs;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace kinematiclabs.iOS
{
    /*
     *  Render of all textfield on app
     */
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            /*if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.Layer.CornerRadius = 10;
                Control.TextColor = UIColor.Black;

            }*/
        }
    }
}