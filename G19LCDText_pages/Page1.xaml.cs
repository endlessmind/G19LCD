using G19LCD;
using G19LCDText_pages.WeatherStuff;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : UserControl
    {
        DispatcherTimer update_timer;
        DispatcherTimer weatherTimer;
        YahooWeather yw;
        LCD lcd;
        WeatherResult wr;

        string titleBase = "Weather for: {0}, {1}";


        public Page1(LCD l)
        {
            lcd = l;
            InitializeComponent();

            update_timer = new DispatcherTimer();
            update_timer.Interval = new TimeSpan(0, 0, 0,0,200);
            update_timer.Tick += new EventHandler(updateTimer_Tick);

            weatherTimer = new DispatcherTimer();
            weatherTimer.Interval = new TimeSpan(0, 10, 0);
            weatherTimer.Tick += new EventHandler(weatherTimer_Tick);

            update_timer.Start();
            weatherTimer.Start();

            yw = new YahooWeather();
            wr = yw.getWeather("Trelleborg", "SE", false);
            
            imgWeather.Source = setWeatherImage(wr.Condition.Code);
            updateLables();
            setForecast();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void updateLables()
        {
            lblTemp.Content = wr.Condition.Temp;
            lblWindSpeed.Content = wr.Condition.WindSpeed;
            lblHumid.Content = wr.Condition.Humidity;
            lblLocation.Content = String.Format(titleBase, wr.City, wr.Province);
        }

        private void weatherTimer_Tick(object sender, EventArgs e)
        {
            wr = yw.getWeather("Trelleborg", "SE", false);
            imgWeather.Source = setWeatherImage(wr.Condition.Code);
            updateLables();
            setForecast();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            lcd.UpdatePage();
        }

        private void setForecast()
        {
            if (wr.FCast.Count >= 1)
            {
                imgFore1.Source = setWeatherImage(wr.FCast[0].Code);
                lblFore1.Content = wr.FCast[0].HighTemp;
                lblFore1Day.Content = wr.FCast[0].Day;
            }

            if (wr.FCast.Count >= 2)
            {
                imgFore2.Source = setWeatherImage(wr.FCast[1].Code);
                lblFore2.Content = wr.FCast[1].HighTemp;
                lblFore2Day.Content = wr.FCast[1].Day;
            }

            if (wr.FCast.Count >= 3)
            {
                imgFore3.Source = setWeatherImage(wr.FCast[2].Code);
                lblFore3.Content = wr.FCast[2].HighTemp;
                lblFore3Day.Content = wr.FCast[2].Day;
            }
        }

        private BitmapImage setWeatherImage(string code)
        {
            Weather w = wr.Condition;
            BitmapImage bmp;
            if (code == "26" || code == "27" || code == "28") {
                //Cloudy
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/cloudy.png", UriKind.Absolute));
            } else if (code == "29" || code == "30") {
                //Cloudy
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/partly_cloudy.png", UriKind.Absolute));
            } else if (code == "9" || code == "11" || code == "12") {
                //Rain
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/rain.png", UriKind.Absolute));
            } else if (code == "14" || code == "15" || code == "16" || code == "41" || code == "42" || code == "43" || code == "46") {
                //Snow
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/snow.png", UriKind.Absolute));
            } else if (code == "3" || code == "37" || code == "38" || code == "39" || code == "47") {
                //Thunder
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/thunder.png", UriKind.Absolute));
            } else if (code == "45") {
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/thunderstorm.png", UriKind.Absolute));
            } else if (code == "32" || code == "36") {
                //Sunny
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/sunny.png", UriKind.Absolute));
            } else if (code == "5") {
                //Snow and rain mix
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/snow_and_rain.png", UriKind.Absolute));
            } else if (code == "9" || code == "40") {
                //Drizzle
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/drizzle.png", UriKind.Absolute));
            } else {
                //Other...
                bmp = new BitmapImage(new Uri("pack://application:,,,/G19LCDText_pages;component/Resources/mostly_cloudy.png", UriKind.Absolute));
            }

            
            return bmp;
        }

    }
}
