using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using static G19LCD.BitmapSourceHelper;
using static G19LCD.WpfPixelUtils;

namespace G19LCD
{
    public class LCD
    {

        int G19_BUFFER_LEN = (320 * 240 * 2) + 512;
        static byte[] header = new byte[16] { 0x10, 0x0F, 0x00, 0x58, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x01, 0xEF, 0x00, 0x0F };

        UsbEndpointWriter writer;
        UsbEndpointReader reader;
        ErrorCode ec = ErrorCode.None;
        IUsbDevice wholeUsbDevice;

        public static UsbDevice MyUsbDevice;
        public UsbDeviceFinder MyUsbFinder;

        /// <summary>
        ///  Create a new instance of the LCD class
        /// </summary>
        /// <param name="vid">Device vendor ID</param>
        /// <param name="pid">Device product ID</param>
        public LCD(int vid, int pid)
        {
            MyUsbFinder = new UsbDeviceFinder(vid, pid);

        }

        /// <summary>
        /// Open connection to the device
        /// </summary>
        public void OpenDevice()
        {
            MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

            if (MyUsbDevice == null) throw new Exception("Device Not Found.");
            // If this is a "whole" usb device (libusb-win32, linux libusb)
            // it will have an IUsbDevice interface. If not (WinUSB) the 
            // variable will be null indicating this is an interface of a 
            // device.

            wholeUsbDevice = MyUsbDevice as IUsbDevice;


            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);
                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(0);
            }
            // open read endpoint 1.
            reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            // open write endpoint 1.
            writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);


        }

        /// <summary>
        /// Close the connection to the device
        /// </summary>
        public void CloseDevice()
        {
            if (MyUsbDevice != null)
            {
                if (MyUsbDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }

                    MyUsbDevice.Close();
                }
                MyUsbDevice = null;

                // Free usb resources
                UsbDevice.Exit();

            }
        }

        /// <summary>
        ///  Update the screen with the bitmap
        /// </summary>
        /// <param name="bmp">WinForm bitmap</param>
        public void UpdateScreen(Bitmap bmp)
        {
            byte[] lcd_buffer = new byte[G19_BUFFER_LEN];

            lcd_buffer[0] = 0x02;
            Array.Copy(header, lcd_buffer, header.Length);

            //Just filling the remaining header with nonsens data
            for (int i = 16; i < 256; i++) { lcd_buffer[i] = Convert.ToByte((char)i); }
            for (int i = 0; i < 256; i++) lcd_buffer[i + 256] = Convert.ToByte((char)i);


            //The actual image data
            int offset = 512;
            int count = 0;
            for (int x = 0; x < 320; ++x)
            {
                for (int y = 0; y < 240; ++y)
                {
                    Color col = bmp.GetPixel(x, y);
                    int green = col.G >> 2;
                    int blue = col.B;
                    int red = col.R;

                    byte firstByte = joinGreenAndBlue(green, blue);
                    lcd_buffer[count + offset] = firstByte;

                    byte secByte = joinRedAndGreen(red, green);
                    lcd_buffer[(count + 1) + offset] = secByte;
                    count += 2;
                }
            }

            //Send it!
            int written;
            //The Texas instrument TMS320DM355ZCE allows us to update this screen at 60fps, but 16ms to send would be to stretch it a bit. 50ms will give 20fps
            ec = writer.Write(lcd_buffer, 50, out written);

            if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);
        }


        /// <summary>
        /// Update the screen with the bitmap
        /// </summary>
        /// <param name="bmp">WPF bitmap</param>
        public void UpdateScreen(BitmapSource bmp)
        {
            byte[] lcd_buffer = new byte[G19_BUFFER_LEN];

            lcd_buffer[0] = 0x02;
            Array.Copy(header, lcd_buffer, header.Length);

            //Just filling the remaining header with nonsens data
            for (int i = 16; i < 256; i++) { lcd_buffer[i] = Convert.ToByte((char)i); }
            for (int i = 0; i < 256; i++) lcd_buffer[i + 256] = Convert.ToByte((char)i);


            //The actual image data
            int offset = 512;
            int count = 0;
            PixelColor[,] pixels = GetPixels(bmp);
            for (int x = 0; x < 320; ++x)
            {
                for (int y = 0; y < 240; ++y)
                {
                    PixelColor col = pixels[x, y];
                    int green = col.Green >> 2;
                    int blue = col.Blue;
                    int red = col.Red;

                    byte firstByte = joinGreenAndBlue(green, blue);
                    lcd_buffer[count + offset] = firstByte;

                    byte secByte = joinRedAndGreen(red, green);
                    lcd_buffer[(count + 1) + offset] = secByte;
                    count += 2;
                }
            }

            //Send it!
            int written;
            //The Texas instrument TMS320DM355ZCE allows us to update this screen at 60fps, but 16ms to send would be to stretch it a bit. 50ms will give 20fps
            ec = writer.Write(lcd_buffer, 50, out written);

            if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);
        }


        private byte joinGreenAndBlue(int g, int b)
        {
            return (byte)((g << 5) | (b >> 3));
        }

        private byte joinRedAndGreen(int r, int g)
        {
            return (byte)((r & 0xF8) | (g >> 3));
        }

    }
}
