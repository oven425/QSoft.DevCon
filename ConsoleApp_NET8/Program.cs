using QSoft.DevCon;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;

Span<byte> mem = File.ReadAllBytes("test").AsSpan();

var lls = GetStrings(mem);
lls.Clear();

static List<string> GetStrings(Span<byte> src)
{
    var cc = MemoryMarshal.Cast<byte, char>(src);
    var list = new List<string>();

    while (!cc.IsEmpty)
    {
        int index = cc.IndexOf('\0');
        if (index == -1) // 找不到分隔符，表示這是最後一段
        {
            list.Add(cc.ToString());
            break;
        }

        var str = cc[..index].ToString();
        if (String.IsNullOrEmpty(str))
        {
            break;
        }
        list.Add(str);
        cc = cc[(index + 1)..];
    }
    return list;
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
    name = x.GetFriendName(),
    isconnect = x.IsConnected(),
    preset = x.IsPresent(),
    id = x.DeviceInstanceId(),
    problemcode = x.ProblemCode(),
    powerdata = x.PowerData(),
    ids = x.HardwaeeIDs(),
});
foreach(var oo in aaa)
{
    System.Diagnostics.Trace.WriteLine($"{oo.name}");
    System.Diagnostics.Trace.WriteLine(oo.powerdata?.ToString());
}



Console.ReadLine();
