using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        static DateTime GetDateTime(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var datetime = DateTime.FromFileTime(0);
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var propertytype, IntPtr.Zero, 0, out var reqsz, 0);
            if (reqsz > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsz);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out propertytype, mem.Pointer, reqsz, out reqsz, 0);
                var tt = Marshal.ReadInt64(mem.Pointer);
                datetime = DateTime.FromFileTime(tt);
            }
            return datetime;
        }

    }
}
