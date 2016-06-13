using System;
using System.Drawing;
using System.Windows.Forms;

namespace G19LCD.Pages
{
    public class LcdPageWF : LcdPage
    {

        public Control Element { get; set; }

        protected override Bitmap OnDrawn()
        {
            Bitmap bmp = new Bitmap((int)Element.Width, (int)Element.Height);
            Element.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            
            return bmp;
        }

        protected override void OnUpdate()
        {
        }
    }
}
