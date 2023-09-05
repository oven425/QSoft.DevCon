using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class DD : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if(x.Contains("USBSTOR") == true && y.Contains("USBSTOR"))
            {
                var xx = x.Replace("#", "\\").ToUpperInvariant();
                var yy = y.ToUpperInvariant();
                var bb = xx.Contains(yy);

                System.Diagnostics.Trace.WriteLine($"x:{xx}");
                System.Diagnostics.Trace.WriteLine($"y:{yy}");
                return bb;
            }
            return false;
            
        }

        public int GetHashCode(string obj)
        {
            int hCode = obj.GetHashCode() ^ obj.GetHashCode();
            return hCode.GetHashCode();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var classname = Guid.Empty.Devices().GroupBy(x => x.GetClass()).Select(x=>x.Key);
                var driver = "Processor".Devices().Select(x => x.GetDriverInfo()).ToList();
                Console.ReadLine();

                //var ports = "Ports".Devices().Where(x=>x.GetService()=="Serial").Select(x => x.GetComPortName()).ToList();
                //var letters = DevMgrExtension.GetVolumeName().ToList();


                //var sssd = "Volume".Devices().Select(x => new { child = x.GetChildren(), parent = x.GetParent(), id = x.GetInstanceId(), physicalname = x.GetPhysicalDeviceObjectName() });
                //var f1 = letters.Join(sssd, x => x.target, y => y.physicalname, (x, y) => new { x,y});
                //var disks = "DiskDrive".Devices().Select(x => new { location=x.GetLocationPaths(), id = x.GetInstanceId(),child = x.GetChildren(), parent = x.GetParent() });
                //var usbs = "USB".GetDevClass().FirstOrDefault().Devices().Select(x => new { id = x.GetInstanceId(), children = x.GetChildren(),location = x.GetLocationPaths() });


                //var j1 = usbs.GroupJoin(sssd, x => x.children, y => y.id, (x, y) => new { x, y }, new DD())
                //    .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new {x, y?.physicalname })
                //    .GroupJoin(letters, x=>x.physicalname, y=>y.target, (x,y)=>new {x,y })
                //    .SelectMany(x=>x.y.DefaultIfEmpty(),(x,y)=>new {usb=x.x.x.x,letter=y.letter });
                //foreach(var oo in j1)
                //{
                //    Console.WriteLine($"{oo.usb.id} letter:{oo.letter}");
                //    var locationpaths = oo.usb.location.Aggregate("", (cur, next) => cur == "" ? next : $"{cur}{Environment.NewLine}{" ".PadRight(12)}{next}", (final) => $"LocalPaths: {final}");
                //    Console.WriteLine(locationpaths);
                //    Console.WriteLine();
                //}
                //Console.ReadLine();

                //var j2 = disks.GroupJoin(sssd, x => x.id, y => y.id, (x, y) => new { x, y }, new DD())
                //    .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x, y })
                //    .Where(x => x.y != null).Select(x=>new { usb=x.x, phi=x.y.physicalname}).ToList();


                //var groups1 = Guid.Empty.Devices(true).GroupBy(x => x.GetClassGuid().GetClassDescription(), y => new { classguid=y.GetClassGuid(), dd=y.GetClass() });
                //var cameras = "Camera".Devices().Select(x => new {name = x.GetDisplayName(), panel=x.Panel() }).ToList();
                //foreach(var camera in cameras)
                //{
                //    Console.WriteLine($"{camera.name}  {camera.panel}");
                //}
                Console.ReadLine();
                var volumes = "Volume".Devices().Select(x => new { child = x.GetChildren(), parent = x.GetParent(), service = x.GetService(), objectname = x.GetPhysicalDeviceObjectName(), name = x.GetDisplayName(), pl = x.GetPowerRelations() }).Where(x => x.child != "");
                var disks = "DiskDrive".Devices().Select(x=>new { instanceid=  x.GetInstanceId(), locationpaths = x.GetLocationPaths() });
                var j1 = volumes.Join(disks, x => x.pl.ToUpperInvariant(), y => y.instanceid.ToUpperInvariant(), (x, y) => new { x, y });
                var letters = DevMgrExtension.GetVolumeName();
                var j2 = letters.Join(j1, x=>x.target, y=>y.x.objectname, (x,y)=> new { x, y });


                var idc = Guid.Empty.Devices().Where(x => x.GetClassGuid().CompareTo(Guid.Parse("{4d1e55b2-f16f-11cf-88cb-001111000030}"))==0).ToList();
                //var has = i(Guid.Parse("{4d1e55b2-f16f-11cf-88cb-001111000030}"));
                var oo = "Camera".GetDevClass().FirstOrDefault();//.Devices().Select(x => x.GetFriendName()).ToList();
                var paths = Guid.Empty.Devices().Select(x => new { 
                path = x.DevicePath()
                }).ToList();
                //var infos = "Camera"
                var infos = Guid.Empty
                    .Devices(false)
                    .Select(x => new
                    {
                        parent = x.GetParent(),
                        driver = x.GetDriver(),
                        hwid = x.GetHardwaeeIDs(),
                        cap = x.GetCapabilities(),
                        description = x.GetDescription(),
                        friendname = x.GetFriendName(),
                        instanceid = x.GetInstanceId(),
                        classname = x.GetClass(),
                        classguid = x.GetClassGuid(),
                        desc = x.GetClassGuid().GetClassDescription(),
                        locationpaths = x.GetLocationPaths(),
                        isconnect = x.IsConnect(),
                        service = x.GetService(),
                        busnumber = x.GetBusNumber(),
                        enumeratorname = x.GetEnumerator_Name(),
                        address = x.GetAddress(),
                    }); ;

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
                    Console.WriteLine($"BusNumber: {info.busnumber}");
                    Console.WriteLine($"EnumeratorName: {info.enumeratorname}");
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

    public static class LinqEx
    {
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return outer
                .GroupJoin(inner,
                    outerKeySelector,
                    innerKeySelector,
                    (outerObj, inners) => new { outerObj, inners = inners.DefaultIfEmpty() }, comparer)
                .SelectMany(a => a.inners.Select(innerObj => resultSelector(a.outerObj, innerObj)));
        }

        //public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
        //{
        //    return outer
        //        .GroupJoin(inner,
        //            outerKeySelector,
        //            innerKeySelector,
        //            (outerObj, inners) => new { outerObj, inners = inners.DefaultIfEmpty() }, new DDD<TKey>(comparer))
        //        .SelectMany(a => a.inners.Select(innerObj => resultSelector(a.outerObj, innerObj)));
        //}

        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner,
                    outerKeySelector,
                    innerKeySelector,
                    (outerObj, inners) => new { outerObj, inners = inners.DefaultIfEmpty() })
                .SelectMany(a => a.inners.Select(innerObj => resultSelector(a.outerObj, innerObj)));
        }
    }

}
