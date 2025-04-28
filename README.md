# QSoft.DevCon
## Quick start

1. Get all devices displayname
```c#
var alldevices = Guid.Empty.Devices()
    .Select(x => new
    {
        displayname = x.GetDisplayName(),
        description = x.GetDescription()
    });
```
2. Get all serial port info
```c#
var ports = "Ports".Devices()
    .Where(x => x.GetService() == "Serial")
    .Select(x => new
    {
        portname = x.GetComPortName(),
        instanceid = x.DeviceInstanceId(),
        locationpaths = x.GetLocationPaths()
    });
```
3. Get serial port info from Modem
```c#
var ffs = "Modem".Devices()
    .Select(x=>new 
    { 
        port = x.GetComPortName(),
        desc = x.GetDeviceDesc(),
    });

```

4. Enable/Disable camera, need administrator privileges
```c#
"Camera".Devices().Enable();
"Camera".Devices().Disable();
```

5. Get all device class name and class guid
```c#
var class_guid = Guid.Empty.Devices()
    .GroupBy(x => x.GetClass(), x => x.GetClassGuid());
```

6. Change friend name
```c#
//change camera friend name
foreach (var oo in "Camera".Devices())
{
    var friendname = oo.GetFriendName();
    oo.SetFriendName($"test {friendname}");
}
```
7. Get device path
```c#   
var cameras = DevConExtension.KSCATEGORY_VIDEO_CAMERA
    .DevicesFromInterface()
    .Select(x => new
    {
        devicepath = x.DevicePath(),
        friendname = x.As().GetFriendName(),
        panel = x.As().Panel(),
    });
```
8. Get device icon
```c#
using QSoft.DevCon.WPF;

foreach (var oo in "Camera".Devices())
{
    var icon = oo.GetIcon();
}
```

PS: Thanks for [Simple Device Manager](https://www.codeproject.com/Articles/14469/Simple-Device-Manager).
