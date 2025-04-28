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
        static string? GetStringNull(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            string? str = null;
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }

            return str;
        }


        static string GetString1(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            var str = "";
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
#if NET8_0_OR_GREATER
                Span<byte> span = stackalloc byte[(int)reqsize];
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, span, reqsize, out reqsize);
#endif
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }

            return str ?? "";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            var str = "";
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }

            return str ?? "";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = "";
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>(reqsize);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }

            return str ?? "";
        }
        
        static void SetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, string data, uint spdrp)
        {
            using var mem = new IntPtrMem<byte>(Marshal.StringToHGlobalUni(data));
            if (!SetupDiSetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, mem.Pointer, (uint)data.Length * 2))
            {
                ThrowExceptionForLastError();
            }
        }

        


    }
}
