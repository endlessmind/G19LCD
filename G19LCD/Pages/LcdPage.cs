using System.Drawing;

namespace G19LCD.Pages
{
    public abstract class LcdPage
    {
        protected abstract Bitmap OnDrawn();

        public Bitmap Invalidate()
        {
            OnUpdate();
            return OnDrawn();
        }

        protected abstract void OnUpdate();

    }
}
