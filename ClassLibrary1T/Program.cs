// See https://aka.ms/new-console-template for more information
using ClassLibrary1;
using System;
using System.Linq;
Console.WriteLine("Hello, World!");
var gg = Class1.GetVolumeName().ToList();

var cameras = "Camera".Devices().Select(x => new
{
    name = x.GetFriendName(),
});
foreach (var cam in cameras)
{

}
var ll = Guid.Empty.Devices().Select(x => new
{

    objectname = x.GetPhysicalDeviceObjectName(),
    service = x.GetService(),
    power_relation = x.GetPowerRelations(),
    mfg = x.GetMFG(),
    instanceid = x.GetDeviceInstanceId(),
    locationpaths = x.GetLocationPaths(),
    hardwareids = x.GetHardwaeeIDs(),
    friendname = x.GetFriendName(),
    class_guid = x.GetClassGuid(),
    children = x.GetChildren(),
    parent = x.GetParent(),
    desc = x.GetDeviceDesc(),
    class_name = x.GetClassGuid().GetClassDesc(),
    drive_version = x.GetDriverVersion(),
    driver_inf = x.GetDriverInfSection(),
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
        


        System.Diagnostics.Trace.WriteLine($"mfg:{device.mfg}");
        System.Diagnostics.Trace.WriteLine($"driver_inf:{device.driver_inf}");
        System.Diagnostics.Trace.WriteLine($"drive_version:{device.drive_version}");
        System.Diagnostics.Trace.WriteLine($"driver_date:{device.driver_date}");
        System.Diagnostics.Trace.WriteLine($"instanceid:{device.instanceid}");
        System.Diagnostics.Trace.WriteLine($"clss guid:{device.class_guid}");
        System.Diagnostics.Trace.WriteLine($"classname:{device.class_name}");
        System.Diagnostics.Trace.WriteLine($"desc:{device.desc}");
        System.Diagnostics.Trace.WriteLine($"locationpaths:");
        foreach(var oo in  device.locationpaths)
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
//var aa = "Camera".GetDevClass();
//foreach (var x in aa)
//{
//    System.Diagnostics.Debug.WriteLine(x);
//}
