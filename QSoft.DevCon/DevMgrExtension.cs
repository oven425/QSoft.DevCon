using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> Devices(this Guid guid, bool showhiddendevice = false)
        {
            uint flags = DIGCF_PRESENT | DIGCF_PROFILE;
            if (showhiddendevice)
            {
                flags = DIGCF_PROFILE;
            }
            //flags |= DIGCF_DEVICEINTERFACE;
            if (guid == Guid.Empty)
            {
                flags |= DIGCF_ALLCLASSES;
            }
            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            try
            {
                while (true)
                {
                    SP_DEVINFO_DATA devinfo = new();
                    devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                    if (!SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo))
                    {
                        yield break;
                    }
                    else
                    {
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
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> Devices(this string src, bool showhiddendevice = false)
        {
            var classguids = src.GetClassGuids();
            if(classguids.Count == 0)
            {
                return Enumerable.Empty<(IntPtr dev, SP_DEVINFO_DATA devdata)>();
            }
            return src.GetClassGuids().FirstOrDefault().Devices(showhiddendevice);
        }

        public static int Enable(this IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> src)
        {
            var count = 0;
            foreach (var oo in src)
            {
                oo.ChangeState(true);
                count++;
            }
            return count;
        }

        /// <summary>
        /// need admin
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int Disable(this IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> src)
        {
            var count = 0;
            foreach (var oo in src)
            {
                oo.ChangeState(false);
                count++;
            }
            return count;
        }
        [Flags]
        internal enum FORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }

        static void ThrowExceptionForLastError()
        {
            var error = Marshal.GetLastWin32Error();
            var msg = error.GetLastErrorMessage();
            if(!string.IsNullOrEmpty(msg))
            {
                var ex = new Exception(msg);
                throw ex;
            }
        }

        internal static string GetLastErrorMessage(this int error)
        {
            IntPtr lpBuff = IntPtr.Zero;
            var sMsg = "";

            if (0 != FormatMessage(FORMAT_MESSAGE.ALLOCATE_BUFFER
                   | FORMAT_MESSAGE.FROM_SYSTEM
                   | FORMAT_MESSAGE.IGNORE_INSERTS,
                   IntPtr.Zero,
                   error, 0, ref lpBuff, 0, IntPtr.Zero))
            {
                sMsg = Marshal.PtrToStringUni(lpBuff);
                Marshal.FreeHGlobal(lpBuff);
            }
            
            return sMsg??"";
        }

        static void ChangeState(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, bool isenable)
        {
            SP_PROPCHANGE_PARAMS params1 = new();
            params1.ClassInstallHeader.cbSize = Marshal.SizeOf<SP_CLASSINSTALL_HEADER>();
            params1.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
            params1.Scope = DICS_FLAG_GLOBAL;
            params1.StateChange = isenable ? DICS_ENABLE : DICS_DISABLE;

            if (!SetupDiSetClassInstallParams(src.dev, src.devdata, params1, Marshal.SizeOf<SP_PROPCHANGE_PARAMS>()))
            {
                ThrowExceptionForLastError();
            }

            if (!SetupDiCallClassInstaller((uint)DIF_PROPERTYCHANGE, src.dev, ref src.devdata))
            {
                ThrowExceptionForLastError();
            }
        }

        public static List<string> GetChildrens(this (IntPtr dev, SP_DEVINFO_DATA devdata) src) => src.GetStrings(DEVPKEY_Device_Children);

        public static string GetParent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, DEVPKEY_Device_Parent);
        }
        public static string GetClass(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return src.GetString(SPDRP_CLASS);
        }
        public static IEnumerable<(string letter, string target)> GetVolumeName()
        {
            var drives = System.IO.DriveInfo.GetDrives();
            foreach (var oo in drives.Select(x => x.Name))
            {
                using var mem = new IntPtrMem<byte>(256 * 2);
                QueryDosDevice(oo.Replace("\\", ""), mem.Pointer, 256);
                yield return (oo, Marshal.PtrToStringUni(mem.Pointer) ?? "");
            }

        }

        public static List<Guid> GetClassGuids(this string src)
        {
            var guids = new List<Guid>();
            SetupDiClassGuidsFromName(src, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 1)
            {
                System.Diagnostics.Trace.WriteLine("");
            }
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<Guid>((int)reqsize);
                SetupDiClassGuidsFromName(src, mem.Pointer, reqsize, out reqsize);
                var guid = new byte[16];
                Marshal.Copy(mem.Pointer, guid, 0, guid.Length);
                var gg = new Guid(guid);
                guids.Add(gg);
            }

            return guids;

        }

        public static string GetFriendName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, SPDRP_FRIENDLYNAME);
        }

        public static void SetFriendName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, string data)
        {
            src.SetString(data, SPDRP_FRIENDLYNAME);
        }

        public static string GetDeviceDesc(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, SPDRP_DEVICEDESC);
        }

        public static List<string> GetHardwaeeIDs(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return src.GetStrings(SPDRP_HARDWAREID);
        }

        public static List<string> GetLocationPaths(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)=> src.GetStrings(SPDRP_LOCATION_PATHS);

        static List<string> GetStrings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var ids = new List<string>();
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out _, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize * 2);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, mem.Pointer, reqsize, out reqsize, 0);
                ids.AddRange(GetStrings(mem.Pointer));
            }

            return ids;
        }

        static List<string> GetStrings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint property)
        {
            var ids = new List<string>();
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, property, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize <= 0) return ids;
            using (var mem = new IntPtrMem<byte>((int)reqsize))
            {
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, property, out property_type, mem.Pointer, reqsize, out reqsize);
                ids.AddRange(GetStrings(mem.Pointer));
            }
            return ids;
        }

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

        public static string GetService(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_SERVICE);

        public static string GetPhysicalDeviceObjectName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_PHYSICAL_DEVICE_OBJECT_NAME);

        public static string GetPowerRelations(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DPKEY_Device_PowerRelations);


        public static string GetDeviceInstanceId(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = "";
            SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, IntPtr.Zero, 0, out var reqszie);
            if (reqszie > 0)
            {
                using var buffer = new IntPtrMem<char>(reqszie * 2);
                SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, buffer.Pointer, reqszie, out reqszie);
                str = Marshal.PtrToStringUni(buffer.Pointer);
            }
            return str ?? "";
        }

        public static Guid GetClassGuid(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = GetString(src, SPDRP_CLASSGUID);
            if (Guid.TryParse(str, out var guid))
            {
                return guid;
            }
            return Guid.Empty;
        }

        public static string? GetClassDesc(this Guid guid)
        {
            var str = "";
            SetupDiGetClassDescription(guid, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>((int)reqsize * 2);
                SetupDiGetClassDescription(guid!, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }
            return str;
        }
        public static string? GetMFG(this (IntPtr dev, SP_DEVINFO_DATA devdata) src) => src.GetString(SPDRP_MFG);

#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static string GetComPortName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var hKey1 = SetupDiOpenDevRegKey(src.dev, ref src.devdata, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            using var hKey = new SafeRegistryHandle(hKey1, true);

            if (!hKey.IsInvalid)
            {
                using var reg = RegistryKey.FromHandle(hKey);
                var portname = reg?.GetValue("PortName")?.ToString();
                return portname ?? "";
            }

            return "";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = "";
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize * 2);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
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
                using var mem = new IntPtrMem<byte>((int)reqsize * 2);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                str = Marshal.PtrToStringUni(mem.Pointer);
            }

            return str??"";
        }

        static void SetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, string data, uint spdrp)
        {
            using var mem = new IntPtrMem<byte>(Marshal.StringToHGlobalUni(data));
            if(!SetupDiSetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, mem.Pointer, (uint)data.Length*2))
            {
                ThrowExceptionForLastError();
            }
        }

        //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-driverversion
        public static string GetDriverVersion(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return src.GetString(DEVPKEY_Device_DriverVersion);
        }

        public static string GetDriverInfSection(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return src.GetString(DEVPKEY_Device_DriverInfSection);
        }

        public static DateTime GetDriverDate(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetDateTime(DEVPKEY_Device_DriverDate);
        

        public static string GetDriverProvider(this (IntPtr dev, SP_DEVINFO_DATA devdata) src) => src.GetString(DEVPKEY_Device_DriverProvider);


        static DateTime GetDateTime(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var datetime = DateTime.FromFileTime(0);
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var propertytype, IntPtr.Zero, 0, out var reqsz, 0);
            if(reqsz > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsz);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out propertytype, mem.Pointer, reqsz, out reqsz, 0);
                var tt = Marshal.ReadInt64(mem.Pointer);

                datetime = DateTime.FromFileTime(tt);
            }
            return datetime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public UIntPtr Reserved;
        }

        public const int DIGCF_DEFAULT = 0x1;
        public const int DIGCF_PRESENT = 0x2;
        public const int DIGCF_ALLCLASSES = 0x4;
        public const int DIGCF_PROFILE = 0x8;
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        public const int DICS_ENABLE = 0x00000001;
        const int DICS_DISABLE = 0x00000002;
        public const int DICS_PROPCHANGE = 0x00000003;
        public const int DICS_START = 0x00000004;
        public const int DICS_STOP = 0x00000005;

#if NET8_0_OR_GREATER
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsW")]
        internal static partial IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInstanceIdW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryPropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiSetDeviceRegistryPropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, IntPtr PropertyBuffer, uint PropertyBufferSize);

        [LibraryImport("setupapi.dll",EntryPoint = "SetupDiClassGuidsFromNameW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiClassGuidsFromName([MarshalAs(UnmanagedType.LPWStr)]string ClassName, IntPtr ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDescriptionW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetClassDescription(Guid ClassGuid, IntPtr ClassDescription, uint ClassDescriptionSize, out uint RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, UInt32 flags);


        [LibraryImport("kernel32.dll", EntryPoint = "QueryDosDeviceW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static partial uint QueryDosDevice([MarshalAs(UnmanagedType.LPWStr)]string lpDeviceName, IntPtr lpTargetPath, int ucchMax);


        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiSetClassInstallParamsW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);


        [LibraryImport("Setupapi", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static partial IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope, uint hwProfile, uint parameterRegistryValueKind, int samDesired);
        
        [LibraryImport("kernel32.dll", EntryPoint = "FormatMessageW")]
        [return: MarshalAs(UnmanagedType.I4)]
        static internal partial int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource,
                                 int dwMessageId, int dwLanguageZId,
                                 ref IntPtr lpBuffer, int nSize, IntPtr Arguments);
#else
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);


        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetupDiClassGuidsFromName(string ClassName, IntPtr guids, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, IntPtr PropertyBuffer, uint PropertyBufferSize);




        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetClassDescription(Guid ClassGuid, IntPtr ClassDescription, uint ClassDescriptionSize, out uint RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, UInt32 flags);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint QueryDosDevice(string lpDeviceName, IntPtr lpTargetPath, int ucchMax);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);
        
        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope, uint hwProfile, uint parameterRegistryValueKind, int samDesired);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static internal extern int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource,
                                 int dwMessageId, int dwLanguageZId,
                                 ref IntPtr lpBuffer, int nSize, IntPtr Arguments);

#endif

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_PROPCHANGE_PARAMS
        {
            public SP_PROPCHANGE_PARAMS()
            {
                ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
            }
            public SP_CLASSINSTALL_HEADER ClassInstallHeader;
            public int StateChange;
            public uint Scope;
            public int HwProfile;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        };
        internal const int DIF_PROPERTYCHANGE = (0x00000012);
        [StructLayout(LayoutKind.Sequential)]
        internal struct DEVPROPKEY
        {
            public Guid fmtid;
            public UInt32 pid;
        }
        //https://www.magnumdb.com/search?q=filename%3A%22FunctionDiscoveryKeys_devpkey.h%22
        readonly internal static DEVPROPKEY DPKEY_Device_PowerRelations = new() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 6 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_Parent = new() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 8 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_Children = new() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 9 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_DevNodeStatus = new() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 2 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverVersion = new() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 3 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverDate = new() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 2 };
        readonly internal static DEVPROPKEY DPKEY_Device_DeviceDesc = new() { fmtid = Guid.Parse("{a45c254e-df1c-4efd-8020-67d146a850e0}"), pid = 2 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverInfSection = new() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 6 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverProvider = new() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 9 };
        readonly internal static DEVPROPKEY DEVPKEY_Device_Siblings = new() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 10 };


        readonly internal static uint SPDRP_DEVICEDESC = 0x00000000;  // DeviceDesc (R/W)
        public const uint SPDRP_HARDWAREID = (0x00000001);  // HardwareID (R/W)
        public const uint SPDRP_COMPATIBLEIDS = (0x00000002);  // CompatibleIDs (R/W)
        public const uint SPDRP_UNUSED0 = (0x00000003);  // unused
        public const uint SPDRP_SERVICE = (0x00000004);  // Service (R/W)
        public const uint SPDRP_UNUSED1 = (0x00000005);  // unused
        public const uint SPDRP_UNUSED2 = (0x00000006);  // unused
        public const uint SPDRP_CLASS = (0x00000007);  // Class (R--tied to ClassGUID)
        public const uint SPDRP_CLASSGUID = (0x00000008);  // ClassGUID (R/W)
        public const uint SPDRP_DRIVER = (0x00000009);  // Driver (R/W)
        public const uint SPDRP_CONFIGFLAGS = (0x0000000A);  // ConfigFlags (R/W)
        public const uint SPDRP_MFG = (0x0000000B);  // Mfg (R/W)
        public const uint SPDRP_FRIENDLYNAME = (0x0000000C);  // FriendlyName (R/W)
        public const uint SPDRP_LOCATION_INFORMATION = (0x0000000D);  // LocationInformation (R/W)
        public const uint SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = (0x0000000E);  // PhysicalDeviceObjectName (R)
        public const uint SPDRP_CAPABILITIES = (0x0000000F);  // Capabilities (R)
        public const uint SPDRP_UI_NUMBER = (0x00000010);  // UiNumber (R)
        public const uint SPDRP_UPPERFILTERS = (0x00000011);  // UpperFilters (R/W)
        public const uint SPDRP_LOWERFILTERS = (0x00000012);  // LowerFilters (R/W)
        public const uint SPDRP_BUSTYPEGUID = (0x00000013);  // BusTypeGUID (R)
        public const uint SPDRP_LEGACYBUSTYPE = (0x00000014);  // LegacyBusType (R)
        public const uint SPDRP_BUSNUMBER = (0x00000015);  // BusNumber (R)
        public const uint SPDRP_ENUMERATOR_NAME = (0x00000016);  // Enumerator Name (R)
        public const uint SPDRP_SECURITY = (0x00000017);  // Security (R/W, binary form)
        public const uint SPDRP_SECURITY_SDS = (0x00000018); // Security (W, SDS form)
        public const uint SPDRP_DEVTYPE = (0x00000019); // Device Type (R/W)
        public const uint SPDRP_EXCLUSIVE = (0x0000001A); // Device is exclusive-access (R/W)
        public const uint SPDRP_CHARACTERISTICS = (0x0000001B); // Device Characteristics (R/W)
        public const uint SPDRP_ADDRESS = (0x0000001C); // Device Address (R)
        public const uint SPDRP_UI_NUMBER_DESC_FORMAT = (0X0000001D); // UiNumberDescFormat (R/W)
        public const uint SPDRP_DEVICE_POWER_DATA = (0x0000001E);  // Device Power Data (R)
        public const uint SPDRP_REMOVAL_POLICY = (0x0000001F);  // Removal Policy (R)
        public const uint SPDRP_REMOVAL_POLICY_HW_DEFAULT = (0x00000020);  // Hardware Removal Policy (R)
        public const uint SPDRP_REMOVAL_POLICY_OVERRIDE = (0x00000021);  // Removal Policy Override (RW)
        public const uint SPDRP_INSTALL_STATE = (0x00000022);  // Device Install State (R)
        public const uint SPDRP_LOCATION_PATHS = (0x00000023);  // Device Location Paths (R)
        public const uint SPDRP_BASE_CONTAINERID = (0x00000024);  // Base ContainerID (R)

        public const uint SPDRP_MAXIMUM_PROPERTY = (0x00000025);  // Upper bound on ordinals

        public const uint DICS_FLAG_GLOBAL = 0x00000001;  // make change in all hardware profiles
        public const uint DICS_FLAG_CONFIGSPECIFIC = 0x00000002;  // make change in specified profile only
        public const uint DICS_FLAG_CONFIGGENERAL = 0x00000004;  // 1 or more hardware profile-specific
        public const uint DIREG_DEV = 0x00000001;         // Open/Create/Delete device key
        public const uint DIREG_DRV = 0x00000002;        // Open/Create/Delete driver key
        public const uint DIREG_BOTH = 0x00000004;        // Delete both driver and Device key

        internal const int ERROR_MORE_DATA = 0xEA;
        internal const int ERROR_SUCCESS = 0;
        internal const int READ_CONTROL = 0x00020000;
        internal const int SYNCHRONIZE = 0x00100000;
        internal const int STANDARD_RIGHTS_READ = READ_CONTROL;
        internal const int STANDARD_RIGHTS_WRITE = READ_CONTROL;
        internal const int KEY_QUERY_VALUE = 0x0001;
        internal const int KEY_SET_VALUE = 0x0002;
        internal const int KEY_CREATE_SUB_KEY = 0x0004;
        internal const int KEY_ENUMERATE_SUB_KEYS = 0x0008;
        internal const int KEY_NOTIFY = 0x0010;
        internal const int KEY_CREATE_LINK = 0x0020;
        internal const int KEY_READ = ((STANDARD_RIGHTS_READ |
                            KEY_QUERY_VALUE |
                            KEY_ENUMERATE_SUB_KEYS |
                            KEY_NOTIFY)
                            &
                            (~SYNCHRONIZE));
        internal const int KEY_WRITE = ((STANDARD_RIGHTS_WRITE |
                            KEY_SET_VALUE |
                            KEY_CREATE_SUB_KEY) &
                            (~SYNCHRONIZE));
    }

    internal sealed class IntPtrMem<T> : IDisposable where T : struct
    {
        public IntPtr Pointer
        {
            get
            {
                IntPtr pointer = m_pBuffer;
                if (pointer == IntPtr.Zero && Size != 0)
                {
                    throw new ObjectDisposedException("Pointer");
                }

                return pointer;
            }
        }
        public int Size { private set; get; } = 0;
        public IntPtrMem(int size)
        {
            var s1 = Marshal.SizeOf<T>();
            Size = s1*size;
            m_pBuffer = Marshal.AllocHGlobal(Size);
        }

        public IntPtrMem(IntPtr ptr)
        {
            m_pBuffer = ptr;
        }

        IntPtr m_pBuffer = IntPtr.Zero;
        public void Dispose()
        {
            IntPtr intPtr = Interlocked.Exchange(ref m_pBuffer, IntPtr.Zero);
            if (intPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(intPtr);
            }
        }
    }
}
