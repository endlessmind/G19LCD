using G19LCD.Pages;
using G19LCD.Transactions;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace G19LCD
{
    public class LCD
    {
        private const int MAX_FRAMES = 20 * 15; //20fps for 15 sec

        UsbEndpointWriter writer;
        UsbEndpointReader reader;
        ErrorCode ec = ErrorCode.None;
        IUsbDevice wholeUsbDevice;

        public static UsbDevice MyUsbDevice;
        public UsbDeviceFinder MyUsbFinder;

        private int cPageIndex;

        private Boolean captureImages;

        private Bitmap currentImageVisible;

        private Transaction pageTrans = null;

        public List<LcdPage> Pages { get; set;  }


        List<Bitmap> images = new List<Bitmap>();

        
        /// <summary>
        /// Get or Set the current Page displayed on the screen
        /// </summary>
        public LcdPage CurrentPage {
            get { return Pages[CurrentPageIndex]; }
            set
            {
                cPageIndex = Pages.IndexOf(value);
                pageTrans = new FadeTransaction(this);
                UpdatePage();
            }
        }

        /// <summary>
        ///  Get or Set the index of the current Page displayed on the screen
        /// </summary>
        public int CurrentPageIndex
        {
            get { return cPageIndex; }
            set
            {
                cPageIndex = value;
                pageTrans = new FadeTransaction(this);
                UpdatePage();
            }
        }

        /// <summary>
        ///  Get or Set if frames are captured
        /// </summary>
        public bool captureFrames
        {
            get { return captureImages; }

            set
            {
                if (!value) { images.Clear(); }
                captureImages = value;
            }
        }

        public List<Bitmap> getImages
        {
            get { return images; } //This list will always be empty as It's just used it to create the gif
        }

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


            if (!ReferenceEquals(wholeUsbDevice, null)) {
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
            if (MyUsbDevice != null) {
                if (MyUsbDevice.IsOpen) {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null)) {
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

        public void UpdatePage()
        {
            LcdPage pg = Pages[CurrentPageIndex];
            UpdateScreen(pg.Invalidate(), pageTrans);
            //Screen updated
            pageTrans = null;
        }


        /// <summary>
        ///  Update the screen with the bitmap
        /// </summary>
        /// <param name="bmp">WinForm Bitmap</param>
        public void UpdateScreen(Bitmap bmp)
        {
           // bmp.Save("page" + cPageIndex + ".jpg");
            UpdateScreen(bmp, null);
        }

        /// <summary>
        ///  Update the screen with the bitmap
        /// </summary>
        /// <param name="bmp">WPF BitmapSource</param>
        public void UpdateScreen(BitmapSource bmp)
        {
            UpdateScreen(Utils.BitmapImage2Bitmap(bmp));
        }


        /// <summary>
        /// Update the screen with the bitmapSource
        /// </summary>
        /// <param name="bmp">WPF BitmapSource</param>
        /// <param name="transaction">Transaction between images</param>
        public void UpdateScreen(BitmapSource bmp, Transaction transaction)
        {
            UpdateScreen(Utils.BitmapImage2Bitmap(bmp), transaction);
        }

        /// <summary>
        ///  Update the screen with the bitmap
        /// </summary>
        /// <param name="bmp">WinForm Bitmap</param>
        /// <param name="transaction">Transaction between images</param>
        public void UpdateScreen(Bitmap bmp, Transaction transaction)
        {
            if (bmp.Width != 320 || bmp.Height != 240)
                throw new SystemException("Incorrect image size! Image size must be 320x240!");


            if (transaction != null && currentImageVisible != null) {
                //We create a clone of the new (or next) bitmap, so that we can dispose the object when the transaction is done.
                //This will reduse the amount of pointers used in memory during transaction, and we can dispose some of them when we're done!
                transaction.start(currentImageVisible, bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat));
            }

            if (captureFrames) {
                if (images.Count == MAX_FRAMES) {
                    images.RemoveAt(0);
                }
                images.Add(bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat));
            }


            byte[] lcd_buffer = Utils.convertToByte(bmp);
            int written;
            //The Texas instrument TMS320DM355ZCE allows us to update this screen at 60fps. 33ms will allow for atleast to 30fps!
            ec = writer.Write(lcd_buffer, 33, out written);

            if (ec != ErrorCode.None) {
                throw new Exception(UsbDevice.LastErrorString);
            } else {
                currentImageVisible = bmp;
            }
        }

    }
}
