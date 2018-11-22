using Newtonsoft.Json;
using Plugin.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kinematiclabs
{
	public partial class ResetPassword : ContentPage
    {
        public ResetPassword()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void CancelAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        async void SendAction(object sender, EventArgs ea)
        {
            var email = emailEntry.Text;

            sendBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;

            String resp = await Functions.Authentication.SendResetLink(email);

            var response = JsonConvert.DeserializeObject<AuthTokenClass>(resp);
            if (response.status == "OK" && response.resetpassword_token != null)
            {
                var newPage = new CreateNewPassword(email);
                await App.MyNavigationPage.PushAsync(newPage);
            }
            else
            {
                if (resp == "falsehttp")
                    await Application.Current.MainPage.DisplayAlert("Atencìón", "Se ha producido un error al llevar a cabo esta operación, intentelo más tarde", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Atencìón", "Este correo electrónico no pertenece a un usuario válido", "OK");
            }

            sendBtn.IsEnabled = true;
            cancelBtn.IsEnabled = true;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("ResetPassword");
        }
    }
}