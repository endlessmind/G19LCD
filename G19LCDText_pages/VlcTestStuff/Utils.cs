using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G19LCDText_pages.VlcTestStuff
{
    class Utils
    {
        static string[] t_21_9 = new string[] { "2.21", "2.22", "2.23", "2.3", "2.31", "2.32", "2.33", "2.34", "2.35", "2.36", "2.37", "2.38", "2.39", "2.4", "2.41", "2.42", "2.43", "2.44", "2.45" };
        static string[] t_16_10 = new string[] { "1.58", "1.59", "1.6", "1.61", "1.62", "1.63" };
        static string[] t_16_9 = new string[] { "1.25", "1.73", "1.74", "1.75", "1.76", "1.77", "1.78", "1.79", "1.8", "1.81", "1.82", "1.83", "1.84", "1.85", "1.86", "1.87", "1.88", "1.89" };
        static string[] t_4_3 = new string[] { "1.3", "1.31", "1.32", "1.33", "1.34", "1.35", "1.36", "1.37", "1.38", "1.39", "1.4", "1.41", "1.42", "1.43" };

        public static long tv_short_max = 1600;
        public static long tv_long_max = 2595;
        public static long tv_long_min = 1601;

        public static Hashtable aspectTable = new Hashtable()
        {
            {"4:3", t_4_3},
            {"16:9", t_16_9},
            {"16:10", t_16_10},
            {"21:9", t_21_9},
        };


        public static string SecToTime(long sec)
        {
            string minStr = ((double)sec / 60d).ToString();
            int min = int.Parse(minStr.Substring(0, minStr.Contains(',') ? minStr.IndexOf(',') : minStr.Length));
            int mSec = (int)sec - (min * 60);

            return String.Format("{0}m {1}s ({2}sec)", min, mSec, sec); ;
        }

        public static string formatSec(long sec)
        {
            string minStr = ((double)sec / 60d).ToString();
            int min = int.Parse(minStr.Substring(0, minStr.Contains(',') ? minStr.IndexOf(',') : minStr.Length));
            int mSec = (int)sec - (min * 60);

            

            return String.Format("{0}:{1}", (min < 10 ? "0" + min : "" + min), (mSec < 10 ? "0" + mSec : "" + mSec)); ;
        }
    }
}
