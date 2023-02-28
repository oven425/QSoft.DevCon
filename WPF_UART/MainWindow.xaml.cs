using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using QSoft.DevCon;

namespace WPF_UART
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
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
            if (m_MainUI == null) 
            {
                this.DataContext = this.m_MainUI = new MainUI();
                var infos = "Ports".GetDevClass().FirstOrDefault()
                    .Devices();
                foreach (var info in infos)
                {
                    Uartd ud = new Uartd();
                    ud.LocalPath = info.GetLocationPaths();
                    ud.InstanceId = info.GetInstanceId();
                    ud.FriendName = info.GetFriendName();
                    ud.Name = info.GetComPortName();
                    m_MainUI.Uarts.Add(ud);
                }
                var setting = Environment.GetCommandLineArgs();
            }
        }
    }

    public class Uartd : INotifyPropertyChanged
    {
        public string FriendName { set; get; }
        public string LocalPath { set; get; }
        public string InstanceId { set; get; }
        public string HardwardID { set; get; }
        public string Name { set; get; }
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Testd
    {
        public string Name { set; get; }
        public bool IsSelected { set; get; }
    }

    public class Setting_Port
    {
        public bool IsRTS { set; get;}
        public bool IsDTR { set; get;}
        public string InstandId { set; get; }
        public string LocalPath { set; get; }
    }

    public class MainUI : INotifyPropertyChanged
    {
        public ObservableCollection<Uartd> Uarts { get; set; } = new ObservableCollection<Uartd> ();    
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
