using G19LCD;
using System.Windows;

namespace G19LCDText_pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LCD lcd;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void setLCD(LCD l)
        {
            lcd = l;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            lcd.CurrentPageIndex = 0;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            lcd.CurrentPageIndex = 1;
        }
    }
}
