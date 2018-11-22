using Newtonsoft.Json;
using Plugin.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace kinematiclabs
{
    public partial class App : Application
    {
        public static NavigationPage MyNavigationPage;

        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }

        public App()
        {
            InitializeComponent();

            var account = AccountStore.Create().FindAccountsForService("Kinematic").FirstOrDefault();
            if (account != null)
            {
                var username = account.Username;
                var password = account.Properties["password"];

                var access_token = !account.Properties.ContainsKey("access_token") ? "" : account.Properties["access_token"];
                
                Task.Run(() => getUserData(username, password, access_token)).Wait();
            }
            else
            {
                
                MyNavigationPage = new NavigationPage(new Login())
                {
                    Icon = "logo.png",
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#009bcd")

                };
                MainPage = MyNavigationPage;
            }

        }

        public async Task getUserData(string email, string password, string access_token)
        {

            var resp = await Functions.Authentication.GetUser(access_token);

            if (resp == "falsehttp")
            {
                MyNavigationPage = new NavigationPage(new Login())
                {
                    Icon = "logo.png",
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#009bcd")

                };
                MainPage = MyNavigationPage;
            }
            else
            {
                var userObj = JsonConvert.DeserializeObject<UserClass>(resp);
                if (!string.IsNullOrEmpty(userObj.Id))
                {
                    Application.Current.Properties["user"] = userObj;
                    await Application.Current.SavePropertiesAsync();

                    Functions.Authentication.SaveCredentials(email, password, access_token);

                    MyNavigationPage = new NavigationPage(new Home())
                    {
                        Icon = "logo.png",
                        BarTextColor = Color.White,
                        BarBackgroundColor = Color.FromHex("#009bcd")

                    };
                    MainPage = MyNavigationPage;

                }
                else
                {
                    MyNavigationPage = new NavigationPage(new Login())
                    {
                        Icon = "logo.png",
                        BarTextColor = Color.White,
                        BarBackgroundColor = Color.FromHex("#009bcd")

                    };
                    MainPage = MyNavigationPage;


                }

            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
