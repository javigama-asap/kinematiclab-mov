using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.GoogleAnalytics;
using UIKit;

namespace kinematiclabs.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;

            System.Threading.Thread.Sleep(3000);

            GoogleAnalytics.Current.Config.TrackingId = "UA-128911156-1";
            GoogleAnalytics.Current.Config.AppId = "KinematicI_iOS";
            GoogleAnalytics.Current.Config.AppName = "Kinematic Lab Mov App";
            GoogleAnalytics.Current.Config.AppVersion = "iOS 1.0";
            GoogleAnalytics.Current.InitTracker();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
