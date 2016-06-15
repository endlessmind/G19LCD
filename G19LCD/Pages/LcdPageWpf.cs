using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace G19LCD.Pages
{
    public class LcdPageWpf : LcdPage
    {
        private readonly RenderTargetBitmap _bitmap;
        private readonly System.Windows.Size _deviceSize;
        private readonly Rect _deviceRect;

        public FrameworkElement Element { get; set; }

        public LcdPageWpf(){
            _bitmap = new RenderTargetBitmap(320, 240, 96.0, 96.0, PixelFormats.Pbgra32);
            _deviceSize = new System.Windows.Size(320, 240);
            _deviceRect = new Rect(_deviceSize);
        }



        protected override Bitmap OnDrawn()
        {
            _bitmap.Clear();
            if (Element != null)
                _bitmap.Render(Element);
            //_bitmap.CopyPixels(_32BppPixels, 320 * 4, 0);
            //return _32BppPixels;
            return Utils.BitmapImage2Bitmap(_bitmap);
        }

        protected override void OnUpdate()
        {
            Element.InvalidateVisual();
            Element.Measure(_deviceSize);
            Element.Arrange(_deviceRect);
        }

    }
}
