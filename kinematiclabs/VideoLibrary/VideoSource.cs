using System;
using Xamarin.Forms;

namespace kinematiclabs
{
    public abstract class VideoSource : Element
    {
        public static VideoSource FromFile(string file)
        {
            return new FileVideoSource() { File = file };
        }

        public static VideoSource FromResource(string path)
        {
            return new ResourceVideoSource() { Path = path };
        }
    }
}
