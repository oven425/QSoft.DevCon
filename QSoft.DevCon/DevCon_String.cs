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
        static string? OrNull(this string src)
        {
            return string.IsNullOrEmpty(src) ? null : src;
        }
        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            string str = "";
#if NET8_0_OR_GREATER
            SetupDiGetDeviceRegistryProperty(src.dev, src.devdata, spdrp, out var property_type, [], 0, out var reqsize);
            if (reqsize <= 2) return "";
            Span<byte> span = stackalloc byte[(int)reqsize];
            SetupDiGetDeviceRegistryProperty(src.dev, src.devdata, spdrp, out property_type, span, reqsize, out reqsize);
            ReadOnlySpan<char> charSpan = MemoryMarshal.Cast<byte, char>(span);
            str = charSpan.TrimEnd('\0').ToString();
#else
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<char>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer) ?? "";
            }
#endif

            return str ?? "";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = "";
#if NET8_0_OR_GREATER
            
            SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out var property_type, [], 0, out var reqsize, 0);
            if (reqsize <= 2) return "";
            Span<byte> span = stackalloc byte[reqsize];
            SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out property_type, span, reqsize, out reqsize, 0);
            str = MemoryMarshal.Cast<byte, char>(span).TrimEnd('\0').ToString();
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
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.Cast<char, byte>(data.AsSpan());
            if(!SetupDiSetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, span_in, (uint)span_in.Length))
            {
                ThrowExceptionForLastError();
            }
#else
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.StringToHGlobalUni(data);
                if (!SetupDiSetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, ptr, (uint)data.Length * 2))
                {
                    ThrowExceptionForLastError();
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
#endif

        }
    }
}
