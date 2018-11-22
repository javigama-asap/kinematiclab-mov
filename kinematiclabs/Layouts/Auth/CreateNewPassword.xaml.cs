using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kinematiclabs
{
	public partial class CreateNewPassword : ContentPage
    {
        public CreateNewPassword(string email)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            emailEntry.Text = email;
        }

        public async void CancelAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        async void SendAction(object sender, EventArgs ea)
        {
            var token = tokenEntry.Text;
            var newpassword = newpasswordEntry.Text;
            var confirmpassword = confirmpasswordEntry.Text;

            if (!newpassword.Equals(confirmpassword))
            {
                await App.Current.MainPage.DisplayAlert("Atención", "Las contraseñas deben coincidir", "OK");
            }
            else if (newpassword == "")
            {
                await App.Current.MainPage.DisplayAlert("Atención", "La contraseña no puede ser nula", "OK");
            }
            else
            {
                sendBtn.IsEnabled = false;
                cancelBtn.IsEnabled = false;

                String resp = await Functions.Authentication.ChangePassword(emailEntry.Text, token, newpassword);

                if (resp.Contains("passwords.reset"))
                {
                    await Application.Current.MainPage.DisplayAlert("Correcto", "Contraseña cambiada correctamente", "OK");
                    var newPage = new Login(emailEntry.Text);
                    await App.MyNavigationPage.PushAsync(newPage);
                }
                else if (resp == "falsehttp")
                    await Application.Current.MainPage.DisplayAlert("Atencìón", "Se ha producido un error al llevar a cabo esta operación, intentelo más tarde", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Atencìón", "Token no válido", "OK");

                sendBtn.IsEnabled = true;
                cancelBtn.IsEnabled = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("CreateNewPassword");
        }
    }
}