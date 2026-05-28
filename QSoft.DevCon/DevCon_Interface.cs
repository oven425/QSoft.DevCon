using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QSoft.DevCon
{
    //[get deviceclss form device interface]("https://github.com/hiyohiyo/CrystalDiskInfo/blob/bdf4e44cc449225ec814c011c5d6c537da3c71fc/EnumVolumeDrive.cpp#L98")
    static public partial class DevConExtension
    {
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata, SP_DEVICE_INTERFACE_DATA interfacedata)> DevicesFromInterface(this Guid guid, bool showhiddendevice = false)
        {
            uint flags = DIGCF_PRESENT | DIGCF_PROFILE;
            if (showhiddendevice)
            {
                flags = DIGCF_PROFILE;
            }
            flags |= DIGCF_DEVICEINTERFACE;
            //if (guid == Guid.Empty)
            //{
            //flags |= DIGCF_ALLCLASSES;
            //}

            Guid ggid = Guid.Empty;
            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(guid, IntPtr.Zero, IntPtr.Zero, flags);
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
            string devpath = "";
            var cbsz = (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8;
            SP_DEVINFO_DATA devinfo = new();
            devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
#if NET8_0_OR_GREATER
            var bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, [], 0, out var reqsize, ref devinfo);
            if(reqsize >0)
            {
                Span<byte> span = stackalloc byte[(int)reqsize];
                MemoryMarshal.Write(span, cbsz);
                uint nBytes = reqsize;
                bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, span, nBytes, out reqsize, ref devinfo);
                devpath = MemoryMarshal.Cast<byte, char>(span[4..]).TrimEnd('\0').ToString();
            }
#else
            var bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, IntPtr.Zero, 0, out var reqsize, ref devinfo);
            var err = Marshal.GetLastWin32Error();
            if (reqsize > 0)
            {
                var ptr = Marshal.AllocHGlobal((int)reqsize);
                try
                {
                    Marshal.WriteInt32(ptr, cbsz);
                    uint nBytes = reqsize;
                    bb = SetupDiGetDeviceInterfaceDetail(src.dev, src.interfaceinfo, ptr, nBytes, out reqsize, ref devinfo);

                    devpath = Marshal.PtrToStringUni(IntPtr.Add(ptr, 4)) ?? "";
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }

            }
#endif
            return devpath;
        }

        public static SafeFileHandle OpenHandle(this string src)
        {

#if NET8_0_OR_GREATER
            return File.OpenHandle(src, FileMode.Open, FileAccess.Read, FileShare.Read|FileShare.Write);
#else
            var handle = CreateFile(src, GENERIC_READ, FILE_SHARE_READ|FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return handle;
#endif

        }


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        // 權限常數
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;

        // 共用模式
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;

        // 建立模式
        const uint OPEN_EXISTING = 3;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public UIntPtr Reserved;
        };

    }
}

