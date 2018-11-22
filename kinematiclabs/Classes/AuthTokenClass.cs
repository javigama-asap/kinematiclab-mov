using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematiclabs
{
    public class AuthTokenClass
    {
        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("expires_in")]
        public string expires_in { get; set; }

        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }

        [JsonProperty("resetpassword_token")]
        public string resetpassword_token { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("error")]
        public string error { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        public AuthTokenClass()
        {
        }
    }
}
