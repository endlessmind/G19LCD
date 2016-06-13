using G19LCDText_pages.VlcTestStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G19LCDText_pages
{
    class VlcStateItem
    {
        public enum VideoType
        {
            None,
            TVShort,
            TVLong,
            Movie
        }
        private DisplayObject mon;
        private string state;
        private string res = "0x0 (1:1)";
        private string title = "No media";
        private long length;
        private long time;
        private VideoType type;
        private bool full;


        public long CurrentPosition
        {
            get { return time;  }
            set { time = value; }
        }

        public bool isFullScreen
        {
            get
            {
                return full;
            }
            set
            {
                full = value;
            }
        }


        public long Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public VideoType PlaybackType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public DisplayObject Monitor
        {
            get
            {
                return mon;
            }
            set
            {
                mon = value;
            }
        }

        public string State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public string Resolution
        {
            get
            {
                return res;
            }
            set
            {
                res = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }
    }
}
