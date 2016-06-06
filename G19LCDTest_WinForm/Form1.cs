using G19LCD;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace G19LCDTest_WinForm
{
    public partial class Form1 : Form
    {
        LCD lcd;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //vID = 0x046D - pID = 0x229C  for the G19
            lcd = new LCD(1133, 49705);
            lcd.OpenDevice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(G19LCDTest_WinForm.Properties.Resources.test1);
            lcd.UpdateScreen(b);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            lcd.CloseDevice();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(Width, Height);
            DrawToBitmap(b, new Rectangle(0, 0, Width, Height));
            lcd.UpdateScreen(b);
        }
    }
}
