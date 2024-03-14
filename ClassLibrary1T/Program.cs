// See https://aka.ms/new-console-template for more information
using ClassLibrary1;
using System;
using System.Linq;
Console.WriteLine("Hello, World!");
var ll = Guid.Empty.Devices().Select(x => new
{
    instanceid = x.GetDeviceInstanceId(),
    locationpaths = x.GetLocationPaths(),
    hardwareids = x.GetHardwaeeIDs(),
    name = x.GetFriendName()
});

try
{
    foreach (var device in ll)
    {
        System.Diagnostics.Trace.WriteLine($"name:{device.name}");
        System.Diagnostics.Trace.WriteLine($"instanceid:{device.instanceid}");
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
