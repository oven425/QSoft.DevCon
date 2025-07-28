using QSoft.DevCon;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;


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
    panel = x.Panel(),
    aa = x.CompatibleIDs()
});
foreach(var oo in aaa)
{
    System.Diagnostics.Trace.WriteLine($"panel: {oo.panel}");
    System.Diagnostics.Trace.WriteLine($"{oo.name}");
    System.Diagnostics.Trace.WriteLine(oo.powerdata?.ToString());
}



Console.ReadLine();
