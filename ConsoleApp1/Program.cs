using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int oi = 164;
                oi = oi & (int)SetupApi.CM_DEVCAP_HARDWAREDISABLED;
                //var devices = Guid.Empty.Devices();
                //foreach(var device in devices) 
                //{
                //    device.GetFriendName();
                //}
                //var groups1 = Guid.Empty.Devices().GroupBy(x => x.GetClass(), y => y.GetClassGuid());
                //var alldeviceinfo = Guid.Empty.Devices().Select(x => new DeviceInfo(x.dev, x.devdata)).ToList();



                //var infos = "Ports".GetDevClass().FirstOrDefault()
                var infos = Guid.Empty
                    .Devices(false)
                    .Select(x => new
                    {
                        driver = x.GetDriver(),
                        hwid = x.GetHardwaeeIDs(),
                        cap = x.GetCapabilities(),
                        portname = x.GetComPortName(),
                        description = x.GetDescription(),
                        friendname = x.GetFriendName(),
                        instanceid = x.GetInstanceId(),
                        classname = x.GetClass(),
                        classguid = x.GetClassGuid(),
                        desc = x.GetClassGuid().GetClassDescription(),
                        locationpaths = x.GetLocationPaths(),
                        isconnect = x.IsConnect(),
                        service = x.GetService(),
                    }); ;
                //foreach (var info in infos)
                //{
                //    Console.WriteLine(info);
                //}
                //var infos = Guid.Empty
                //    .Devices()
                //    .Where(x=>x.GetDisplayName() == "USB Mass Storage Device")
                //    .Select(x => new
                //    {
                //        friendname = x.GetFriendName(),
                //        description = x.GetDescription(),
                //        instanceid = x.GetInstanceId(),
                //        classname = x.GetClass(),
                //        classguid = x.GetClassGuid(),
                //        desc = x.GetClassGuid().GetClassDescription(),
                //        localoath = x.GetLocationPaths(),
                //        locationinformation=x.GetLoationInformation(),
                //    });
                foreach (var info in infos)
                {
                    Console.WriteLine($"CalssMame: {info.classname}");
                    Console.WriteLine($"FriendName: {info.friendname}");
                    Console.WriteLine($"GetDescription: {info.description}");
                    Console.WriteLine($"InstanceId: {info.instanceid}");
                    var hwids = info.hwid.Aggregate("", (cur, next) => cur == "" ? next : $"{cur}{Environment.NewLine}{" ".PadRight(13)}{next}", (final) => $"HardwareIds: {final}");
                    //var aaa1 = info.hwid.Aggregate("", (x, y) => $"{x}{y}{Environment.NewLine}{" ".PadRight(13)}", (x)=> $"HardwareIds: {x.Remove(x.LastIndexOf(Environment.NewLine))}");
                    //var aaa= info.hwid.Aggregate("HardwareIds: ", (x, y) => $"{x}{y}\r\n{" ".PadRight(13)}");
                    Console.WriteLine(hwids);
                    var locationpaths = info.locationpaths.Aggregate("", (cur, next) => cur == "" ? next : $"{cur}{Environment.NewLine}{" ".PadRight(12)}{next}", (final) => $"LocalPaths: {final}");
                    Console.WriteLine(locationpaths);
                    Console.WriteLine($"Service: {info.service}");
                    Console.WriteLine($"Driver: {info.driver}");
                    Console.WriteLine("-----------------");
                }
                Console.WriteLine("ed");
                Console.ReadLine();
                //List<(string instanceid, int port)> changes = new List<(string instanceid, int port)>();
                //changes.Add(("ACPI\\PNP0501\\0", 100));
                //changes.Add(("ACPI\\PNP0501\\1", 101));

                //var joinchange = "Ports".GetDevClass().FirstOrDefault()
                //    .Devices()
                //    .Join(changes, x => x.GetInstanceId(), y => y.instanceid, (x, y) => new { x, y })
                //    .Do(x => 
                //    {
                //        x.x.Enable();
                //    });
                //foreach(var oo in joinchange)
                //{
                    
                //}
                

                //"Ports".GetDevClass().FirstOrDefault()
                //    .Devices()
                //    .Where(x => x.GetFriendName() == "通訊連接埠 (")
                //    .Enable();
                //"Ports".GetDevClass().FirstOrDefault()
                //    .Devices()
                //    .Where(x => x.GetFriendName() == "")
                //    .Do(x => { x.SetFriendName("COMPORT"); });
                //guids.FirstOrDefault().Devices().Where(x => x.GetFriendName() == "通訊連接埠 (").Enable();
                //foreach (var device in guids.FirstOrDefault().Devices())
                //{
                //    //System.Diagnostics.Trace.WriteLine(device.GetFriendName());
                //    //System.Diagnostics.Trace.WriteLine(device.GetDescription());
                //    //System.Diagnostics.Trace.WriteLine(device.GetInstanceId());
                //}


                var dds = new DevMgr().AllDevice();
                foreach (var item in dds)
                {
                    System.Diagnostics.Trace.WriteLine(item.Description);
                }

                var groups = new DevMgr().AllDevice().GroupBy(x=>x.Class);
                foreach (var group in groups)
                {
                    System.Diagnostics.Trace.WriteLine(group.Key);
                    foreach(var item in group)
                    {
                        System.Diagnostics.Trace.WriteLine(item.Description);
                    }
                }
                
                var disable_count = new DevMgr().Disable(x => x.Description.Contains("Camera"));

                var enable_count = new DevMgr().Enable(x => x.Description.Contains("Camera"));
                //var dds = new DevMgr().AllDevice().Where(x => x.Description.Contains("Camera"));
                //var dds = new DevMgr().AllPath();
                //var lookup = dds.ToLookup(x => x.DeviceInfo);
            }
            catch(Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
            }
            //Enable(x => x == 5);
            Console.ReadLine();
        }

        static void Enable(Func<int, bool> func)
        {
            for(int i=0; i<100; i++)
            {
                if(func(i)==true)
                {
                    break;
                }
            }
        }



    }
}
