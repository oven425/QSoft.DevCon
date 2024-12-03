using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtension;

namespace QSoft.DevCon
{
    public static partial class DevMgrExtension
    {
        public static IntPtr Icon(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
#if NET5_0_OR_GREATER

#else
            SetupDiLoadDeviceIcon(src.dev, src.devdata, 96, 96, 0, out var icon);
#endif
        }

    }
}
