using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static (int vid, int pid) VidPid(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var id = src.DeviceInstanceId();
            var match = Regex.Match(id, @"VID_(?<vid>[0-9A-F]{4})&PID_(?<pid>[0-9A-F]{4})", RegexOptions.IgnoreCase);
            if(match.Success)
            {
                if(int.TryParse(match.Groups["vid"].Value, System.Globalization.NumberStyles.HexNumber, null, out var vid)
                &&int.TryParse(match.Groups["pid"].Value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
                {
                    return (vid, pid);
                }
            }
            return (0, 0);
        }

        public static (int vid, int pid) VidPid(this (IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfacedata) src)
        {
            var devicepath = src.DevicePath();
            HidD_GetHidGuid(out var hidguid);
            try
            {
                using FileStream fs = new(devicepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //#if NET8_0_OR_GREATER
                //            using var file = File.OpenHandle(devicepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //#else
                //            using var file = new SafeFileHandle(devicepath, FileAccess.ReadWrite, FileShare.ReadWrite);
                //#endif
                if (HidD_GetAttributes(fs.SafeFileHandle, out var attr))
                {
                    return (attr.VendorID, attr.ProductID);
                }
                var err = Marshal.GetLastWin32Error();
            }
            catch(Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
            
            return (0,0);
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HIDD_ATTRIBUTES
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint Size;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VendorID;
            [MarshalAs(UnmanagedType.U2)]
            public ushort ProductID;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VersionNumber;
        }



        [DllImport("hid.dll", EntryPoint = "HidD_GetHidGuid", SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid Guid);
        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetAttributes(SafeFileHandle HidDeviceObject, out HIDD_ATTRIBUTES Attributes);
    }



}
