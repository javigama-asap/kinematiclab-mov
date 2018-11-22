using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Diagnostics;

using Xamarin.Forms;
using Plugin.GoogleAnalytics;

namespace kinematiclabs
{   
	public partial class FellowEdit : ContentPage
    {
        public UserClass userObj;
        public string fellowID;

		public FellowEdit(string id = "0")
        {
            userObj = Application.Current.Properties["user"] as UserClass;
            fellowID = id;

			InitializeComponent();

            TitleLabel.Text = fellowID != "0" ? "Edición de sujeto" : "Nuevo sujeto";
            saveBtn.Text = fellowID != "0" ? "Guardar cambios" : "Crear sujeto";

            sexPicker.Items.Add("Hombre");
            sexPicker.Items.Add("Mujer");
            sexPicker.SelectedIndex = 0;

            birthdatePicker.Date = DateTime.Now;

            NavigationPage.SetHasNavigationBar(this, false);
 
        }

        public async void GetFellow()
        {
            HttpClient httpClient = new HttpClient();
            string urlrequest = "http://appws.kinematiclab.org/api/fellows/" + fellowID;
                
            httpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                var response = await httpClient.GetAsync(new Uri(urlrequest));
                    
    			var resp = response.Content.ReadAsStringAsync().Result;

                var fellowObj = JsonConvert.DeserializeObject<FellowClass>(resp);
                    
    		    if (fellowObj.Id != null)
				{
					nameEntry.Text = fellowObj.Name;
                    lastnameEntry.Text = fellowObj.Lastname;
				    sexPicker.SelectedIndex = fellowObj.Sex == "Hombre" ? 0 : 1;

                    if (fellowObj.Birthdate != null)
                        birthdatePicker.Date = Convert.ToDateTime(fellowObj.Birthdate);
			    }
            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
            }

            httpClient.Dispose();
        }

        public async void SaveAction(object sender, ItemTappedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El nombre es obligatorio", "OK");
            else if (string.IsNullOrEmpty(lastnameEntry.Text))
                await Application.Current.MainPage.DisplayAlert("Atencìón", "El campo apellidos es obligatorio", "OK");
            else
            {
                saveBtn.IsEnabled = false;

                String mydate = birthdatePicker.Date.Day + "/" + birthdatePicker.Date.Month + "/" + birthdatePicker.Date.Year + " 0:00:00";

                var data = new Dictionary<string, string>
                {
                    ["user_id"] = userObj.Id,
                    ["name"] = nameEntry.Text,
                    ["lastname"] = lastnameEntry.Text,
                    ["sex"] = sexPicker.Items[sexPicker.SelectedIndex],
                    ["birthdate"] = mydate
                };

                string urlrequest = fellowID != null && fellowID != "0"
                    ? "http://appws.kinematiclab.org/api/fellows/update/" + fellowID
                    : "http://appws.kinematiclab.org/api/fellows/store";

                HttpClient httpClient = new HttpClient();

                var content = new FormUrlEncodedContent(data);

                httpClient.Timeout = TimeSpan.FromSeconds(15);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = await httpClient.PostAsync(new Uri(urlrequest), content);
                    var resp = response.Content.ReadAsStringAsync().Result;
                    
					var fellowObj = JsonConvert.DeserializeObject<FellowClass>(resp);

                    if (string.IsNullOrEmpty(fellowObj.Id))
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                        saveBtn.IsEnabled = true;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Correcto!", "Sujeto creado / editado correctamente", "OK");
                        await Navigation.PopAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MENSAJE:  " + ex.Message);
                    await Application.Current.MainPage.DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                    saveBtn.IsEnabled = true;
                }
                saveBtn.IsEnabled = true;
                httpClient.Dispose();
            }

            saveBtn.IsEnabled = true;

        }

        public void HomeAction(object sender, EventArgs ea)
        {
            Application.Current.MainPage = new NavigationPage(new Home());
        }

        public async void BackAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //GoogleAnalytics.Current.Tracker.SendView("FellowEdit");

            if (fellowID != null && fellowID != "0")
                GetFellow();         
        }
    }
}
