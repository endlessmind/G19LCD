using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G19LCDText_pages.WeatherStuff
{
    class WeatherResult
    {
        public string City
        {
            get; set;
        }

        public string Province
        {
            get; set;
        }

        public Weather Condition
        {
            get; set;
        }

        public List<Forecast> FCast
        {
            get; set;
        }
    }
}
