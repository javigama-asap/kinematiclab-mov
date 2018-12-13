using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.GoogleAnalytics;
using Plugin.CurrentActivity;

namespace kinematiclabs.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            GoogleAnalytics.Current.Config.TrackingId = "UA-128911156-1";
            GoogleAnalytics.Current.Config.AppId = "Kinematic_Android";
            GoogleAnalytics.Current.Config.AppName = "Kinematic Lab Mov App";
            GoogleAnalytics.Current.Config.AppVersion = "Android 1.0";
            GoogleAnalytics.Current.InitTracker();

            CrossCurrentActivity.Current.Activity = this;

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}