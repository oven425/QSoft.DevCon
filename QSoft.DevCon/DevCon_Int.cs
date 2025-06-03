using System;
using System.Runtime.InteropServices;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        static int GetInt32(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {

            var str = 0;
#if NET8_0_OR_GREATER
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, [], 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                Span<byte> mem = stackalloc byte[reqsize];
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem, reqsize, out reqsize, 0);
                str = BitConverter.ToInt32(mem);
            }

#else


            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                str = Marshal.ReadInt32(mem.Pointer);
            }
#endif
            return str;
        }
    }
}
