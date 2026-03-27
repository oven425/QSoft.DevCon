using Microsoft.Win32.SafeHandles;
using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp_NET472
{

    internal class Program
    {

        static void Main(string[] args)
        {
            try
            {
                string namespacePath = @"\\.\root\wmi";
                string queryString = "SELECT * FROM BatteryStatus";

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(namespacePath, queryString);

                // 2. 執行查詢並遍歷結果
                foreach (ManagementObject obj in searcher.Get())
                {
                    Console.WriteLine("--- Battery Status ---");

                    // 讀取屬性 (例如：是否正在充電)
                    // 注意：WMI 傳回的是 object，需要根據文件轉型
                    bool isCharging = (bool)obj["Charging"];
                    uint voltage = (uint)obj["Voltage"];
                    uint capacity = (uint)obj["RemainingCapacity"];

                    Console.WriteLine($"Charging: {isCharging}");
                    Console.WriteLine($"Voltage: {voltage} mV");
                    Console.WriteLine($"Remaining Capacity: {capacity} mWh");

                    // 如果想列出所有可用屬性：
                    /*
                    foreach (PropertyData data in obj.Properties)
                    {
                        Console.WriteLine($"{data.Name}: {data.Value}");
                    }
                    */
                }
                //Process.Start("powercfg.exe", "/batteryreport  /output battery.xml xml").WaitForExit();
                //var serializer = new XmlSerializer(typeof(BatteryReport));
                //using (var reader = new StreamReader("battery.xml"))
                //{
                //    var report = (BatteryReport)serializer.Deserialize(reader);
                //    Console.WriteLine($"設計容量: {report.Batteries[0].DesignCapacity} mWh");
                //    Console.WriteLine($"完整充電容量: {report.Batteries[0].FullChargeCapacity} mWh");
                //}
                QSoft.DevCon.DevConExtension.GetVolumeName().ToArray();
                var guid = "Battery".GetClassGuids().FirstOrDefault();
                var batterys = guid.DevicesFromInterface().Select(x => new
                {
                    devpath = x.DevicePath(),
                    desc = x.As().DeviceDesc(),
                });



                foreach (var oo in batterys)
                {
//                    public static SafeFileHandle OpenHandle(
//    string path,
//    FileMode mode = FileMode.Open,
//    FileAccess access = FileAccess.Read,
//    FileShare share = FileShare.Read,
//    FileOptions options = FileOptions.None,
//    long preallocationSize = 0
//);

                    using (var fs = oo.devpath.OpenHandle())
                    {
                        fs.GetBatteryInfo();
                    }
                }
            }
            catch (Exception ee)
            {
            }
            var aaa = "Camera".Devices().Select(x => new
            {
                name = x.Service(),
                power = x.PowerData(),
                mac = x.Manufacturer(),
                pps = x.AllPropertys(),
                id = x.DeviceInstanceId(),
                clssdesc = x.ClassGuid().ClassDesc(),
                installdate = x.FirstInstallDate(),
            });
            var pp = aaa.ToArray()[0].power.ToString();
            System.Diagnostics.Trace.WriteLine(pp);
            var ffs = "Camera".GetClassGuids();
            var ggu = Guid.Parse("{6bdd1fc6-810f-11d0-bec7-08002be2092f}");
            var ddd = ggu.GetClassDesc();
            var cameraa = Guid.Parse("{E5323777-F976-4f5b-9B55-B94699C46E44}");
            var cameras = DevConExtension.KSCATEGORY_CAPTURE.DevicesFromInterface()
                .Select(x => new
                {
                    devicepath = x.DevicePath(),
                    friendname = x.As().GetFriendName(),
                    panel = x.As().Panel(),
                }).ToList();
            System.Diagnostics.Trace.WriteLine($"sensor:{cameras.Count}");
            var vvvs = "Volume".Devices(true)
                .Select(x => new
                {
                    connected = x.IsConnected(),
                    present = x.IsPresent(),
                    id = x.DeviceInstanceId(),
                    //biosname = x.BiosDeviceName()
                });
            try
            {
                foreach (var oo in vvvs)
                {
                    Console.WriteLine(oo);
                }
            }
            catch(Exception ee)
            {

            }
            
            foreach (var oo in "Camera".Devices())
            {
                var biosname = oo.BiosDeviceName();
                var firstinstalldate = oo.FirstInstallDate();
                var isconnected= oo.IsConnected();
                var panel = oo.Panel();
                var siblings = oo.Siblings();
                var driverprovider = oo.DriverProvider();
                var problemcode = oo.ProblemCode();
                var infsection = oo.DriverInfSection();
                var friendname = oo.GetFriendName();
                //oo.SetFriendName($"USB2.0 HD UVC WebCam");
            }

            var ll = Guid.Empty.Devices().Select(x => new
            {
                objectname = x.GetPhysicalDeviceObjectName(),
                service = x.Service(),
                power_relation = x.PowerRelations(),
                mfg = x.Manufacturer(),
                instanceid = x.DeviceInstanceId(),
                locationpaths = x.GetLocationPaths(),
                hardwareids = x.HardwaeeIDs(),
                friendname = x.GetFriendName(),
                class_guid = x.GetClassGuid(),
                children = x.Childrens(),
                parent = x.Parent(),
                desc = x.GetDeviceDesc(),
                class_name = x.GetClassGuid().GetClassDesc(),
                drive_version = x.GetDriverVersion(),
                driver_inf = x.DriverInfSection(),
                driver_date = x.GetDriverDate(),
            });

            try
            {
                foreach (var device in ll)
                {

                    System.Diagnostics.Trace.WriteLine($"power_relation:{device.power_relation}");
                    System.Diagnostics.Trace.WriteLine($"friend name:{device.friendname}");
                    System.Diagnostics.Trace.WriteLine($"objectname:{device.objectname}");
                    System.Diagnostics.Trace.WriteLine($"service:{device.service}");
                    System.Diagnostics.Trace.WriteLine($"parent:{device.parent}");
                    System.Diagnostics.Trace.WriteLine($"children: {device.children.Count}");
                    foreach (var oo in device.children)
                    {
                        System.Diagnostics.Trace.WriteLine($"{oo}");
                    }



                    System.Diagnostics.Debug.WriteLine($"mfg:{device.mfg}");
                    System.Diagnostics.Trace.WriteLine($"driver_inf:{device.driver_inf}");
                    System.Diagnostics.Trace.WriteLine($"drive_version:{device.drive_version}");
                    System.Diagnostics.Trace.WriteLine($"driver_date:{device.driver_date}");
                    System.Diagnostics.Trace.WriteLine($"instanceid:{device.instanceid}");
                    System.Diagnostics.Trace.WriteLine($"clss guid:{device.class_guid}");
                    System.Diagnostics.Trace.WriteLine($"classname:{device.class_name}");
                    System.Diagnostics.Trace.WriteLine($"desc:{device.desc}");
                    System.Diagnostics.Trace.WriteLine($"locationpaths:");
                    foreach (var oo in device.locationpaths)
                    {
                        System.Diagnostics.Trace.WriteLine(oo);
                    }
                    System.Diagnostics.Trace.WriteLine("hardwareids:");
                    foreach (var oo in device.hardwareids)
                    {
                        System.Diagnostics.Trace.WriteLine(oo);
                    }
                    System.Diagnostics.Trace.WriteLine("");
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
            }

            Console.ReadLine();

        }
    }

    [XmlRoot("BatteryReport", Namespace = "http://schemas.microsoft.com/battery/2012")]
    public class BatteryReport
    {
        //[XmlElement("ReportInformation")]
        //public ReportInformation ReportInformation { get; set; }

        //[XmlElement("SystemInformation")]
        //public SystemInformation SystemInformation { get; set; }

        [XmlArray("Batteries")]
        [XmlArrayItem("Battery")]
        public List<Battery> Batteries { get; set; }

        [XmlElement("RuntimeEstimates")]
        public RuntimeEstimates RuntimeEstimates { get; set; }
    }

    public class ReportInformation
    {
        public string ReportGuid { get; set; }
        public string ReportVersion { get; set; }
        public DateTime ScanTime { get; set; }
        public DateTime LocalScanTime { get; set; }
        public DateTime ReportStartTime { get; set; }
        public DateTime LocalReportStartTime { get; set; }
        public int ReportDuration { get; set; }
        public string UtcOffset { get; set; }
    }

    public class SystemInformation
    {
        public string ComputerName { get; set; }
        public string SystemManufacturer { get; set; }
        public string SystemProductName { get; set; }
        public DateTime BIOSDate { get; set; }
        public string BIOSVersion { get; set; }
        public string OSBuild { get; set; }
        public string PlatformRole { get; set; }
        public int ConnectedStandby { get; set; }
    }

    public class Battery
    {
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string SerialNumber { get; set; }
        public string ManufactureDate { get; set; }
        public string Chemistry { get; set; }
        public int LongTerm { get; set; }
        public int RelativeCapacity { get; set; }
        public int DesignCapacity { get; set; }
        public int FullChargeCapacity { get; set; }
        public int CycleCount { get; set; }
    }

    public class RuntimeEstimates
    {
        [XmlElement("DesignCapacity")]
        public CapacityEstimate DesignCapacity { get; set; }

        [XmlElement("FullChargeCapacity")]
        public CapacityEstimate FullChargeCapacity { get; set; }
    }

    public class CapacityEstimate
    {
        public int Capacity { get; set; }
        public string ActiveRuntime { get; set; }
        public string ConnectedStandbyRuntime { get; set; }
    }
}
