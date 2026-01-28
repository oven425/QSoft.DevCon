using Microsoft.Win32.SafeHandles;
using QSoft.DevCon;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;
using static QSoft.DevCon.DevConExtensiona;
//USB xHCI 相容的主機控制器
//USB 根集線器 (USB 3.0)


var subs = "Usb".Devices().Select(x => new
{
    childs = x.Childrens(),
    enumerator = x.EnumeratorName(),
    desc = x.GetDeviceDesc(),
    pps = x.AllPropertys(),
    //x.DeviceDesc,
    instanceid = x.DeviceInstanceId(),
});
var allChildIds = subs.SelectMany(device => device.childs, (x, y) => new {parent = x, child = y });
foreach(var oo in allChildIds)
{
    System.Diagnostics.Trace.WriteLine($"parent: {oo.parent}, child: {oo.child}");
}
var jj = subs.Join(allChildIds, x => x.instanceid.ToUpperInvariant(), y => y.child.ToUpperInvariant(), (x,y)=>new { c = x, p = y.parent});
var ccc = jj.Where(x => x.p.enumerator == "PCI");
var usb32 = ccc.Where(x => x.c.enumerator != "USB4");
foreach(var oo in usb32)
{

}

var usbcontroller = DevConExtension.GUID_DEVINTERFACE_USB_HOST_CONTROLLER.DevicesFromInterface()
    .Select(x => new
    {
        devicepath = x.DevicePath(),
        friendname = x.As().GetFriendName(),
        instanceid = x.As().DeviceInstanceId(),
        childs = x.As().Childrens(),
    });

var usbhub = DevConExtension.GUID_DEVINTERFACE_USB_HUB.DevicesFromInterface()
    .Select(x => new
    {
        devicepath = x.DevicePath(),
        friendname = x.As().GetFriendName(),
        desc = x.As().GetDeviceDesc(),
        instanceid = x.As().DeviceInstanceId(),
        childs = x.As().Childrens(),
    });
var controlers = usbcontroller.SelectMany(x=>x.childs, (parent, child) => new { parent = parent, child = child });
var pc = controlers.Join(usbhub, x => x.child.ToUpperInvariant(), y => y.instanceid.ToUpperInvariant(), (x, y) => new { controller = x.parent, hub = y });
foreach(var oo in pc)
{
    using var ff = File.OpenHandle(oo.controller.devicepath, FileMode.Open);
    using var ff1 = File.OpenHandle(oo.hub.devicepath, FileMode.Open);
    ff1.GET_NODE_INFORMATION();
    var nodeinfo = ff1.NodeInfo();
    for(uint i= 1; i < nodeinfo.HubInformation.HubDescriptor.bNumberOfPorts; i++)
    {
        var pcps = ff1.GetPortConntorProperties(i);
        var nodeEX = ff1.GetNodeConnectionInformationEX(i);
        if (nodeEX.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
        {
            var desc = ff1.GetConfigDescriptor(i);
        }
        else
        {
            var desc = ff1.GetConfigDescriptor(i);
        }
            
    }
    var nodes = ff1.NodeInfo(nodeex => nodeex.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected,
        (nodeex, desc) => new { nodeex, desc });
    var aa3 = nodes.Select(x => new {x.nodeex, usb = x.desc.ParseConfig1((usb, mem) => new { usb, mem }) });
    var aa4 = aa3.SelectMany(x => x.usb, (x, y) => new { x.nodeex.ConnectionIndex, y });
    //var aa1 = nodes.Select(x=>x.desc).ParseConfig1((usb, mem) => new { usb, mem });
    //var aa2 = aa1.SelectMany(x => x.values, (x,y)=>new { x,y});
    //foreach(var oo2 in aa2)
    //{

    //}


    System.Diagnostics.Trace.WriteLine($"controller: {oo.controller.friendname}, hub: {oo.hub.desc}");
}



var usbs = DevConExtension.GUID_DEVINTERFACE_USB_DEVICE.DevicesFromInterface();
foreach(var usb in usbs)
{
    var desc = usb.As().GetDeviceDesc();
    var firendname = usb.As().GetFriendName();
    System.Diagnostics.Trace.WriteLine($"desc: {desc}, friendname: {firendname}");
    var devicepath = usb.DevicePath();

    //using var ff = File.OpenHandle(devicepath, FileMode.Open);
    //var roothubname = ff.GetRootHubName();
    //devicepath = $"\\\\.\\{roothubname}";
    //using var ff1 = File.OpenHandle(devicepath, FileMode.Open);
    //ff1.GET_NODE_INFORMATION();
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
    //x.DeviceDesc,
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

