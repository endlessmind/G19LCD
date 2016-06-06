using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace G19LCDTest_WPF
{
    class Utils
    {

        public static BitmapImage ConvertBmpToBmpImg(System.Drawing.Bitmap bmp)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static RenderTargetBitmap GetImage(Window w)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(320, 240, 96d, 96d, PixelFormats.Default);
            bitmap.Render(w);

            //var bitmapImage = new BitmapImage();
            //var bitmapEncoder = new PngBitmapEncoder();
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));

            //using (var stream = new MemoryStream())
            //{
            //    bitmapEncoder.Save(stream);
            //    stream.Seek(0, SeekOrigin.Begin);

            //    bitmapImage.BeginInit();
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.StreamSource = stream;
            //    bitmapImage.EndInit();
            //}
       
            return bitmap;
        }

    }
}
