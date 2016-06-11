using G19LCD;
using G19LCD.Transactions;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static G19LCD.Transaction;

namespace G19LCDTest_WinForm
{
    public partial class Form1 : Form
    {
        LCD lcd;
        Bitmap imgToShow;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //vID = 0x046D - pID = 0xC229  for the G19
            lcd = new LCD(1133, 49705);
            lcd.OpenDevice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            imgToShow = new Bitmap(G19LCDTest_WinForm.Properties.Resources.test1);
            Thread drawT = new Thread(new ThreadStart(showOnScreen));
            drawT.Start();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            lcd.CloseDevice();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imgToShow = new Bitmap(Width, Height);
            DrawToBitmap(imgToShow, new Rectangle(0, 0, Width, Height));
            //We run this in a thread so we don't lock up the GUI
            Thread drawT = new Thread(new ThreadStart(showOnScreen));
            drawT.Start();
        }

        
        private void showOnScreen()
        {
            Transaction fade = new FadeTransaction(lcd);
            fade.TransEvent += new TransactionEventHandler(OnTransActionComplete);

            lcd.UpdateScreen(imgToShow, fade);
        }

        private void OnTransActionComplete(Object sender, EventArgs e)
        {
            //Transaction is complete. We can no continue updating the display.
            Console.WriteLine("Transaction complete");
            //Unregister the event, as the Transaction will be disposed after this event was triggered
            ((Transaction)sender).TransEvent -= new TransactionEventHandler(OnTransActionComplete);
        }
    }
}
