using Microsoft.Win32.SafeHandles;
using QSoft.DevCon;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;

//var hids = DevConExtension.GUID_DEVINTERFACE_HID.DevicesFromInterface();
//foreach(var hid in hids)
//{
//    var vp = hid.VidPid();
//    System.Diagnostics.Trace.WriteLine($"vid: {vp.vid:X4}, pid: {vp.pid:X4}");
//}

var usbs = DevConExtension.GUID_DEVINTERFACE_USB_HOST_CONTROLLER.DevicesFromInterface();
foreach(var usb in usbs)
{
    var desc = usb.As().GetDeviceDesc();
    var firendname = usb.As().GetFriendName();
    var devicepath = usb.DevicePath();

    using var ff = File.OpenHandle(devicepath, FileMode.Open);
    var roothubname = ff.GetRootHubName();
    devicepath = $"\\\\.\\{roothubname}";
    using var ff1 = File.OpenHandle(devicepath, FileMode.Open);
    ff1.GET_NODE_INFORMATION();
}

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
    vp = x.VidPid(),
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
    System.Diagnostics.Trace.WriteLine($"{oo.vp.vid} {oo.vp.pid}");
    System.Diagnostics.Trace.WriteLine(oo.powerdata?.ToString());
}



Console.ReadLine();
