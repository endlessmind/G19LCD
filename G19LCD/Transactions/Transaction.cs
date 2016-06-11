using System;
using System.Drawing;

namespace G19LCD
{
    public abstract class Transaction
    {
        public delegate void TransactionEventHandler(object sender, EventArgs e);
        public event TransactionEventHandler TransEvent;

        protected virtual void OnTransActionComplete(EventArgs e)
        {
            TransEvent?.Invoke(this, e);
        }

        public Bitmap currentImage, nextImage;
        public LCD lcd_instance;
        public int written;

        public Transaction(LCD lcd)
        {
            lcd_instance = lcd;
            written = 0;
        }

        public abstract void start(Bitmap current, Bitmap next);
    }
}
