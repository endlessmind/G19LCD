using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using G19LCD;
using System.Windows.Controls;
using System.IO;
using System.Drawing.Imaging;

namespace G19LCDTest_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LCD lcd;
        DispatcherTimer life_timer;
        LifeGame sim;
        System.Drawing.Brush blackBrush = System.Drawing.Brushes.Black;
        System.Drawing.Brush whiteBrush = System.Drawing.Brushes.White;
        int cellSize = 2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            life_timer = new DispatcherTimer();
            life_timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            life_timer.Tick += new EventHandler(Life_Tick);

            //vID = 0x046D - pID = 0xC229  for the G19
            lcd = new LCD(1133, 49705);
            lcd.OpenDevice();

            InitLife();
        }


        private void gridImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeFinished();
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (life_timer.IsEnabled)
            {
                life_timer.Stop();
                btnAction.Content = "Start";
            }
            else
            {
                life_timer.Start();
                btnAction.Content = "Stop";
            }
        }

        private void btnRandomize_Click(object sender, RoutedEventArgs e)
        {
            RandomizeCells();
        }

        private void Life_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                sim.ProcessNextGen();
                drawGeneration();
            });

        }


        private void ResizeFinished()
        {
            if (life_timer != null && life_timer.IsEnabled)
            {
                life_timer.Stop();
                InitLife();
                life_timer.Start();
            }
            else
            {
                InitLife();
            }
        }

        private void drawGeneration()
        {
            //Create our bitmap. I'm using System.Drawing as it has way better performance than WPF Canvas or DrawingVisual
            var bitmap = new System.Drawing.Bitmap(320, 240);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //We need to draw the whole bitmap white, as we're drawing black rectangles on it.
            //If we don't, we'll just get a completely black image.
            g.Clear(System.Drawing.Color.White);
            //Loop through our 2-dimentional array of cells
            for (int x = 0; x < sim.GridSize.X; x++)
            {
                for (int y = 0; y < sim.GridSize.Y; y++)
                {
                    drawCell(bitmap, g, x, y);
                }
            }
            //We're done. Dispose the Grapghics instance to free up the memory
            g.Dispose();
            //Convert the Drawing's bitmap to Imaging's BitmapImage
            BitmapImage bmpImg = Utils.ConvertBmpToBmpImg(bitmap);
            //Freezing the BitmapImage allows for it to be "moved" or accesssed cross threads!
            bmpImg.Freeze();
            //Invoking our UI Thread so we can update the UI
            Dispatcher.BeginInvoke((Action)(() =>
            {
                imgOutput.Source = bmpImg;
                lbGeneration.Content = String.Format("Generation {0}", sim.Generation);

                //We could take an image of the whole window and send that to the screen also
                //lcd.UpdateScreen(Utils.GetImage(this));
                
                //Send the image to the screen
                lcd.UpdateScreen(bmpImg);

            }));

            //Dispose the bitmap to remove it from memory.
            bitmap.Dispose();
        }


        private void drawCell(System.Drawing.Bitmap bmp, System.Drawing.Graphics g, int x, int y)
        {
            //We only draw the cell if it's alive
            if (sim[x, y])
            {
                System.Drawing.Rectangle r = new System.Drawing.Rectangle(x * cellSize, y * cellSize, cellSize, cellSize);
                //Draw the rectangle, If we're drawing to fast, we might get an error saying that the brush it already in use.
                g.FillRectangle(blackBrush, r);
            }
        }



        private void InitLife()
        {
            try
            {
                //I'm using the grids size because the size of the Image is set by the size of the bitmap that has been set as source.
                sim = new LifeGame(cellSize, (int)gridImage.ActualWidth, (int)gridImage.ActualHeight);
                //Randomize the new cells
                RandomizeCells();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void RandomizeCells()
        {
            sim.SetAllState(false);
            Random r = new Random();
            for (int x = 0; x < sim.GridSize.X; x++)
            {
                for (int y = 0; y < sim.GridSize.Y; y++)
                {
                    if (r.Next(100) <= 25)
                    {
                        sim.ToggleCell(x, y);
                    }
                }
            }
            //We manage it as if it was the first generation
            sim.ProcessFirstGen();
            drawGeneration();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lcd.CloseDevice();
        }
    }
}
