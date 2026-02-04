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
#if NET8_0_OR_GREATER
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, [], 0, out var reqsize);
            if (reqsize <= 2) return str;
            Span<byte> span = stackalloc byte[(int)reqsize];
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, span, reqsize, out reqsize);
            str = Encoding.Unicode.GetString(span[..^2]);
#else
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }
#endif
            return str;
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            string str = "";
#if NET8_0_OR_GREATER
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, [], 0, out var reqsize);
            if (reqsize <= 2) return "";
            Span<byte> span = stackalloc byte[(int)reqsize];
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, span, reqsize, out reqsize);
            str = Encoding.Unicode.GetString(span[..^2]);
#else
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer, (int)reqsize);
            }
#endif
            return str??"";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = "";
#if NET8_0_OR_GREATER
            
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, [], 0, out var reqsize, 0);
            if (reqsize <= 2) return "";
            Span<byte> span = stackalloc byte[reqsize];
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, span, reqsize, out reqsize, 0);
            //str = Encoding.Unicode.GetString(span[..^2]);
            //var cast = MemoryMarshal.Cast<byte, char>(span);
            str = new string(MemoryMarshal.Cast<byte, char>(span[..^2]));
#else
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>(reqsize);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }
#endif
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
