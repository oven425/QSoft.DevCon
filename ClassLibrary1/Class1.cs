using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;

namespace ClassLibrary1
{
    static public partial class Class1
    {

        public static IEnumerable<(IntPtr dev, SP_DEVINFO_DATA devdata)> Devices(this Guid guid, bool showhiddendevice=false)
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

        public static string GetChildren(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, DEVPKEY_Device_Children);
        }

        public static string GetParent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, DEVPKEY_Device_Parent);
        }


        public static List<Guid> GetGuids(this string src)
        {
            var guids = new List<Guid>();
            var hr = SetupDiClassGuidsFromName(src, IntPtr.Zero, 0, out var reqsize);
            if(reqsize >1)
            {
                System.Diagnostics.Trace.WriteLine("");
            }
            using (var mem = new IntPtrMem<Guid>((int)reqsize))
            {
                
                var guid = new byte[16];
                var ss = Marshal.SizeOf<Guid>();
                Marshal.Copy(mem.Pointer, guid, 0, guid.Length);
                var gg = new Guid(guid);
                guids.Add(gg);
            }
            return guids;

        }

        public static string GetFriendName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = "";
            str = GetString(src, SPDRP_FRIENDLYNAME);
            return str??"";
        }

        public static string GetDeviceDesc(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = "";
            str = GetString(src, SPDRP_DEVICEDESC);
            return str ?? "";
        }

        public static List<string> GetHardwaeeIDs(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var ids = new List<string>();

            var hr = SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_HARDWAREID, out var property_type, IntPtr.Zero, 0, out  var reqsize);
            if (reqsize <= 0) return ids;
            using (var mem = new IntPtrMem<byte>((int)reqsize))
            {
                hr = SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_HARDWAREID, out property_type,mem.Pointer, reqsize, out reqsize);
                ids.AddRange(GetStrings(mem.Pointer));
            }

            return ids;
        }

        public static List<string> GetLocationPaths(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var ids = new List<string>();
            var hr = SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_LOCATION_PATHS, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if(reqsize<=0)return ids;
            using (var mem = new IntPtrMem<byte>((int)reqsize))
            {
                hr = SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_LOCATION_PATHS, out property_type, mem.Pointer, reqsize, out reqsize);
                ids.AddRange(GetStrings(mem.Pointer));
            }
            return ids;
        }

        static List<string> GetStrings(this IntPtr src)
        {
            var strs = new List<string>();
            var ptr = src;
            while(true)
            {
                var str = Marshal.PtrToStringUni(ptr);
                if(string.IsNullOrEmpty(str))
                {
                    break;
                }
                var len = str.Length;
                ptr = IntPtr.Add(ptr, len * 2 + 2);
                strs.Add(str);
            }

            return strs;
        }


        public static string? GetDeviceInstanceId(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            int reqszie = 0;
            var ii = IntPtr.Zero;
            var bb = SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, ii, 0, out reqszie);
            var s1 = Marshal.SizeOf<char>();
            using (var buffer = new IntPtrMem<char>(reqszie*2))
            {
                SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, buffer.Pointer, reqszie, out reqszie);
                return Marshal.PtrToStringUni(buffer.Pointer);
            }
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
            SetupDiGetClassDescription(guid!, IntPtr.Zero, 0, out var reqsize);
            if(reqsize > 0)
            {
                using(var mem = new IntPtrMem<byte>((int)reqsize *2))
                {
                    SetupDiGetClassDescription(guid!, mem.Pointer, reqsize, out reqsize);
                    str = Marshal.PtrToStringUni(mem.Pointer);
                }
            }
            return str;
        }
        public static string? GetMFG(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            return GetString(src, SPDRP_MFG);
        }
        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = "";
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Device_Children, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using (var mem = new IntPtrMem<byte>(reqsize * 2))
                {
                    SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                    str = Marshal.PtrToStringUni(mem.Pointer);
                }
            }

            return str ?? "";
        }

        static string GetString(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, uint spdrp)
        {
            var str = "";
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using (var mem = new IntPtrMem<byte>((int)reqsize * 2))
                {
                    SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, spdrp, out property_type, mem.Pointer, reqsize, out reqsize);
                    str = Marshal.PtrToStringUni(mem.Pointer);
                }
            }

            return str??"";
        }

        //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-driverversion
        public static string GetDriverVersion(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var str = "";
            uint propertytype = 0;
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Device_DriverVersion, out propertytype, IntPtr.Zero, 0, out var reqsz, 0);
            if (reqsz > 0)
            {
                using (var mem = new IntPtrMem<byte>(reqsz * 2))
                {
                    SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Device_DriverVersion, out propertytype, mem.Pointer, reqsz, out reqsz, 0);
                    str = Marshal.PtrToStringUni(mem.Pointer);

                }
            }
            return str ?? "";
        }



        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public UIntPtr Reserved;
        }

        public const int DIGCF_DEFAULT = 0x1;
        public const int DIGCF_PRESENT = 0x2;
        public const int DIGCF_ALLCLASSES = 0x4;
        public const int DIGCF_PROFILE = 0x8;
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        public const int DICS_ENABLE = 0x00000001;
        public const int DICS_DISABLE = 0x00000002;
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

        [LibraryImport("setupapi.dll",EntryPoint = "SetupDiClassGuidsFromNameW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiClassGuidsFromName([MarshalAs(UnmanagedType.LPWStr)]string ClassName, IntPtr ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDescriptionW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetClassDescription(Guid ClassGuid, IntPtr ClassDescription, uint ClassDescriptionSize, out uint RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, UInt32 flags);

#else
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);


        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiClassGuidsFromName(string ClassName, IntPtr guids, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetClassDescription(Guid ClassGuid, IntPtr ClassDescription, uint ClassDescriptionSize, out uint RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, UInt32 flags);
#endif

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEVPROPKEY
        {
            public Guid fmtid;
            public UInt32 pid;
        }
        //https://www.magnumdb.com/search?q=filename%3A%22FunctionDiscoveryKeys_devpkey.h%22
        internal static DEVPROPKEY DPKEY_Device_PowerRelations = new DEVPROPKEY() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 6 };
        internal static DEVPROPKEY DEVPKEY_Device_Parent = new DEVPROPKEY() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 8 };
        internal static DEVPROPKEY DEVPKEY_Device_Children = new DEVPROPKEY() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 9 };
        //public static DEVPROPKEY DEVPKEY_Device_Connected = new DEVPROPKEY() { fmtid = Guid.Parse("{78C34FC8-104A-4ACA-9EA4-524D52996E57}"), pid = 55 };
        internal static DEVPROPKEY DEVPKEY_Device_DevNodeStatus = new DEVPROPKEY() { fmtid = Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), pid = 2 };
        internal static DEVPROPKEY DEVPKEY_Device_DriverVersion = new DEVPROPKEY() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 3 };
        internal static DEVPROPKEY DEVPKEY_Device_DriverDate = new DEVPROPKEY() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 2 };
        internal static DEVPROPKEY DPKEY_Device_DeviceDesc = new DEVPROPKEY() { fmtid = Guid.Parse("{a45c254e-df1c-4efd-8020-67d146a850e0}"), pid = 2 };
        internal static DEVPROPKEY DEVPKEY_Device_DriverInfSection = new DEVPROPKEY() { fmtid = Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), pid = 6 };
        public const uint SPDRP_DEVICEDESC = 0x00000000;  // DeviceDesc (R/W)
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
