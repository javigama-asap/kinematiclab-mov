using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Xamarin.Forms;
using Plugin.GoogleAnalytics;

namespace kinematiclabs
{
    public partial class FellowDetails : ContentPage
    {
        public UserClass userObj;
        public string fellowID;
        public FellowClass fellowObj;

        public FellowDetails(string id)
        {
            userObj = Application.Current.Properties["user"] as UserClass;
            fellowID = id;

            InitializeComponent();

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

                fellowObj = JsonConvert.DeserializeObject<FellowClass>(resp);

                if (fellowObj.Id != null)
                {
                    nameLabel.Text = fellowObj.Name + " " + fellowObj.Lastname;
                    sexLabel.Text = fellowObj.Sex;
                    var birthdate = Convert.ToDateTime(fellowObj.Birthdate);
                    var today = DateTime.Now;
                    var age = Math.Floor((today - birthdate).TotalDays / 365);
                    ageLabel.Text = Convert.ToString(age) + " años";

                    if (fellowObj.Tests.Count > 0)
                    {
                        TestsListView.ItemsSource = fellowObj.Tests;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
            }
        }

        public void HomeAction(object sender, EventArgs ea)
        {
            Application.Current.MainPage = new NavigationPage(new Home());
        }

        public async void BackAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        public async void EditAction(object sender, EventArgs ea)
        {
            await Navigation.PushAsync(new FellowEdit(fellowID));
        }

        public async void NewAction(object sender, EventArgs ea)
        {
            await Navigation.PushAsync(new NewTest(fellowObj));
        }

        private async void OpenTarget(object sender, ItemTappedEventArgs e)
        {
            var item = TestsListView.SelectedItem as TestClass;

            HttpClient httpClient = new HttpClient();
            string urlrequest = "http://appws.kinematiclab.org/api/tests/" + item.Id;

            httpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                var response = await httpClient.GetAsync(new Uri(urlrequest));

                var resp = response.Content.ReadAsStringAsync().Result;

                var testObj = JsonConvert.DeserializeObject<TestClass>(resp);

                if (testObj.Id != null)
                {
                    await Navigation.PushAsync(new TestResults(testObj));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MENSAJE:  " + ex.Message);
            }
        }

        public async void DeleteAction(object sender, ItemTappedEventArgs e)
        {
            var s = await DisplayAlert("Alerta", "Está seguro que quiere eliminar este sujeto y todos sus tests?", "Si", "No");

            if (s)
            {
                deleteBtn.IsEnabled = false;

                HttpClient httpClient = new HttpClient();
                string urlrequest = "http://appws.kinematiclab.org/api/fellows/delete/" + fellowID;

                httpClient.Timeout = TimeSpan.FromSeconds(15);

                try
                {
                    var response = await httpClient.GetAsync(new Uri(urlrequest));

                    var resp = response.Content.ReadAsStringAsync().Result;

                    if (resp == "true")
                    {
                        await DisplayAlert("Kinematic Lab", "Sujeto eliminado correctamente", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Atención", "No hemos podido atender su solicitud, intentelo más tarde", "OK");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MENSAJE:  " + ex.Message);
                }
            }

            deleteBtn.IsEnabled = true;


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("FellowDetails");

            if (fellowID != null && fellowID != "0")
                GetFellow();
        }
    }


}
