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
        [Obsolete("Obsoleted, please use Service")]
        public static string GetService(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_SERVICE);
        [Obsolete("Obsoleted, please use Manufacturer")]
        public static string GetMFG(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_MFG);
        [Obsolete("Obsoleted, please use Parent")]
        public static string GetParent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_Parent);
        [Obsolete("Obsoleted, please use Childrens")]
        public static List<string> GetChildrens(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(DEVPKEY_Device_Children);
        [Obsolete("Obsoleted, please use HardwaeeIDs")]
        public static List<string> GetHardwaeeIDs(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(SPDRP_HARDWAREID);
        [Obsolete("Obsoleted, please use DeviceInstanceId")]
        public static string GetDeviceInstanceId(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = "";
            //SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, IntPtr.Zero, 0, out var reqsize);
            ////System.Diagnostics.Trace.WriteLine($"reqszie:{reqsize}");
            //if (reqsize > 0)
            //{
            //    using var buffer = new IntPtrMem<char>(reqsize);
            //    SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, buffer.Pointer, reqsize, out reqsize);
            //    str = Marshal.PtrToStringUni(buffer.Pointer);
            //}
            return str ?? "";
        }
        [Obsolete("Obsoleted, please use PowerRelations")]
        public static string GetPowerRelations(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_PowerRelations);


    }
}
