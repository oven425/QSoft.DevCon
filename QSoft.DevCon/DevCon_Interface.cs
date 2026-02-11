using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace QSoft.DevCon
{
    //[get deviceclss form device interface]("https://github.com/hiyohiyo/CrystalDiskInfo/blob/bdf4e44cc449225ec814c011c5d6c537da3c71fc/EnumVolumeDrive.cpp#L98")
    static public partial class DevConExtension
    {
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfacedata)> DevicesFromInterface(this Guid guid, bool showhiddendevice = false)
        {
            uint flags = DIGCF_PRESENT;
            if (showhiddendevice)
            {
                flags |= DIGCF_PROFILE;
            }
            flags |= DIGCF_DEVICEINTERFACE;
            //if (guid == Guid.Empty)
            //{
                //flags |= DIGCF_ALLCLASSES;
            //}

            Guid ggid = Guid.Empty;
            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            if (hDevInfo == new IntPtr(-1))
            {
                yield break;
            }
            try
            {
                while (true)
                {
                    SP_DEVICE_INTERFACE_DATA interfaceinfo = new();
                    interfaceinfo.cbSize = (uint)Marshal.SizeOf(interfaceinfo);
                    if (!SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, guid, index, out interfaceinfo))
                    {
                        var err = Marshal.GetLastWin32Error();
                        yield break;
                    }
                    else
                    {
                        //#if !NET8_0_OR_GREATER
                        SP_DEVINFO_DATA devinfo = new();
                        devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                        var bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo, IntPtr.Zero, 0, out var reqsize, ref devinfo);
                        yield return (hDevInfo, devinfo, interfaceinfo);
                    }
                    index++;
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }

        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> As(this IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfaceinfo)> src)
            => src.Select(x => (x.dev, x.devdata));

        public static (IntPtr dev, SP_DEVINFO_DATA devdata) As(this (IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfaceinfo) src)
            => (src.dev, src.devdata);

        public static string DevicePath(this (IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfaceinfo) src)
        {
            SP_DEVINFO_DATA devinfo = new();
            devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
            var bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, IntPtr.Zero, 0, out var reqsize, ref devinfo);
            var err = Marshal.GetLastWin32Error();
            if(reqsize >0)
            {
                var ptr = Marshal.AllocHGlobal((int)reqsize);
                try
                {
                    Marshal.WriteInt32(ptr, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
                    uint nBytes = reqsize;
                    bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, ptr, nBytes, out reqsize, ref devinfo);

                    byte[] bb1 = new byte[nBytes];
                    Marshal.Copy(ptr, bb1, 0, bb1.Length);
                    var po = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4))??"";
                    Marshal.FreeHGlobal(ptr);
                    return po;
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
                
            }
            return "";
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public UIntPtr Reserved;
    };

}

