using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace G19LCD.Transactions
{
    public class FadeTransaction : Transaction
    {

        public FadeTransaction(LCD lcd) : base(lcd)
        {
        }

        public override void start(Bitmap current, Bitmap next) {
            this.currentImage = current;
            this.nextImage = next;

            //Thread drawT = new Thread(new ThreadStart(fadeLoop));
            //drawT.Start();
            fadeLoop();
            //Transaction done
            OnTransActionComplete(new EventArgs());

            currentImage.Dispose();
            next.Dispose();
        }

        private void fadeLoop()
        {
            bool done = false;
            bool dark = false;
            int alpha = 0;

            while (!done)
            {
                Thread.Sleep(16);
                if (!dark) {
                    if (alpha >= 255) {
                        dark = true;
                        alpha -= 45;
                    } else {
                        alpha += 45;
                    }
                } else {
                    if (alpha <= 0)
                        done = true;
                    else
                        alpha -= 45;
                }

                if (alpha < 0)
                    alpha = 0;

                if (alpha > 255)
                    alpha = 255;

                int h, w;

                if (!dark) {
                    h = currentImage.Height;
                    w = currentImage.Width;
                } else {
                    h = nextImage.Height;
                    w = nextImage.Width;
                }

                // Bitmap current = !dark ? images[currentImage] : images[nextBmp];

                Brush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                //create an empty Bitmap image
                Bitmap bmp = new Bitmap(w, h);

                //turn the Bitmap into a Graphics object
                Graphics gfx = Graphics.FromImage(bmp);
                //set the InterpolationMode to HighQualityBicubic so to ensure a high
                //quality image once it is transformed to the specified size
              //  gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //now draw our new image onto the graphics object
                gfx.DrawImage(!dark ? currentImage : nextImage, new Point(0, 0));
                //Then we overlay a partial transparent color
                gfx.FillRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                //dispose of our Graphics object
                gfx.Dispose();
                // setBitmap(bmp);
                lcd_instance.UpdateScreen(bmp);

                brush.Dispose();
                bmp.Dispose();

            }
        }

    }
}
