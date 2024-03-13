// See https://aka.ms/new-console-template for more information
using ClassLibrary1;
using System;
using System.Linq;
Console.WriteLine("Hello, World!");
var ll = Guid.Empty.Devices().Select(x => x.GetFriendName());

try
{
    foreach (var device in ll)
    {
        System.Diagnostics.Trace.WriteLine(device);
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
