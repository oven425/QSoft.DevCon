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
var ports = "Ports".Devices().Where(x => x.GetService() == "Serial")
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
PS:
version 1.x.x.x change to 2.x.x.x

|  Old(1.x.x.x)  | New(2.x.x.x)  |
|  ----  | ----  |
| GetChildren()  | GetChildrens().FirstOrDefault() |
| GetInstanceId()  | GetDeviceInstanceId() |
| GetDevClass()  | GetClassGuids() |
| GetDisplayName()  | GetFriendName() |


PS: Thanks for [Simple Device Manager](https://www.codeproject.com/Articles/14469/Simple-Device-Manager).
