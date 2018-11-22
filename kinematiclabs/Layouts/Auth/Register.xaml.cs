using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Plugin.GoogleAnalytics;

namespace kinematiclabs
{
    public partial class Register : ContentPage
    {
        public Register()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            /*nameEntry.Text = "Prueba";
            lastnameEntry.Text = "de Registro";
            emailEntry.Text = "javigama2012@gmail.com";
            passwordEntry.Text = "depeche";
            confirmpasswordEntry.Text = "depeche";
            cityEntry.Text = "Madrid";
            cpEntry.Text = "28111";*/

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                var page = new Conditions();

                await Navigation.PushModalAsync(page);
            };
            conditionsSwitchLabel.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public async void CancelAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        public async void SendAction(object sender, EventArgs ea)
        {
            if (!conditionsSwitch.IsToggled)
                await Application.Current.MainPage.DisplayAlert("Atencìón", "Debe aceptar las condiciones de uso", "OK");
            else if (string.IsNullOrEmpty(nameEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El nombre es obligatorio", "OK");
            else if (string.IsNullOrEmpty(lastnameEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El campo apellidos es obligatorio", "OK");
            else if (string.IsNullOrEmpty(emailEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El correo electrónico es obligatorio", "OK");
            else if (!Functions.Miscellaneous.IsValidEmail(emailEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "Indique un correo electrónico válido", "OK");
            /*else if (string.IsNullOrEmpty(cityEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El campo Ciudad es obligatorio", "OK");
            else if (string.IsNullOrEmpty(cpEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El campo Código postal es obligatorio", "OK");
            */else if (passwordEntry.Text.Length < 6)
                await Application.Current.MainPage.DisplayAlert("Atencìón", "La contraseña debe tener al menos 6 caracteres", "OK");
            else if (!passwordEntry.Text.Equals(confirmpasswordEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "Las contraseñas deben coincidir", "OK");
            else
            {
                sendBtn.IsEnabled = false;
                cancelBtn.IsEnabled = false;

                var data = new Dictionary<string, string>
                {
                    ["name"] = nameEntry.Text,
                    ["lastname"] = lastnameEntry.Text,
                    ["email"] = emailEntry.Text,
                    ["city"] = cityEntry.Text + "",
                    ["cp"] = cpEntry.Text + "",
                    ["password"] = passwordEntry.Text,
                    ["newsletter"] = newsletterSwitch.IsToggled ? "1" : "0"
                };

                HttpClient httpClient = new HttpClient();
                string urlrequest = "http://appws.kinematiclab.org/api/register";

                var content = new FormUrlEncodedContent(data);

                httpClient.Timeout = TimeSpan.FromSeconds(15);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = await httpClient.PostAsync(new Uri(urlrequest), content);
                    var resp = response.Content.ReadAsStringAsync().Result;

                    var sb = new StringBuilder(resp);
                    sb.Replace("[", "");
                    sb.Replace("]", "");

                    resp = sb.ToString();

                    var userObj = JsonConvert.DeserializeObject<UserClass>(resp);

                    if (string.IsNullOrEmpty(userObj.Id))
                    {
                        if (!String.IsNullOrEmpty(userObj.Errors.Email) && userObj.Errors.Email.Contains("unique"))
                            await Application.Current.MainPage.DisplayAlert("¡Atención!", "Ya existe un usuario con ese correo electrónico", "OK");
                        else
                            await Application.Current.MainPage.DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                        sendBtn.IsEnabled = true;
                        cancelBtn.IsEnabled = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(userObj.Api_token))
                        {
                            Application.Current.Properties["AuthToken"] = userObj.Api_token;
                            Application.Current.Properties["email"] = emailEntry.Text;
                            Application.Current.Properties["password"] = passwordEntry.Text;
                            Application.Current.Properties["user"] = userObj;
                            await Application.Current.SavePropertiesAsync();

                            Functions.Authentication.SaveCredentials(emailEntry.Text, passwordEntry.Text, userObj.Api_token);
                            await Application.Current.MainPage.DisplayAlert("Registro correcto", "¡Bienvenido a  Kinematic Lab!", "OK");
                            Application.Current.MainPage = new NavigationPage(new Home());
                        }
                    }
                }

                catch (Exception e)
                {
                    Debug.WriteLine("MENSAJE:  " + e.Message);
                    await Application.Current.MainPage.DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                }

                sendBtn.IsEnabled = true;
                cancelBtn.IsEnabled = true;
            }

            sendBtn.IsEnabled = true;
            cancelBtn.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("Register");
        }
    }
}