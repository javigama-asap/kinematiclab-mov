using System;
using System.Threading.Tasks;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace kinematiclabs
{
    public partial class Conditions : ContentPage
    {
        public Conditions()
        {
            InitializeComponent();
        }

        void DismissPopup(object sender, EventArgs ea)
        {
            Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("Conditions");
        }

    }
}