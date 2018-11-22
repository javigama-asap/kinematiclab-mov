using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kinematiclabs
{
    public class FamilyTestClass
    {
        public FamilyTestClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("member")]
        public string Member { get; set; }

        [JsonProperty("typetests")]
        public List<TypeTestClass> TypeTests { get; set; }
    }
}
