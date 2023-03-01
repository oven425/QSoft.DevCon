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
using System.IO;
using System.Xml.Serialization;
using System.IO.Ports;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

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
                    ud.Tests.Add(new Testd() {Key= "LocationPath", Value = info.GetLocationPaths() });
                    ud.Tests.Add(new Testd() { Key = "InstanceId", Value = info.GetInstanceId() });
                    ud.Tests.Add(new Testd() { Key = "PortName", Value = info.GetComPortName() });
                    //ud.LocationPath = new Testd() { Value = info.GetLocationPaths() };
                    //ud.InstanceId = new Testd() { Value = info.GetInstanceId() };
                    ud.FriendName = info.GetDisplayName();
                    //ud.Name = new Testd() { Value = info.GetComPortName() };
                    //ud.LocationPath = new Testd() { Value = info.GetLocationPaths() };
                    m_MainUI.Uarts.Add(ud);
                    System.Diagnostics.Trace.WriteLine($"ud.Tests:{ud.Tests.Count}");
                }
                var args = Environment.GetCommandLineArgs();
                if(args.Length>1)
                {
                    var settingfile = args[1];
                    System.Diagnostics.Trace.WriteLine($"setting:{args[1]}");
                    if (File.Exists(args[1]))
                    {
                        System.Diagnostics.Trace.WriteLine(settingfile);
                        try
                        {
                            using (var file = File.OpenRead(settingfile))
                            {
                                System.Xml.Serialization.XmlSerializer xml = new XmlSerializer(typeof(Setting));
                                Setting setting = (Setting)xml.Deserialize(file);
                                foreach(var port in setting.Ports)
                                {
                                    m_MainUI.Uarts.Any(x => port.IsTest(x));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                            MessageBox.Show(ex.Message);
                            this.Close();
                        }
                        
                    }
                }

            }
        }

        private void button_savesetting_Click(object sender, RoutedEventArgs e)
        {
            //var test = this.m_MainUI.Uarts.Where(x => x.InstanceId.IsSelected == true || x.LocationPath.IsSelected == true || x.Name.IsSelected == true);
            var test = this.m_MainUI.Uarts.Where(x => x.Tests.All(y=>y.IsSelected==true));
            if (test.Any())
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Text Files | *.txt";
                save.DefaultExt = "txt";
                if (save.ShowDialog() == true)
                {
                    System.Diagnostics.Trace.WriteLine(save.FileName);
                    try
                    {
                        using (var file = File.Create(save.FileName))
                        {
                            System.Xml.Serialization.XmlSerializer xml = new XmlSerializer(typeof(Setting));
                            Setting setting = new Setting();
                            setting.Ports.AddRange(test);
                            xml.Serialize(file, setting);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }

                }

            }
        }
    }

    public class Uartd : INotifyPropertyChanged
    {
        [XmlAttribute("EnableRTS")]
        public bool EnableRTS { set; get; }
        [XmlAttribute("EnableDTR")]
        public bool EnableDTR { set; get; }
        [XmlIgnore]
        public string FriendName { set; get; }
        public ObservableCollection<Testd> Tests { set; get; } = new ObservableCollection<Testd>();
        //public Testd LocationPath { set; get; }
        //public Testd InstanceId { set; get; }
        //public string HardwardID { set; get; }
        //public Testd Name { set; get; }
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsTest(Uartd data)
        {
            bool result = false;
            //if(this.LocationPath.IsSelected==true)
            //{
            //    if(data.)
            //}

            return result;
        }
    }

    public class Testd
    {
        [XmlAttribute("IsSelected")]
        public bool IsSelected { set; get; }
        public string Key { set; get; }
        public string Value { set; get; }
    }

    //public class Setting_Port
    //{
    //    [XmlAttribute("EnableRTS")]
    //    public bool EnableRTS { set; get;}
    //    [XmlAttribute("EnableRTS")]
    //    public bool EnableDTR { set; get;}
    //    public Testd InstandId { set; get; }
    //    public Testd LocationPath { set; get; }
    //    public Testd Name { set; get; }
    //}

    public class Setting
    {
        public string TestFile { set; get; }
        //[XmlArray("Ports")]
        //[XmlArrayItem("Port")]
        public List<Uartd> Ports { set; get; } = new List<Uartd>();
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
