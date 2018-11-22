using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace kinematiclabs
{
    public class TestClass
    {
        public TestClass()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }

        [JsonProperty("charge")]
        public string Charge { get; set; }

        [JsonProperty("leftright")]
        public string Leftright { get; set; }

        [JsonProperty("testdate")]
        public DateTime Testdate { get; set; }

        [JsonProperty("fellow")]
        public FellowClass Fellow { get; set; }

        [JsonProperty("typetest")]
        public TypeTestClass TypeTest { get; set; }
        
        public string Video { get; set; }

        [JsonProperty("points")]
        public List<PointClass> Points { get; set; }

        public string TestName
        {
            get
            {
                return "Test nº " + Id;
            }
        }
    }
}
