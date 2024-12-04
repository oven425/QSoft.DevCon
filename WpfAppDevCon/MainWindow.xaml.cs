using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QSoft.DevCon;
using System.Windows.Interop;

namespace WpfAppDevCon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var cameras = "Camera".Devices();
            foreach(var cam in cameras)
            {
                var iconptr = cam.Icon();
                var icon = Imaging.CreateBitmapSourceFromHIcon(iconptr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //iconptr.DestoryIcon();
                this.image.Source = icon;

            }
        }
    }
}