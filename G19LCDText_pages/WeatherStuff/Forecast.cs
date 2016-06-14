using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G19LCDText_pages.WeatherStuff
{
    class Forecast : Weather
    {
        public string Day
        {
            get; set;
        }

        public string HighTemp
        {
            get; set;
        }

        public string LowTemp
        {
            get; set;
        }


    }
}
