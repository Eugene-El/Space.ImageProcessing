using System.Drawing;

namespace Space.ImageProcessing.Lib.ImageLoaders
{
    public class BitmapLoader
    {
        public Bitmap LoadImage(string path)
        {
            return new Bitmap(path);
        }
    }
}
