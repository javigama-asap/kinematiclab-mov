using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kinematiclabs
{
    public class PointClass
    {
        public PointClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("test")]
        public TestClass Test { get; set; }
    }


}
