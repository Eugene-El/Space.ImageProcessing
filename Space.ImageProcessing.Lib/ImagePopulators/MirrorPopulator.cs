using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Space.ImageProcessing.Lib.ImagePopulators
{
    public class MirrorPopulator
    {
        public Bitmap Populate(Bitmap sourceBitmap)
        {
            unsafe
            {
                BitmapData bitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width,
                    sourceBitmap.Height), ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);

                int widthInPixels = bitmapData.Width;
                int heightInPixels = bitmapData.Height;

                int bytesPerPixel = Image.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;

                int widthInBytes = widthInPixels * bytesPerPixel;
                int heightInBytes = heightInPixels * bytesPerPixel;

                Bitmap outputBitmap = new Bitmap(widthInPixels, heightInPixels, bitmapData.PixelFormat);
                BitmapData outputBitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.WriteOnly, sourceBitmap.PixelFormat);

                byte* sourcePointer = (byte*)bitmapData.Scan0;
                byte* outputPointer = (byte*)outputBitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* sourceLine = sourcePointer + (y * bitmapData.Stride);
                    byte* outputLine = outputPointer + (y * outputBitmapData.Stride);

                    for (int x = 0; x < widthInPixels; x++)
                    {
                        var currentPosition = x * bytesPerPixel;
                        var revercedPosition = widthInBytes - bytesPerPixel - x * bytesPerPixel;

                        outputLine[revercedPosition] = sourceLine[currentPosition];
                        outputLine[revercedPosition + 1] = sourceLine[currentPosition + 1];
                        outputLine[revercedPosition + 2] = sourceLine[currentPosition + 2];
                    }
                });
                sourceBitmap.UnlockBits(bitmapData);
                outputBitmap.UnlockBits(outputBitmapData);

                return outputBitmap;
            }
        }
    }
}
