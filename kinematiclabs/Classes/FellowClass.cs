using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kinematiclabs
{
    public class FellowClass
    {
        public FellowClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        [JsonProperty("user")]
        public UserClass User { get; set; }

        [JsonProperty("tests")]
        public List<TestClass> Tests { get; set; }

		public string FullName {
            get
            {
				return Name + " " + Lastname;
            }
		}
    }
}
