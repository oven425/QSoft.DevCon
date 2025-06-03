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
        static List<string> GetStrings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var ids = new List<string>();
#if NET8_0_OR_GREATER

#else
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out _, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize * 2);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, mem.Pointer, reqsize, out reqsize, 0);
                ids.AddRange(GetStrings(mem.Pointer));
            }
#endif
            return ids;
        }

        static List<string> GetStrings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint property)
        {
            var ids = new List<string>();
#if NET8_0_OR_GREATER

#else
            //SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, property, out var property_type, IntPtr.Zero, 0, out var reqsize);
            //if (reqsize <= 0) return ids;
            //using (var mem = new IntPtrMem<byte>((int)reqsize))
            //{
            //    SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, property, out property_type, mem.Pointer, reqsize, out reqsize);
            //    ids.AddRange(GetStrings(mem.Pointer));
            //}
#endif
            return ids;
        }
#if NET8_0_OR_GREATER
        static List<string> GetStrings(this Span<byte> src)
        {
            var strs = new List<string>();
            var str = Encoding.Unicode.GetString(src);
            strs.Add(str);
            return strs;
        }
#else

        static List<string> GetStrings(this IntPtr src)
        {
            var strs = new List<string>();
            var ptr = src;
            while (true)
            {
                var str = Marshal.PtrToStringUni(ptr);
                if (string.IsNullOrEmpty(str))
                {
                    break;
                }
                var len = str.Length;
                ptr = IntPtr.Add(ptr, len * 2 + 2);
                strs.Add(str);
            }

            return strs;
        }
#endif

    }
}
