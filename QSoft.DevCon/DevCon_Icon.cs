using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtension;

namespace QSoft.DevCon
{
    public static partial class DevConExtension
    {
        public static IntPtr Icon(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            SetupDiLoadDeviceIcon(src.dev, ref src.devdata, 96, 96, 0, out var icon);

            return icon;
        }

        public static void DestoryIcon(this IntPtr ptr)
        {
            DestoryIcon(ptr);
        }
    }
}
