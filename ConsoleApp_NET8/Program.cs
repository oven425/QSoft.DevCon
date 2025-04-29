using QSoft.DevCon;
using System.Runtime.InteropServices;


var aaa = "Camera".Devices().Select(x => new
{
    name = x.Service()
});
aaa.ToArray();

Console.ReadLine();
