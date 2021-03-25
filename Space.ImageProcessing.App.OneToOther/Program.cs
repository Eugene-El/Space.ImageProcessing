using Space.ImageProcessing.Lib.ImageLoaders;
using Space.ImageProcessing.Lib.ImagePopulators;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace Space.ImageProcessing.App.OneToOther
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("App started");

                BitmapLoader bitmapLoader = new BitmapLoader();
                RotationPopulator rotationPopulator = new RotationPopulator();

                Console.WriteLine("Loading image");
                var bitmap = bitmapLoader.LoadImage("Images/heic0910e.jpg");

                Console.WriteLine("Rotating images");
                var list = rotationPopulator.Populate(bitmap);

                Console.WriteLine("Saving images in PNG");
                Directory.CreateDirectory("GeneratedContent");
                for (int i = 0; i < list.Count; i++)
                    list[i].Save("GeneratedContent/" + i + ".png", ImageFormat.Png);

                Console.WriteLine("App finished");
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.ReadLine();
            }
        }
    }
}
