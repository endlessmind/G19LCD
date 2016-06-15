using G19LCD;
using G19LCD.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        DispatcherTimer update_timer;
        Page1 pg1;
        Page2 pg2;
        LCD lcd;
        List<LcdPage> _pages = new List<LcdPage>();
        int nextPage = 1;
        bool oneTurn = false;
        List<Bitmap> images = new List<Bitmap>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            update_timer = new DispatcherTimer();
            update_timer.Interval = new TimeSpan(0, 0, 10);
            update_timer.Tick += new EventHandler(updateTimer_Tick);
            update_timer.Start();

            lcd = new LCD(1133, 49705);
            lcd.OpenDevice();
            //lcd.captureFrames = true;

            pg1 = new Page1(lcd);
            pg2 = new Page2(lcd);

            _pages.Add(new LcdPageWpf { Element = pg1 });
            _pages.Add(new LcdPageWpf { Element = pg2 });

            lcd.Pages = _pages;
            lcd.CurrentPage = _pages[0];
            lcd.UpdatePage();


            //MainWindow mw = new MainWindow();
            //mw.setLCD(lcd);
            //mw.Show();

        }

        private void createGif()
        {
            System.Windows.Media.Imaging.GifBitmapEncoder gEnc = new GifBitmapEncoder();

            foreach (System.Drawing.Bitmap bmpImage in images)
            {
                var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpImage.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                gEnc.Frames.Add(BitmapFrame.Create(src));
            }
            using (FileStream fs = new FileStream("anim.gif", FileMode.Create))
            {
                gEnc.Save(fs);
                
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            //if (nextPage == 1 && oneTurn)
            //{
            //    lcd.captureFrames = false;
            //    images = lcd.getImages;
            //    createGif();
            //}
            lcd.CurrentPageIndex = nextPage;

            nextPage++;
            if (nextPage >= _pages.Count)
                nextPage = 0;

            //if (nextPage == 1)
            //    oneTurn = true;

        }
    }
}
