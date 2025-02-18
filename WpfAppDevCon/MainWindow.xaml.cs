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
using System.Collections.ObjectModel;
using QSoft.DevCon.WPF;

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
        MainUI m_MainUI;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(m_MainUI == null)
            {
                this.DataContext = m_MainUI = new MainUI();
                var cameras = "Camera".Devices();
                foreach (var cam in cameras)
                {
                    this.m_MainUI.Devices.Add(new DeviceData()
                    {
                        Icon = cam.Icon(),
                        FriendName = cam.GetDeviceDesc()
                    });

                }
            }
            
        }
    }
    
    public class MainUI
    {
        public ObservableCollection<DeviceData> Devices { set; get; } = [];
    }

    public class DeviceData
    {
        public BitmapSource Icon { set; get; }
        public string FriendName { set; get; }
        public ObservableCollection<DeviceData> Devices { set; get; } = [];
    }
}