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
        instanceid = x.GetInstanceId(),
        locationpaths = x.GetLocationPaths()
    });
```

3. Enable/Disable camera, need administrator privileges
```c#
"Camera".Devices().Enable();
"Camera".Devices().Disable();
```

4. Get all device class name and class guid
```c#
var class_guid = Guid.Empty.Devices()
    .GroupBy(x => x.GetClass(), x => x.GetClassGuid());
```

5. change friend name
```c#
//change camera friend name
foreach (var oo in "Camera".Devices())
{
    var friendname = oo.GetFriendName();
    oo.SetFriendName($"test {friendname}");
}
```
6. get device path
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
7. get device icon
```c#
using QSoft.DevCon.WPF;

foreach (var oo in "Camera".Devices())
{
    var icon = oo.GetIcon();
}
```

PS: Thanks for [Simple Device Manager](https://www.codeproject.com/Articles/14469/Simple-Device-Manager).
