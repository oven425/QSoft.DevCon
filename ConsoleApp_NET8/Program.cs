using QSoft.DevCon;
using System.Runtime.InteropServices;

Span<byte> mem = File.ReadAllBytes("test").AsSpan();
var cc = MemoryMarshal.Cast<byte, char>(mem);
cc.IndexOf('\0'); // 0x00 is the null terminator in UTF-16 encoding

var cameras = DevConExtension.KSCATEGORY_AUDIO.DevicesFromInterface()
                .Select(x => new
                {
                    devicepath = x.DevicePath(),
                    friendname = x.As().GetFriendName(),
                    classname = x.As().GetClass(),
                    panel = x.As().Panel(),
                }).ToList();

var aaa = "Camera".Devices().Select(x => new
{
    name = x.GetFriendName(),
    isconnect = x.IsConnected(),
    preset = x.IsPresent(),
    id = x.DeviceInstanceId(),
    problemcode = x.ProblemCode(),
    powerdata = x.PowerData(),
    ids = x.HardwaeeIDs(),
});
foreach(var oo in aaa)
{
    System.Diagnostics.Trace.WriteLine($"{oo.name}");
    System.Diagnostics.Trace.WriteLine(oo.powerdata?.ToString());
}



Console.ReadLine();
