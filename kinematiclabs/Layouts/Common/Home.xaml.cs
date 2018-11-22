using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.GoogleAnalytics;
using Xamarin.Auth;
using Xamarin.Forms;

namespace kinematiclabs
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

        }

        public async void AddFellowAction(object sender, EventArgs ea)
        {
            await Navigation.PushAsync(new FellowEdit());
        }

        public async void LoadFellowAction(object sender, EventArgs ea)
        {
            await Navigation.PushAsync(new FellowsList());
        }

        public void TutorialAction(object sender, EventArgs ea)
        {
            Device.OpenUri(new Uri("https://www.youtube.com/watch?v=JYMFFwWN0dw"));
        }
        public void LogoutAction(object sender, EventArgs args)
        {
            var account = AccountStore.Create().FindAccountsForService("Kinematic").FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create().Delete(account, "Kinematic");
            }

            App.MyNavigationPage = new NavigationPage(new Login());

            Application.Current.MainPage = App.MyNavigationPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("Home");
        }
    }
}
