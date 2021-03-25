using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Space.ImageProcessing.Lib.ImagePopulators
{
    public class RotationPopulator
    {
        public List<Bitmap> Populate(Bitmap sourceBitmap)
        {
            var outputBitmaps = new List<Bitmap>();

            unsafe
            {
                BitmapData bitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width,
                    sourceBitmap.Height), ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);

                int widthInPixels = bitmapData.Width;
                int heightInPixels = bitmapData.Height;

                int bytesPerPixel = Image.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;

                int widthInBytes = widthInPixels * bytesPerPixel;
                int heightInBytes = heightInPixels * bytesPerPixel;

                BitmapData[] bitmapDatas = new BitmapData[3];
                byte*[] pointers = new byte*[3];
                for (int i = 0; i < 3; i++)
                {
                    Bitmap currentBitmap = i == 0 ? new Bitmap(widthInPixels, heightInPixels, bitmapData.PixelFormat)
                        : new Bitmap(heightInPixels, widthInPixels, bitmapData.PixelFormat);
                    bitmapDatas[i] = currentBitmap.LockBits(new Rectangle(0, 0, currentBitmap.Width, currentBitmap.Height), ImageLockMode.WriteOnly, sourceBitmap.PixelFormat);
                    pointers[i] = (byte*)bitmapDatas[i].Scan0;
                    outputBitmaps.Add(currentBitmap);
                }
                int outputHeightForVerticals = bitmapDatas[1].Stride;

                byte* sourcePointer = (byte*)bitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = sourcePointer + (y * bitmapData.Stride);

                    byte* output1Line = pointers[0] + ((heightInPixels - y - 1) * bitmapData.Stride);
                    int output2bytesOffset = (heightInPixels - 1 - y) * bytesPerPixel;
                    int output3bytesOffset = y * bytesPerPixel;

                    for (int x = 0; x < widthInPixels; x++)
                    {
                        var currentPosition = x * bytesPerPixel;

                        byte currentBlue = currentLine[currentPosition];
                        byte currentGreen = currentLine[currentPosition + 1];
                        byte currentRed = currentLine[currentPosition + 2];

                        output1Line[widthInBytes - currentPosition - 3] = currentBlue;
                        output1Line[widthInBytes - currentPosition - 2] = currentGreen;
                        output1Line[widthInBytes - currentPosition - 1] = currentRed;

                        var output2 = pointers[1] + outputHeightForVerticals * x + output2bytesOffset;
                        output2[0] = currentBlue;
                        output2[1] = currentGreen;
                        output2[2] = currentRed;

                        var output3 = pointers[2] + outputHeightForVerticals * (widthInPixels - 1 - x) + output3bytesOffset;
                        output3[0] = currentBlue;
                        output3[1] = currentGreen;
                        output3[2] = currentRed;
                    }
                });
                sourceBitmap.UnlockBits(bitmapData);
                for (int i = 0; i < bitmapDatas.Length; i++)
                    outputBitmaps[i].UnlockBits(bitmapDatas[i]);
            }

            return outputBitmaps;
        }
    }
}
