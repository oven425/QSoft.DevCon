using QSoft.DevCon;


var aaa = "Camera".Devices().Select(x => new
{
    id = x.DeviceInstanceId(),
    name = x.Service(),
    problemcode = x.ProblemCode(),
    powerdata = x.PowerData(),
});
aaa.ToArray();

Console.ReadLine();
