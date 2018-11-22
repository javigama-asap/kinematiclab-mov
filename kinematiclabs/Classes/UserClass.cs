using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kinematiclabs
{
    public class UserClass
    {
        public UserClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("cp")]
        public string Cp { get; set; }

        [JsonProperty("api_token")]
        public string Api_token { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fellows")]
        public List<TestClass> Fellows { get; set; }

        [JsonProperty("errors")]
        public UserErrorClass Errors { get; set; }

    }

    public class UserErrorClass
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
