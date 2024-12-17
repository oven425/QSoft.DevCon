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
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> DevicesFromInterface(this Guid guid, bool showhiddendevice)
        {
            uint flags = DIGCF_PRESENT;
            if (showhiddendevice)
            {
                flags = DIGCF_PROFILE;
            }
            flags |= DIGCF_DEVICEINTERFACE;
            //if (guid == Guid.Empty)
            //{
            //    flags |= DIGCF_ALLCLASSES;
            //}


            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            try
            {
                while (true)
                {
                    SP_DEVICE_INTERFACE_DATA interfaceinfo = new();
                    interfaceinfo.cbSize = (uint)Marshal.SizeOf(interfaceinfo);
                    //IntPtrMem<SP_DEVICE_INTERFACE_DATA> interfaceinfo = new();
                    //interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
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
                        var err = Marshal.GetLastWin32Error();
                        var ptr = Marshal.AllocHGlobal((int)reqsize);
                        Marshal.WriteInt32(ptr, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
                        uint nBytes = reqsize;
                        bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo, ptr, nBytes, out reqsize, ref devinfo);

                        byte[] bb1 = new byte[nBytes];
                        Marshal.Copy(ptr, bb1, 0, bb1.Length);
                        var po = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4));
                        Marshal.FreeHGlobal(ptr);

                        yield return (hDevInfo, devinfo);
                    }
                    index++;
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }

        }

        public static string Interface(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            //var bb = SetupDiGetDeviceInterfaceDetail(src.dev, interfaceinfo, IntPtr.Zero, 0, out var reqsize, ref devinfo);
            //var err = Marshal.GetLastWin32Error();
            //var ptr = Marshal.AllocHGlobal((int)reqsize);
            //Marshal.WriteInt32(ptr, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
            //uint nBytes = reqsize;
            //bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo, ptr, nBytes, out reqsize, ref devinfo);

            //byte[] bb1 = new byte[nBytes];
            //Marshal.Copy(ptr, bb1, 0, bb1.Length);
            //var po = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4));
            //Marshal.FreeHGlobal(ptr);
            return "";
        }

        //public static IEnumerable<(string filepath, (IntPtr dev, SP_DEVINFO_DATA devdata) devclass)> Interfaces(this Guid guid, bool showhiddendevice = false)
        //{
        //    uint flags = DIGCF_PRESENT;
        //    if (showhiddendevice)
        //    {
        //        flags = DIGCF_PROFILE;
        //    }
        //    flags |= DIGCF_DEVICEINTERFACE;
        //    //if (guid == Guid.Empty)
        //    //{
        //    //    flags |= DIGCF_ALLCLASSES;
        //    //}


        //    uint index = 0;
        //    IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
        //    try
        //    {
        //        while (true)
        //        {
        //            SP_DEVINFO_DATA devinfo = new();
        //            devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);

        //            SP_DEVICE_INTERFACE_DATA interfaceinfo = new();
        //            interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
        //            //IntPtrMem<SP_DEVICE_INTERFACE_DATA> interfaceinfo = new();
        //            //interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
        //            if (!SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, guid, index, out interfaceinfo))
        //            {
        //                var err = Marshal.GetLastWin32Error();
        //                yield break;
        //            }
        //            else
        //            {
        //                //#if !NET8_0_OR_GREATER
        //                var bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo,  IntPtr.Zero, 0, out var reqsize, ref devinfo);
        //                var err = Marshal.GetLastWin32Error();
        //                var ptr = Marshal.AllocHGlobal((int)reqsize);
        //                Marshal.WriteInt32(ptr, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
        //                uint nBytes = reqsize;
        //                bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo, ptr, nBytes, out reqsize, ref devinfo);

        //                byte[] bb1 = new byte[nBytes];
        //                Marshal.Copy(ptr, bb1, 0, bb1.Length);
        //                var po = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4));
        //                Marshal.FreeHGlobal(ptr);
                        
                        
        //                //#endif
        //                yield return (po,(hDevInfo, devinfo));
        //            }
        //            index++;
        //        }
        //    }
        //    finally
        //    {
        //        SetupDiDestroyDeviceInfoList(hDevInfo);
        //    }
        //}

        //public static string Interface(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        //{
        //    uint index = 0;
        //    while (true)
        //    {
        //        SP_DEVINFO_DATA devinfo = new();
        //        devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);

        //        SP_DEVICE_INTERFACE_DATA interfaceinfo = new();
        //        interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
        //        //IntPtrMem<SP_DEVICE_INTERFACE_DATA> interfaceinfo = new();
        //        //interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
        //        if (!SetupDiEnumDeviceInterfaces(src.dev, src.devdata, Guid.Empty, index, out interfaceinfo))
        //        {
        //            var err = Marshal.GetLastWin32Error();
        //            break;
        //        }
        //        else
        //        {
        //            //#if !NET8_0_OR_GREATER
        //            var bb = SetupDiGetDeviceInterfaceDetail(src.dev, interfaceinfo, IntPtr.Zero, 0, out var reqsize, IntPtr.Zero);
        //            var err = Marshal.GetLastWin32Error();
        //            var ptr = Marshal.AllocHGlobal((int)reqsize);
        //            Marshal.WriteInt32(ptr, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
        //            uint nBytes = reqsize;
        //            bb = SetupDiGetDeviceInterfaceDetail(src.dev, interfaceinfo, ptr, nBytes, out reqsize, IntPtr.Zero);

        //            byte[] bb1 = new byte[nBytes];
        //            Marshal.Copy(ptr, bb1, 0, bb1.Length);
        //            var po = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4));
        //            Marshal.FreeHGlobal(ptr);


        //            //#endif
        //            //yield return (hDevInfo, interfaceinfo);
        //        }
        //        index++;
        //    }
        //    return "";
        //}
        
        
        
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, Guid InterfaceClassGuid, uint MemberIndex, out SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, Guid InterfaceClassGuid, uint MemberIndex, out SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);
        //[DllImport("setupapi.dll", SetLastError = true)]
        //static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, out SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, uint DeviceInterfaceDetailDataSize, out uint RequiredSize, out SP_DEVINFO_DATA DeviceInfoData);
        //[DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInterfaceDetailW", CharSet = CharSet.Ansi, SetLastError = true)]
        //static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, uint DeviceInterfaceDetailDataSize, out uint RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);
    }

    
    


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public UIntPtr Reserved;
    };

    struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        uint cbSize;
        //char DevicePath[ANYSIZE_ARRAY];
    };
}

