using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace G19LCDText_pages.VlcTestStuff
{
    class Monitor
    {
        public Monitor()
        {

        }

        List<DisplayObject> monitors = new List<DisplayObject>();

        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        static extern uint GetWindowText(uint hWnd, StringBuilder text, uint count);


        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            Active = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            AttachedToDesktop = 0x5,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }


        public DisplayObject getVLC()
        {
            IntPtr intWindowHandle = IntPtr.Zero;
            Process[] p = Process.GetProcessesByName("vlc");
            if (p.Count() > 0)
            {
                intWindowHandle = p[0].MainWindowHandle;
            }
            else
            {
                return null; //VLC is not running.. return null
            }
            Screen scr = Screen.FromHandle(intWindowHandle);
            
            foreach (DisplayObject obj in monitors)
            {
                if (obj.DisplayName == scr.DeviceName)
                {
                    return obj; //VLC is present on this screen
                }
            }

            return null;
        }

        public void getMonitors()
        {
            monitors.Clear();
            uint i = 0;

            DISPLAY_DEVICE disp = new DISPLAY_DEVICE();
            disp.cb = Marshal.SizeOf(disp);

            while (EnumDisplayDevices(null, i, ref disp, 0))
            {
                if (disp.StateFlags == DisplayDeviceStateFlags.Active || disp.StateFlags == DisplayDeviceStateFlags.AttachedToDesktop)
                {
                    string gfx = disp.DeviceString;
                    string displayName = disp.DeviceName;

                    EnumDisplayDevices(disp.DeviceName, 0, ref disp, 0);
                    string mon = disp.DeviceString;

                    DisplayObject d = new DisplayObject();
                    d.DisplayName = displayName;
                    d.Name = mon;
                    d.GFX = gfx;

                    monitors.Add(d);
                }
                i++;
            }

        }
    }
}
