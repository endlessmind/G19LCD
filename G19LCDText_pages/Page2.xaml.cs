using G19LCD;
using G19LCD.Pages;
using G19LCDText_pages.VlcTestStuff;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : UserControl
    {
        DispatcherTimer updateTimer;
        DispatcherTimer monitorTimer;
        Monitor mon;
        DisplayObject obj;
        LCD lcd;
        public Page2(LCD l)
        {
            lcd = l;
            InitializeComponent();

            mon = new Monitor();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            updateTimer.Tick += new EventHandler(updateTimer_Tick);

            monitorTimer = new DispatcherTimer();
            monitorTimer.Interval = new TimeSpan(0, 0, 1);
            monitorTimer.Tick += new EventHandler(MonitorTimer_Tick);


            mon.getMonitors();
            updateTimer.Start();
            monitorTimer.Start();
            obj = mon.getVLC();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        string GetWebPage(string address)
        {
            string responseText;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            String username = "";
            String password = "fisk";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Timeout = 1000;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                    {
                        responseText = responseStream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return responseText;
        }


        //This is made posible by enabeling the Lua Interface in VLC Player:
        // VLC Preferences -> Show settings: All -> Interfaces -> Main intefaces -> check the "Web" box
        //Then expand "Main interfaces" and go in to "Lua". Type something in the "Lua Interface" field and
        // set a password. Source directory should be *vlc install folder*\lua\http    and Directory index should be checked.
        //
        //
        //If you now open "http://127.0.0.1:8080/requests/status.xml" in your browser while VLC is playing,
        //you should be able to login by entering your password (leave username empty)


        void ParseXml(DisplayObject obj)
        {
            //Console.WriteLine();
            VlcStateItem vlc = new VlcStateItem();
            vlc.Monitor = obj;
            string data = GetWebPage("http://127.0.0.1:8080/requests/status.xml");
            if (data == null)
            {
                tbMonitor.Text = "No monitor";
                tbStatus.Text = String.Format("isOpen=False, PlayState={0}, isFullScreen={1}", "null", "False");
                tbTitle.Text = "No media";
                tbResAndAsp.Text = "0x0 (1:1)";
                tbType.Text = String.Format("{0} @ {1}", VlcStateItem.VideoType.None.ToString(), Utils.SecToTime(0));

                return;
            }
            XDocument objDoc = XDocument.Parse(data);

            var root = objDoc.Element("root");
            var cat = root.Element("information").Elements("category");
            long length = long.Parse(root.Element("length").Value);
            long time = long.Parse(root.Element("time").Value);
            bool fullScreen = bool.Parse(root.Element("fullscreen").Value.Replace("0", "False").Replace("1", "True"));
            string state = root.Element("state").Value;

            vlc.State = state.Replace(state[0].ToString(), state[0].ToString().ToUpper());
            vlc.Length = length;
            vlc.CurrentPosition = time;
            vlc.isFullScreen = fullScreen;

            if (length < Utils.tv_long_min)
            {
                vlc.PlaybackType = VlcStateItem.VideoType.TVShort;
            }
            if (length > Utils.tv_long_min && length < Utils.tv_long_max)
            {
                vlc.PlaybackType = VlcStateItem.VideoType.TVLong;
            }

            if (length > Utils.tv_long_max)
            {
                vlc.PlaybackType = VlcStateItem.VideoType.Movie;
            }

            // Console.WriteLine(state);
            foreach (var c in cat)
            {
                var info = c.Elements("info");
                foreach (var i in info)
                {
                    string name = i.Attribute("name").Value;
                    if (name.Equals("title"))
                    {
                        vlc.Title = i.Value.Replace("&#39;", "'");
                    }

                    if (name.Equals("filename"))
                    {
                        vlc.Title = i.Value.Replace("&#39;", "'");
                    }

                    if (name.Equals("showName"))
                    {
                        //It's has metadata telling us that it's a TV show. NICE!!!
                        if (length < Utils.tv_long_min)
                        {
                            vlc.PlaybackType = VlcStateItem.VideoType.TVShort;
                        }
                        else
                        {
                            vlc.PlaybackType = VlcStateItem.VideoType.TVLong;
                        }
                    }

                    if (name.Equals("Skärmupplösning") || name.Equals("Display resolution"))
                    {
                        string res = i.Value;
                        double width = int.Parse(res.Split('x')[0]);
                        double height = int.Parse(res.Split('x')[1]);
                        double aspect = Math.Round(width / height, 2);

                        //Console.WriteLine("Res: {0} ({1} calculated from: {2} )  ", res, "0:0", aspect);
                        foreach (DictionaryEntry de in Utils.aspectTable)
                        {
                            String asp = (string)de.Key;
                            string[] scales = (string[])de.Value;
                            foreach (string sc in scales)
                            {
                                if (aspect.ToString().Replace(',', '.') == sc)
                                {
                                    vlc.Resolution = String.Format("{0} ({1})", res, asp);
                                    // Console.WriteLine("Res: {0} ({1} calculated from: {2} )  ", res, asp, aspect);
                                }
                            }
                        }
                    }
                }
            }

            tbMonitor.Text = vlc.Monitor.Name;
            tbStatus.Text = String.Format("isOpen=True, {0} {1}", vlc.State, vlc.isFullScreen ? "in fullscreen" :"");
            tbTitle.Text = vlc.Title;
            tbResAndAsp.Text = vlc.Resolution;
            tbType.Text = String.Format("{0} @ {1}", vlc.PlaybackType.ToString(), Utils.SecToTime(vlc.Length));

            slider.Maximum = vlc.Length;
            slider.Value = vlc.CurrentPosition;

            lblPos.Content = Utils.formatSec(vlc.CurrentPosition);
            lblLenght.Content = Utils.formatSec(vlc.Length);
        }


        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (obj != null) //VLC is running.. time to get some data
            {
                ParseXml(obj);
            }
            else
            {
                tbMonitor.Text = "No monitor";
                tbStatus.Text = String.Format("isOpen=False, PlayState={0}, isFullScreen={1}", "null", "False");
                tbTitle.Text = "No media";
                tbResAndAsp.Text = "0x0 (1:1)";
                tbType.Text = String.Format("{0} @ {1}", VlcStateItem.VideoType.None.ToString(), Utils.SecToTime(0));
            }
            if (((LcdPageWpf)lcd.CurrentPage).Element == this)
                lcd.UpdatePage();
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            obj = mon.getVLC();
        }
    }
}
