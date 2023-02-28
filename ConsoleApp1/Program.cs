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

                //var devices = Guid.Empty.Devices();
                //foreach(var device in devices) 
                //{
                //    device.GetFriendName();
                //}
                //var groups1 = Guid.Empty.Devices().GroupBy(x => x.GetClass(), y => y.GetClassGuid());
                //var alldeviceinfo = Guid.Empty.Devices().Select(x => new DeviceInfo(x.dev, x.devdata)).ToList();
                //var infos = "Ports".GetDevClass().FirstOrDefault()
                //    .Devices()
                //    .Select(x => new
                //    {
                //        portname=x.GetComPortName(),
                //        friendname = x.GetFriendName(),
                //        instanceid = x.GetInstanceId(),
                //        classname = x.GetClass(),
                //        classguid = x.GetClassGuid(),
                //        desc = x.GetClassGuid().GetClassDescription(),
                //        localoath = x.GetLoalPaths()
                //    });
                var infos = Guid.Empty
                    .Devices()
                    .Where(x=>x.GetInstanceId() == "USB\\VID_058F&PID_6387\\51204A16")
                    .Select(x => new
                    {
                        friendname = x.GetFriendName(),
                        instanceid = x.GetInstanceId(),
                        classname = x.GetClass(),
                        classguid = x.GetClassGuid(),
                        desc = x.GetClassGuid().GetClassDescription(),
                        localoath = x.GetLocationPaths(),
                        locationinformation=x.GetLoationInformation()
                    });
                foreach (var info in infos)
                {
                    Console.WriteLine(info);
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
