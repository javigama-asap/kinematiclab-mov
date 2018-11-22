/*SOS Perito es un software propiedad de Soluciones Perin S.C.P.*/

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
    public static class Client
    {
        public static string baseurl = "http://test.sosperito.es/";

        public async static Task<string> GetClient(string id)
        {
            //var data = new Dictionary<string, string>();

            //data["id"] = id;

            HttpClient httpClient = new HttpClient();
            string urlrequest = baseurl + "api/clients/show/" + id;

            //var content = new FormUrlEncodedContent(data);

            httpClient.Timeout = TimeSpan.FromSeconds(15);

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

    }
}
