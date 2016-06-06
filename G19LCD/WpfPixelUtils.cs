using G19LCD;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static G19LCD.BitmapSourceHelper;

namespace G19LCD
{
    public class WpfPixelUtils
    {


        /// <summary>
        ///  Get pixel data from WPF Bitmap
        /// </summary>
        /// <param name="source">WPF Bitmap to get pixel data from</param>
        /// <returns>2 dimentional PixelColor-array with ARGB pixel data</returns>
        public static PixelColor[,] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            PixelColor[,] result = new PixelColor[width, height];
            var pixelBytes = new byte[height * width * 4];

            source.CopyPixels(result, width * 4, 0, 0);


            return result;
        }

    }
}
