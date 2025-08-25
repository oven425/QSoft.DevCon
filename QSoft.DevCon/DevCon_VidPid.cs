using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        //vid pid regrex
        //var match = Regex.Match(deviceId, @"VID_([0-9A-F]{4})&PID_([0-9A-F]{4})", RegexOptions.IgnoreCase);
        public static (int vid, int pid) VidPid(this (IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfacedata) src)
        {
            var devicepath = src.DevicePath();
            HidD_GetHidGuid(out var hidguid);
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
            return (0, 0);
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
