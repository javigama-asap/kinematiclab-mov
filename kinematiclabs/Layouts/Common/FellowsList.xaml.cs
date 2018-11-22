using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;
using System.Globalization;
using Plugin.GoogleAnalytics;

namespace kinematiclabs
{
    public partial class FellowsList : ContentPage
    {
        public UserClass userObj;
        public ObservableCollection<FellowClass> fellowList { get; set; }
        public List<FellowClass> allfellows { get; set; }

        public FellowsList()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            userObj = Application.Current.Properties["user"] as UserClass;

            allfellows = new List<FellowClass>();
        }

        public async void getFellows()
        {
            HttpClient httpClient = new HttpClient();
            string urlrequest = "http://appws.kinematiclab.org/api/fellows/user/" + userObj.Id;
            
            httpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                var response = await httpClient.GetAsync(new Uri(urlrequest));
                var resp = response.Content.ReadAsStringAsync().Result;

                allfellows = JsonConvert.DeserializeObject<List<FellowClass>>(resp);

                fellowList = new ObservableCollection<FellowClass>(allfellows);

                if (fellowList.Count > 0)
                {
                    FellowsListView.ItemsSource = fellowList;
                    FellowsListView.IsVisible = true;
                    FellowsListView.IsEnabled = true;
                    NoResults.IsVisible = false;
                }
                else
                {
                    FellowsListView.IsVisible = false;
                    FellowsListView.IsEnabled = false;
                    NoResults.IsVisible = true;
                }



            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
            }
        }

        private async void OpenTarget(object sender, ItemTappedEventArgs e)
        {
            var item = FellowsListView.SelectedItem as FellowClass;
            await Navigation.PushAsync(new FellowDetails(item.Id));
        }

        public void homeAction(object sender, EventArgs ea)
        {
			Application.Current.MainPage = new NavigationPage(new Home());
        }

        public async void backAction(object sender, EventArgs ea)
        {
			await Navigation.PopAsync();
        }

        public void SearchOnApp(object sender, EventArgs ea)
        {
            var query = searchBar.Text;

            if (!string.IsNullOrEmpty(query))
            {


                var filteredfellows = fellowList.Where(x => CultureInfo.InvariantCulture.CompareInfo.IndexOf(x.FullName.ToLower(), query.ToLower(), CompareOptions.IgnoreNonSpace) > -1);

                FellowsListView.ItemsSource = filteredfellows;
            }
            else
            {
                FellowsListView.ItemsSource = ""; 
                FellowsListView.ItemsSource = fellowList;
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("FellowsList");

            getFellows();         
        }
    }
}
