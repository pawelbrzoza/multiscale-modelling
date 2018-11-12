using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace grain_growth.Helpers
{
    public class Converters
    {
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
             
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static int StringToInt(string intString)
        {
            return (Int32.TryParse(intString, out int i) ? i : 0);
        }
    }
}