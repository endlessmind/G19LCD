using G19LCD;
using G19LCD.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : UserControl
    {
        DispatcherTimer update_timer;
        int count = 0;
        LCD lcd;
        public Page1(LCD l)
        {
            lcd = l;
            InitializeComponent();

            update_timer = new DispatcherTimer();
            update_timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            update_timer.Tick += new EventHandler(updateTimer_Tick);
            update_timer.Start();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void updateTimer_Tick(object sender, EventArgs e)
        {
            label1.Content = DateTime.Now.ToLongTimeString();
            slider.Value = count;
            count++;

            if (count > 240)
                count = 0;

            if (((LcdPageWpf)lcd.CurrentPage).Element == this)
                lcd.UpdatePage();


            Console.WriteLine(count);
        }

    }
}
