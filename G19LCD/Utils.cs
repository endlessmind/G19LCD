using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace G19LCD
{
    class Utils
    {
        static int G19_BUFFER_LEN = (320 * 240 * 2) + 512;
        static byte[] header = new byte[16] { 0x10, 0x0F, 0x00, 0x58, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x01, 0xEF, 0x00, 0x0F };

        public static byte[] convertToByte(Bitmap bmp)
        {
            byte[] lcd_buffer = new byte[G19_BUFFER_LEN];

            lcd_buffer[0] = 0x02;
            Array.Copy(header, lcd_buffer, header.Length);

            //Just filling the remaining header with nonsens data
            for (int i = 16; i < 256; i++) { lcd_buffer[i] = Convert.ToByte((char)i); }
            for (int i = 0; i < 256; i++) lcd_buffer[i + 256] = Convert.ToByte((char)i);


            //The actual image data
            int offset = 512;
            int count = 0;
            for (int x = 0; x < 320; ++x)
            {
                for (int y = 0; y < 240; ++y)
                {
                    Color col = bmp.GetPixel(x, y);
                    int green = col.G >> 2;
                    int blue = col.B;
                    int red = col.R;

                    byte firstByte = joinGreenAndBlue(green, blue);
                    lcd_buffer[count + offset] = firstByte;

                    byte secByte = joinRedAndGreen(red, green);
                    lcd_buffer[(count + 1) + offset] = secByte;
                    count += 2;
                }
            }
            return lcd_buffer;
        }

        public static byte[] convertToByte(BitmapSource bmp)
        {
            Bitmap b = BitmapImage2Bitmap(bmp);
            return convertToByte(b);
        }


        private static byte joinGreenAndBlue(int g, int b)
        {
            return (byte)((g << 5) | (b >> 3));
        }

        private static byte joinRedAndGreen(int r, int g)
        {
            return (byte)((r & 0xF8) | (g >> 3));
        }

        public static Bitmap BitmapImage2Bitmap(BitmapSource bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

    }
}
