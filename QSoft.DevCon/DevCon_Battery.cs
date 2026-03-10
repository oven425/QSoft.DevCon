using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    public static partial class DevConExtension
    {
        public static void GetBatteryInfo()
        {
            var batteryguid = "Battery".GetClassGuids().FirstOrDefault();
            var ll = batteryguid.DevicesFromInterface().Select(x => new
            {
                devicepath = x.DevicePath(),
                friendname = x.As().DeviceDesc(),
            });
            foreach (var oo in ll)
            {
                System.Diagnostics.Trace.WriteLine($"{oo.friendname} {oo.devicepath}");
            }
        }
    }
}
