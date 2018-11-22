using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kinematiclabs
{
    public class TypeTestClass
    {
        public TypeTestClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("segment")]
        public string Segment { get; set; }

        [JsonProperty("haslaterality")]
        public string Haslaterality { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("weightpercent")]
        public string WeightPerCent { get; set; }

        [JsonProperty("heightpercent")]
        public string HeightPerCent { get; set; }

        [JsonProperty("gif")]
        public string Gif { get; set; }

        [JsonProperty("family")]
        public TypeTestClass FamilyTest { get; set; }
        
        [JsonProperty("tests")]
        public List<TestClass> Tests { get; set; }
    }
}
