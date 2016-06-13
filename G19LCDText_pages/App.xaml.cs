using G19LCD;
using G19LCD.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Page1 pg1;
        Page2 pg2;
        LCD lcd;
        List<LcdPage> _pages = new List<LcdPage>();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            lcd = new LCD(1133, 49705);
            lcd.OpenDevice();

            pg1 = new Page1(lcd);
            pg2 = new Page2(lcd);

            _pages.Add(new LcdPageWpf { Element = pg1 });
            _pages.Add(new LcdPageWpf { Element = pg2 });

            lcd.Pages = _pages;
            lcd.CurrentPage = _pages[0];
            lcd.UpdatePage();


            MainWindow mw = new MainWindow();
            mw.setLCD(lcd);
            mw.Show();
        }
    }
}
