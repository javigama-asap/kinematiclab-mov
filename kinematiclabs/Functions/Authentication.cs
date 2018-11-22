using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Forms;
using System.Net.Http.Headers;
using Xamarin.Auth;

namespace kinematiclabs.Functions
{
    public static class Authentication
    {
        public static string baseurl = "http://appws.kinematiclab.org/api/";

        public async static Task<string> GetAuthorization(string email, string password)
        {
            var data = new Dictionary<string, string>();

            data["email"] = email;
            data["password"] = password;
            
            HttpClient httpClient = new HttpClient();
            string urlrequest = baseurl + "login";

            var content = new FormUrlEncodedContent(data);

            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var resp = await httpClient.PostAsync(new Uri(urlrequest), content);
                return resp.Content.ReadAsStringAsync().Result;

            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
                return "falsehttp";
            }
        }

        public async static Task<string> GetUser(string access_token = null)
        {
            string myauthToken = access_token;
            string urlrequest = baseurl + "user";
            if (Application.Current.Properties.ContainsKey("AuthToken"))
                myauthToken = Application.Current.Properties["AuthToken"] as string;

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", myauthToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var resp = await httpClient.GetAsync(new Uri(urlrequest));
                return resp.Content.ReadAsStringAsync().Result;

            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
                return "falsehttp";
            }
        }

        public async static Task<string> SendResetLink(string email)
        {
            var data = new Dictionary<string, string>();

            data["email"] = email;

            HttpClient httpClient = new HttpClient();
            string urlrequest = baseurl + "password/email";

            var content = new FormUrlEncodedContent(data);

            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var resp = await httpClient.PostAsync(new Uri(urlrequest), content);
                return resp.Content.ReadAsStringAsync().Result;

            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
                return "falsehttp";
            }
        }

        public async static Task<string> ChangePassword(string email, string token, string newpassword)
        {
            var data = new Dictionary<string, string>();

            data["email"] = email;
            data["token"] = token;
            data["password"] = newpassword;
            data["password_confirmation"] = newpassword;

            HttpClient httpClient = new HttpClient();
            string urlrequest = baseurl + "password/reset";

            var content = new FormUrlEncodedContent(data);

            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var resp = await httpClient.PostAsync(new Uri(urlrequest), content);
                return resp.Content.ReadAsStringAsync().Result;

            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
                return "falsehttp";
            }
        }

        public static void SaveCredentials(string email, string password, string access_token)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(access_token))
            {
                Account account = new Account
                {
                    Username = email
                };
                account.Properties.Add("password", password);
                account.Properties.Add("access_token", access_token);
                AccountStore.Create().Save(account, "Kinematic");
            }
        }
    }
}
