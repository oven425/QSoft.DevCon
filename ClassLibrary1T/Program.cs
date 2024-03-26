// See https://aka.ms/new-console-template for more information
using ClassLibrary1;
using System;
using System.Linq;
Console.WriteLine("Hello, World!");
var guids = "Camera".GetGuids();
var gg = new Guid("{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}");
var ll = Guid.Empty.Devices().Select(x => new
{
    mfg=x.GetMFG(),
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
});

try
{
    foreach (var device in ll)
    {
        System.Diagnostics.Trace.WriteLine($"friend name:{device.friendname}");
        System.Diagnostics.Trace.WriteLine($"children:{device.children}");
        System.Diagnostics.Trace.WriteLine($"parent:{device.parent}");
        System.Diagnostics.Trace.WriteLine($"mfg:{device.mfg}");
        System.Diagnostics.Trace.WriteLine($"drive_version:{device.drive_version}");
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
