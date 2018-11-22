using Newtonsoft.Json;
using Plugin.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kinematiclabs
{
	public partial class Login : ContentPage
	{
        public string email;
        public string password;
        public string authtoken;

        public Login()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

			/*emailEntry.Text= "javigama@yahoo.es";
			passwordEntry.Text = "depeche";*/
        }

        public Login(string email)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            emailEntry.Text = email;
        }

        public async void LoginUser(object sender, EventArgs ea)
        {
            loginActionBtn.IsEnabled = false;
            email = emailEntry.Text;
            password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("¡Atención!", "Introduzca su Correo electrónico", "OK");
                loginActionBtn.IsEnabled = true;
            }
            else if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert("¡Atención!", "Introduzca su contraseña", "OK");
                loginActionBtn.IsEnabled = true;
            }
            else
            {
                String resp = await Functions.Authentication.GetAuthorization(email, password);

                if (resp == "falsehttp")
                {
                    await DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                    loginActionBtn.IsEnabled = true;
                }
                else
                {
                    try
                    {
                        var userObj = JsonConvert.DeserializeObject<UserClass>(resp);

                        if (String.IsNullOrEmpty(userObj.Id))
                        {
                            await DisplayAlert("¡Atención!", "Correo electrónico y/o contraseña incorrectos", "OK");
                            loginActionBtn.IsEnabled = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(userObj.Api_token))
                            {
                                Application.Current.Properties["AuthToken"] = userObj.Api_token;
                                Application.Current.Properties["email"] = email;
                                Application.Current.Properties["password"] = password;

                                Application.Current.Properties["user"] = userObj;
                                await Application.Current.SavePropertiesAsync();

                                Functions.Authentication.SaveCredentials(email, password, userObj.Api_token);

                                Application.Current.MainPage = new NavigationPage(new Home());

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("MENSAJE:  " + e.Message);
                        loginActionBtn.IsEnabled = true;
                    }

                }

            }

        }

        public async void ResetPassword(object sender, EventArgs ea)
        {
            Debug.WriteLine("forgor password");
            var newPage = new ResetPassword();
            await App.MyNavigationPage.PushAsync(newPage);
        }

        //Create an accountbutton action
        public async void CreateAccount(object sender, EventArgs ea)
        {
            var newPage = new Register();
            await App.MyNavigationPage.PushAsync(newPage);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("Login");
        }
    }
}