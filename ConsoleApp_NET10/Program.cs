// See https://aka.ms/new-console-template for more information
using QSoft.DevCon;
Console.WriteLine("Hello, World!");
var aaa = "Camera".Devices().Select(x => new
{
    vp = x.VidPid(),
    name = x.GetFriendName(),
    isconnect = x.IsConnected(),
    preset = x.IsPresent(),
    id = x.DeviceInstanceId(),
    problemcode = x.ProblemCode(),
    powerdata = x.PowerData(),
    ids = x.HardwaeeIDs(),
    panel = x.Panel(),
    //x.DeviceDesc,
    desc = x.GetDeviceDesc(),
    aa = x.CompatibleIDs()
});
foreach (var a in aaa)
{
    Console.WriteLine($"Name:{a.name}");
    Console.WriteLine($"VidPid:{a.vp}");
    Console.WriteLine($"IsConnected:{a.isconnect}");
    Console.WriteLine($"IsPresent:{a.preset}");
    Console.WriteLine($"DeviceInstanceId:{a.id}");
    Console.WriteLine($"ProblemCode:{a.problemcode}");
    Console.WriteLine($"PowerData:{a.powerdata}");
    //Console.WriteLine($"DeviceDesc:{a.DeviceDesc}");
    Console.WriteLine("HardwareIDs:");
    foreach (var id in a.ids)
    {
        Console.WriteLine($"\t{id}");
    }
    Console.WriteLine("CompatibleIDs:");
    foreach (var id in a.aa)
    {
        Console.WriteLine($"\t{id}");
    }
    Console.WriteLine($"Panel:{a.panel}");
    Console.WriteLine("--------------------------------------------------");
}