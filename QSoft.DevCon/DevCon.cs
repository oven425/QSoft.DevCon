using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> Devices(this Guid guid, bool showhiddendevice = false)
        {
            //uint flags = DIGCF_PRESENT | DIGCF_PROFILE| DIGCF_ALLCLASSES;
            uint flags = DIGCF_PRESENT | DIGCF_PROFILE;
            if (showhiddendevice)
            {
                flags = DIGCF_PROFILE;
            }
            //flags |= DIGCF_DEVICEINTERFACE;
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
                return [];
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

        public static List<string> Childrens(this (IntPtr dev, SP_DEVINFO_DATA devdata) src) 
            => src.GetStrings(DEVPKEY_Device_Children);

        public static string Parent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_Parent);

        public static string GetClass(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_CLASS);
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
            => GetString(src, SPDRP_FRIENDLYNAME);

        public static void SetFriendName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, string data)
            => src.SetString(data, SPDRP_FRIENDLYNAME);

        public static string GetDeviceDesc(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => GetString(src, SPDRP_DEVICEDESC);

        public static List<string> HardwaeeIDs(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(SPDRP_HARDWAREID);

        public static List<string> GetLocationPaths(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(SPDRP_LOCATION_PATHS);

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

        
        public static string Service(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_SERVICE);

        public static string GetPhysicalDeviceObjectName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_PHYSICAL_DEVICE_OBJECT_NAME);

        public static string PowerRelations(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DPKEY_Device_PowerRelations);
        public static int ProblemCode(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetInt32(DEVPKEY_Device_ProblemCode);

        public static bool IsConnected(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetBoolean(DEVPKEY_Device_IsConnected);

        public static bool IsPresent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetBoolean(DEVPKEY_Device_IsPresent);

        static List<DEVPROPKEY> GetDevicePropertyKeys(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var bb = SetupDiGetDevicePropertyKeys(src.dev, ref src.devdata, IntPtr.Zero, 0, out var cc, 0);
            var ss = Marshal.SizeOf<DEVPROPKEY>();
            ss *= (int)cc;
            var ptr = Marshal.AllocHGlobal((int)ss);
            bb = SetupDiGetDevicePropertyKeys(src.dev, ref src.devdata, ptr, cc, out cc, 0);
            List<DEVPROPKEY> keys = [];
            for (int i = 0; i < cc; i++)
            {
                ptr = IntPtr.Add(ptr, Marshal.SizeOf<DEVPROPKEY>());
                var kk = Marshal.PtrToStructure<DEVPROPKEY>(ptr);
                keys.Add(kk);
            }
            var aa = keys.ToLookup(x => x.fmtid);
            foreach (var oo in aa)
            {

                foreach (var ooo in oo)
                {
                    System.Diagnostics.Trace.WriteLine($"{oo.Key} {ooo.pid}");
                }

            }
            return [];
        }

        public static string DeviceInstanceId(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_InstanceId);


        //public static string DeviceInstanceId(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        //{
        //    var str = "";
        //    SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, IntPtr.Zero, 0, out var reqsize);
        //    //System.Diagnostics.Trace.WriteLine($"reqszie:{reqsize}");
        //    if (reqsize > 0)
        //    {
        //        using var buffer = new IntPtrMem<char>(reqsize);
        //        SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, buffer.Pointer, reqsize, out reqsize);
        //        str = Marshal.PtrToStringUni(buffer.Pointer);
        //    }
        //    return str ?? "";
        //}

        public static Guid GetClassGuid(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            if(Guid.TryParse(GetString(src, SPDRP_CLASSGUID), out var guid))
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

        public static string Manufacturer(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_MFG);







        //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-driverversion
        public static string GetDriverVersion(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_DriverVersion);

        public static string DriverInfSection(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_DriverInfSection);

        public static DateTime GetDriverDate(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetDateTime(DEVPKEY_Device_DriverDate);
        
        public static string DriverProvider(this (IntPtr dev, SP_DEVINFO_DATA devdata) src) 
            => src.GetString(DEVPKEY_Device_DriverProvider);

        public static string BiosDeviceName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_BiosDeviceName);

        public static DateTime FirstInstallDate(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetDateTime(DEVPKEY_Device_FirstInstallDate);

        public static List<string> Siblings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(DEVPKEY_Device_Siblings);

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



        internal const int DEVPROP_TYPE_EMPTY = 0x00000000;  // nothing, no property data
        internal const int DEVPROP_TYPE_NULL= 0x00000001;  // null property data
        internal const int DEVPROP_TYPE_SBYTE = 0x00000002;  // 8-bit signed int (SBYTE)
//#define DEVPROP_TYPE_BYTE                       0x00000003  // 8-bit unsigned int (BYTE)
//#define DEVPROP_TYPE_INT16                      0x00000004  // 16-bit signed int (SHORT)
//#define DEVPROP_TYPE_UINT16                     0x00000005  // 16-bit unsigned int (USHORT)
//#define DEVPROP_TYPE_INT32                      0x00000006  // 32-bit signed int (LONG)
//#define DEVPROP_TYPE_UINT32                     0x00000007  // 32-bit unsigned int (ULONG)
//#define DEVPROP_TYPE_INT64                      0x00000008  // 64-bit signed int (LONG64)
//#define DEVPROP_TYPE_UINT64                     0x00000009  // 64-bit unsigned int (ULONG64)
//#define DEVPROP_TYPE_FLOAT                      0x0000000A  // 32-bit floating-point (FLOAT)
//#define DEVPROP_TYPE_DOUBLE                     0x0000000B  // 64-bit floating-point (DOUBLE)
//#define DEVPROP_TYPE_DECIMAL                    0x0000000C  // 128-bit data (DECIMAL)
//#define DEVPROP_TYPE_GUID                       0x0000000D  // 128-bit unique identifier (GUID)
//#define DEVPROP_TYPE_CURRENCY                   0x0000000E  // 64 bit signed int currency value (CURRENCY)
//#define DEVPROP_TYPE_DATE                       0x0000000F  // date (DATE)
//#define DEVPROP_TYPE_FILETIME                   0x00000010  // file time (FILETIME)
//#define DEVPROP_TYPE_BOOLEAN                    0x00000011  // 8-bit boolean (DEVPROP_BOOLEAN)
//#define DEVPROP_TYPE_STRING                     0x00000012  // null-terminated string
//#define DEVPROP_TYPE_STRING_LIST (DEVPROP_TYPE_STRING|DEVPROP_TYPEMOD_LIST) // multi-sz string list
//#define DEVPROP_TYPE_SECURITY_DESCRIPTOR        0x00000013  // self-relative binary SECURITY_DESCRIPTOR
//#define DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING 0x00000014  // security descriptor string (SDDL format)
//#define DEVPROP_TYPE_DEVPROPKEY                 0x00000015  // device property key (DEVPROPKEY)
//#define DEVPROP_TYPE_DEVPROPTYPE                0x00000016  // device property type (DEVPROPTYPE)
//#define DEVPROP_TYPE_BINARY      (DEVPROP_TYPE_BYTE|DEVPROP_TYPEMOD_ARRAY)  // custom binary data
//#define DEVPROP_TYPE_ERROR                      0x00000017  // 32-bit Win32 system error code
//#define DEVPROP_TYPE_NTSTATUS                   0x00000018  // 32-bit NTSTATUS code
//#define DEVPROP_TYPE_STRING_INDIRECT            0x00000019  // string resource (@[path\]<dllname>,-<strId>)

    }

}
