using QSoft.DevCon;


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
