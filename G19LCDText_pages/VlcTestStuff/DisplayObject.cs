using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G19LCDText_pages.VlcTestStuff
{
    class DisplayObject
    {
        private string name;
        private string displayName;
        private string gfx;


        public string GFX
        {
            get
            {
                return gfx;
            }
            set
            {
                gfx = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
