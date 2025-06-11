using QSoft.DevCon;

var alldevices = Guid.Empty.Devices()
    .Select(x => new
    {
        displayname = x.GetFriendName(),
        description = x.GetDeviceDesc()
    });

var ports = "Ports".Devices()
    .Where(x => x.Service() == "Serial")
    .Select(x => new
    {
        portname = x.GetComPortName(),
        instanceid = x.DeviceInstanceId(),
        locationpaths = x.GetLocationPaths()
    });

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
    id = x.DeviceInstanceId(),
    name = x.Service(),
    problemcode = x.ProblemCode(),
    powerdata = x.PowerData(),
});
aaa.ToArray();

Console.ReadLine();
