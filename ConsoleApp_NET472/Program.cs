using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QSoft.DevCon;

namespace ConsoleApp_NET472
{

    internal class Program
    {

        static void Main(string[] args)
        {
            //var pps = Guid.Empty.Devices().GroupBy(x => x.GetClass(), x=>x.GetClassGuid());
            //foreach(var oo in pps)
            //{
            //    System.Diagnostics.Trace.WriteLine($"{oo.Key}");
            //}

            var aaa = "Camera".Devices().Select(x => new
            {
                name = x.Service(),
                power = x.PowerData(),
                mac = x.Manufacturer(),
            });
            aaa.ToArray();

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
}
