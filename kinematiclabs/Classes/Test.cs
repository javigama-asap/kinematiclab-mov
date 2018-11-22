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

        public PatientClass patient { get; set; }
        public string video { get; set; }
        public string weight { get; set; }
        public string height { get; set; }
        public TypeTestClass type { get; set; }
        public string lever { get; set; }
        public string charge { get; set; }
        public List<PointClass> points { get; set; }
    }
}
