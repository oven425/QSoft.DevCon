# QSoft.DevCon
## Quick start

1. Get all devices displayname and more info
```c#
var alldevices = Guid.Empty.Devices()
    .Select(x => new
    {
        displayname = x.GetFriendName(),
        description = x.DeviceDesc(),
    });
```
2. Get all serial port info
```c#
var ports = "Ports".Devices()
    .Where(x => x.Service() == "Serial")
    .Select(x => new
    {
        portname = x.GetComPortName(),
        instanceid = x.DeviceInstanceId(),
        locationpaths = x.LocationPaths()
    });
```
3. Get serial port info from Modem
```c#
var ffs = "Modem".Devices()
    .Select(x=>new 
    { 
        port = x.GetComPortName(),
        desc = x.DeviceDesc(),
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
    .GroupBy(x => x.GetClass(), x => x.ClassGuid());
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

7. check datetime is null or empty
```c#
//transform date is default value to null
var alldevices = Guid.Empty.Devices()
    .Select(x => new
    {
        description = x.DeviceDesc(),
        driverdate = x.DriverDate().OrNull()?.ToString("yyyy/MM/dd") ?? "",
    });

```


8. Get device path
```c#   
var cameras = QSoft.DevCon.DevConExtension.KSCATEGORY_VIDEO_CAMERA
    .DevicesFromInterface()
    .Select(x => new
    {
        devicepath = x.DevicePath(),
        friendname = x.As().GetFriendName(),
        panel = x.As().Panel(),
    });
```
9. Get device icon
```c#
using QSoft.DevCon.WPF;

foreach (var oo in "Camera".Devices())
{
    var icon = oo.Icon();
}
```

PS: Thanks for [Simple Device Manager](https://www.codeproject.com/Articles/14469/Simple-Device-Manager).
