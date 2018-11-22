using System;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;


namespace kinematiclabs
{
	public partial class DemoVideo : ContentPage
	{
        public DemoVideo(string videofile)
        {
            InitializeComponent();

            videoPlayer.Source = new ResourceVideoSource
            {
                Path = videofile
            }; 
        }

        void DismissPopup(object sender, EventArgs ea)
        {
            Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("DemoVideo");
        }
    }
}
