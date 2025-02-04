﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QSoft.DevCon;

namespace ConsoleApp_NET472
{
    //public class AA
    //{
    //    string AAa = "123";
    //}
    internal class Program
    {
        //static void AA(out AA aa)
        //{
        //    aa = null;
        //}
        static void Main(string[] args)
        {
            //AA(out _);
            //var pps = Guid.Empty.Devices().GroupBy(x => x.GetClass(), x=>x.GetClassGuid());
            //foreach(var oo in pps)
            //{
            //    System.Diagnostics.Trace.WriteLine($"{oo.Key}");
            //}

            //foreach(var oo in DevConExtension.GUID_DEVINTERFACE_HID.Interfaces())
            //{

            //}

            var vvvs = "Volume".Devices(true).Select(x => new
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
