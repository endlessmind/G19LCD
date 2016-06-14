using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace G19LCDText_pages.WeatherStuff
{
    class YahooWeather
    {
        static string baseUrl = "https://query.yahooapis.com/v1/public/yql?q={0}&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        static string query = "select * from weather.forecast where woeid in(select woeid from geo.places(1) where text=\"{0},{1}\")";

        static string CelciusString = "°C";
        static string FahrenheitString = "°F";

        public WeatherResult getWeather(string city, string province, bool fahrenheit)
        {
            if (city == null || province == null || city.Length < 1 || province.Length < 1)
                throw new ArgumentException("You're missing an argument");


            string url = string.Format(baseUrl, String.Format(query, city, province));
            var inStream = WebRequest.Create(url).GetResponse().GetResponseStream();
            string content = new StreamReader(inStream).ReadToEnd();

            var obj = JObject.Parse(content);

            WeatherResult wr = new WeatherResult();
            wr.City = city;
            wr.Province = province;

            JObject itm = (JObject)obj["query"]["results"]["channel"];
            JObject condition = (JObject)itm["item"]["condition"];
            JArray arr = (JArray)itm["item"]["forecast"];
            JObject windObj = (JObject)itm["wind"];


            Weather wCon = new Weather();
            wCon.Code = condition["code"].ToString();
            wCon.Date = condition["date"].ToString();
            if (fahrenheit)
                wCon.Temp = condition["temp"].ToString() + FahrenheitString;
            else
                wCon.Temp = ConvertF2C(Double.Parse(condition["temp"].ToString())).ToString();

            wCon.Text = condition["text"].ToString();
            wCon.WindFeelTemp = ConvertF2C(Double.Parse(windObj["chill"].ToString())).ToString();
            wCon.WindSpeed = ConvertMPH2MPS(Double.Parse(windObj["speed"].ToString()));
            wCon.WindDirection = windObj["direction"].ToString();
            wCon.Humidity = itm["atmosphere"]["humidity"].ToString() + "%";

            wr.Condition = wCon;

            Console.WriteLine(condition["code"].ToString());
            Console.WriteLine(condition["date"].ToString());
            Console.WriteLine(condition["temp"].ToString());
            Console.WriteLine(ConvertF2C(Double.Parse(condition["temp"].ToString())).ToString());
            Console.WriteLine(condition["text"].ToString());

            List<Forecast> foreC = new List<Forecast>();
            foreach (var c in arr)
            {
                Forecast fc = new Forecast();
                fc.Code = c["code"].ToString();
                fc.Date = c["date"].ToString();
                fc.Day = c["day"].ToString();
                if (fahrenheit)
                {
                    fc.HighTemp = c["high"].ToString() + FahrenheitString;
                    fc.LowTemp = c["low"].ToString() + FahrenheitString;
                } else
                {
                    fc.HighTemp = ConvertF2C(Double.Parse(c["high"].ToString())).ToString(); c["high"].ToString();
                    fc.LowTemp = ConvertF2C(Double.Parse(c["low"].ToString())).ToString(); c["low"].ToString();
                }
                fc.Text = c["text"].ToString();
                foreC.Add(fc);
            }

            wr.FCast = foreC;
            Console.WriteLine(ConvertMPH2MPS(21.74799));
            return wr;
        }

        private string ConvertF2C(double valueToConvert)
        {
            return (int)Math.Round(5.0 / 9.0 * (valueToConvert - 32), 2) + CelciusString;
        }

        private string ConvertMPH2MPS(double value)
        {
            //Miles per hour to Meter per second
            double kph = value * 1.609344;
            double mps = (int)(kph * 1000) / 3600;
            return mps + "m/s";
        }

    }
}
