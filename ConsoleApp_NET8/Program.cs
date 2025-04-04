﻿using QSoft.DevCon;


//var aa = DevConExtension.GUID_DEVINTERFACE_DISK.Interfaces();
//foreach(var oo in aa)
//{
//    Console.WriteLine(oo.filepath);
//    Console.WriteLine($"FriendName:{oo.devclass.GetDeviceDesc()}");
//    Console.WriteLine($"DeviceInstanceId:{oo.devclass.DeviceInstanceId()}");
//}

//var aaa = DevConExtension.GUID_DEVINTERFACE_DISK.DevicesFromInterface(false);
//foreach(var oo in aaa)
//{
//    System.Diagnostics.Trace.WriteLine($"{oo.DeviceInstanceId()}");
//}
//var aaa = "Camera".Devices().Select(x => x.Interface()).ToList();
var lls = Guid.Empty.Devices().Where(x => x.BusRelations().Count>0).ToList();
foreach(var oo in lls)
{
    System.Diagnostics.Trace.WriteLine(oo.GetDeviceDesc());
}
var cameras = "Camera".Devices();
foreach(var cam in cameras)
{
    Console.WriteLine($"{cam.GetDeviceDesc()}");
    var ss = cam.PowerRelations();
    var commm = cam.CompatibleIDs();
    var ss1 = cam.GetPhysicalDeviceObjectName();
    var siblings = cam.Siblings();
    var buss = cam.Parent();
    //cam.SetFriendName("USB2.0 HD UVC WebCam");
}
    

var ll = Guid.Empty.Devices().Select(x => new
{

    objectname = x.GetPhysicalDeviceObjectName(),
    service = x.Service(),
    power_relation = x.GetPowerRelations(),
    mfg = x.Manufacturer(),
    instanceid = x.GetDeviceInstanceId(),
    locationpaths = x.GetLocationPaths(),
    hardwareids = x.GetHardwaeeIDs(),
    friendname = x.GetFriendName(),
    class_guid = x.GetClassGuid(),
    children = x.GetChildrens(),
    parent = x.GetParent(),
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



        System.Diagnostics.Trace.WriteLine($"mfg:{device.mfg}");
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

