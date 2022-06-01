using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    public class DevMgr
    {
        List<string> Split(byte[] src)
        {
            List<string> dst = new List<string>();
            int begin_idx = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == 0)
                {
                    if (begin_idx == i)
                    {
                        break;
                    }
                    var harwareidss = Encoding.UTF8.GetString(src, begin_idx, i - begin_idx);
                    dst.Add(harwareidss);
                    begin_idx = i + 1;
                }
            }
            return dst;
        }

        public IEnumerable<DeviceInfo> AllDevice()
        {
            List<DeviceInfo> dds = new List<DeviceInfo>();
            Guid DiskGUID = Guid.Empty;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES | DIGCF_DEVICEINTERFACE);

            int err = 0;
            uint index = 0;
            while (true)
            {
                SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                if (SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo) == false)
                {
                    err = Marshal.GetLastWin32Error();
                    yield break;
                }
                else
                {
                    DeviceInfo dev = new DeviceInfo();


                    //StringBuilder hardwareid = new StringBuilder(2048);
                    //SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_HARDWAREID, IntPtr.Zero, hardwareid, hardwareid.Capacity, IntPtr.Zero);
                    //dev.HardwareID = hardwareid.ToString();
                    byte[] buf = new byte[2048];
                    
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_HARDWAREID, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
                    dev.HardwareIDs.AddRange(Split(buf));

                    StringBuilder friendlyname = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_FRIENDLYNAME, IntPtr.Zero, friendlyname, friendlyname.Capacity, IntPtr.Zero);
                    dev.FriendlyName = friendlyname.ToString();
                    StringBuilder devicedesc = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_DEVICEDESC, IntPtr.Zero, devicedesc, devicedesc.Capacity, IntPtr.Zero);
                    dev.Description = devicedesc.ToString();

                    StringBuilder deviceclass = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_CLASS, IntPtr.Zero, deviceclass, deviceclass.Capacity, IntPtr.Zero);
                    dev.Class = deviceclass.ToString();
                    if(string.IsNullOrEmpty(dev.Class) == true)
                    {
                        System.Diagnostics.Trace.WriteLine("");
                    }
                    StringBuilder deviceclassguid = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_CLASSGUID, IntPtr.Zero, deviceclassguid, deviceclassguid.Capacity, IntPtr.Zero);
                    System.Diagnostics.Trace.WriteLine($"class name:{deviceclass.ToString()} {deviceclassguid.ToString()}");
                    if(string.IsNullOrEmpty(deviceclassguid.ToString()) == true)
                    {
                        dev.ClassGuid = Guid.Empty;
                    }
                    else
                    {
                        var guid = Guid.Parse(deviceclassguid.ToString());
                        dev.ClassGuid = guid;
                        StringBuilder classdesc = new StringBuilder(2048);
                        SetupDiGetClassDescription(ref guid, classdesc, classdesc.Capacity, IntPtr.Zero);
                        dev.ClassDescription = classdesc.ToString();
                    }

                    StringBuilder location = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_LOCATION_INFORMATION, IntPtr.Zero, location, location.Capacity, IntPtr.Zero);
                    dev.Location = location.ToString();

                    Array.Clear(buf, 0, buf.Length);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_LOCATION_PATHS, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
                    dev.LocationPaths.AddRange(this.Split(buf));



                    StringBuilder instanceid = new StringBuilder(2048);
                    SetupDiGetDeviceInstanceId(hDevInfo, ref devinfo, instanceid, instanceid.Capacity, IntPtr.Zero);
                    dev.InstanceId = instanceid.ToString();
                    yield return dev;
                }
                index++;
            }

            //return dds;
        }


        
        string GetValueString(IntPtr hDevInfo, ref SP_DEVINFO_DATA devinfo, uint spdrp)
        {
            int property_type = 0;
            int requiresize = 0;
            var hr = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, out property_type, IntPtr.Zero, 0, out requiresize);
            var err = Marshal.GetLastWin32Error();
            if(requiresize == 0)
            {
                return "";
            }
            byte[] buf = new byte[requiresize];
            hr = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
            err = Marshal.GetLastWin32Error();
            var sss = Encoding.UTF8.GetString(buf, 0, buf.Length);

            StringBuilder strb = new StringBuilder(requiresize);
            
            SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return sss;

            //StringBuilder strb = new StringBuilder(requiresize);
            //SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            //return strb.ToString();
        }

        public void AA()
        {
            Guid DiskGUID = Guid.Empty;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES | DIGCF_DEVICEINTERFACE);
            SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            DeviceInterfaceData.cbSize = Marshal.SizeOf(DeviceInterfaceData);

        }

        //public int Enable(Func<DeviceInfo, bool> func)
        //{
        //    int count = 0;
        //    foreach (var dev in this.AllDevice())
        //    {
        //        if (func(dev) == true)
        //        {
        //            dev.ChnageState(true);
        //        }
        //    }

        //    return count;
        //}

        //public int Disable(Func<DeviceInfo, bool> func)
        //{
        //    int count = 0;
        //    foreach (var dev in this.AllDevice())
        //    {
        //        if (func(dev) == true)
        //        {
        //            dev.ChnageState(false);
        //        }
        //    }

        //    return count;
        //}

        Guid GUID_DEVINTERFACE_DISK = new Guid(0x53f56307, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x00, 0xa0, 0xc9, 0x1e, 0xfb, 0x8b);
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,  UInt32 deviceInterfaceDetailDataSize, ref UInt32 requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid interfaceClassGuid;
            public Int32 flags;
            private UIntPtr reserved;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int size;
            public char devicePath;
        }

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //struct SP_DEVICE_INTERFACE_DETAIL_DATA
        //{
        //    public int cbSize;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BUFFER_SIZE)]
        //    public string DevicePath;
        //}

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetClassDescription(ref Guid ClassGuid, StringBuilder ClassDescription, int ClassDescriptionSize, IntPtr RequiredSize);
        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public UIntPtr Reserved;
        }
        const int DIGCF_DEFAULT = 0x1;
        const int DIGCF_PRESENT = 0x2;
        const int DIGCF_ALLCLASSES = 0x4;
        const int DIGCF_PROFILE = 0x8;
        const int DIGCF_DEVICEINTERFACE = 0x10;
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out UInt32 propertyRegDataType, byte[] propertyBuffer, uint propertyBufferSize, out UInt32 requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, IntPtr requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out int propertyRegDataType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, StringBuilder propertyBuffer, int propertyBufferSize, IntPtr requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, string[] propertyBuffer, int propertyBufferSize, IntPtr requiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);
        uint SPDRP_DEVICEDESC = 0x00000000; // DeviceDesc (R/W)
        uint SPDRP_HARDWAREID = (0x00000001);  // HardwareID (R/W)
        uint SPDRP_COMPATIBLEIDS = (0x00000002);  // CompatibleIDs (R/W)
        uint SPDRP_UNUSED0 = (0x00000003);  // unused
        uint SPDRP_SERVICE = (0x00000004); // Service (R/W)
        uint SPDRP_UNUSED1 = (0x00000005); // unused
        uint SPDRP_UNUSED2 = (0x00000006); // unused
        uint SPDRP_CLASS = (0x00000007); // Class (R--tied to ClassGUID)
        uint SPDRP_CLASSGUID = (0x00000008); // ClassGUID (R/W)
        uint SPDRP_DRIVER = (0x00000009); // Driver (R/W)
        uint SPDRP_CONFIGFLAGS = (0x0000000A); // ConfigFlags (R/W)
        uint SPDRP_MFG = (0x0000000B);// Mfg (R/W)
        uint SPDRP_FRIENDLYNAME = (0x0000000C);// FriendlyName (R/W)
        uint SPDRP_LOCATION_INFORMATION = (0x0000000D); // LocationInformation (R/W)
        uint SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = (0x0000000E); // PhysicalDeviceObjectName (R)
        uint SPDRP_CAPABILITIES = (0x0000000F);  // Capabilities (R)
        uint SPDRP_UI_NUMBER = (0x00000010); // UiNumber (R)
        uint SPDRP_UPPERFILTERS = (0x00000011); // UpperFilters (R/W)
        uint SPDRP_LOWERFILTERS = (0x00000012); // LowerFilters (R/W)
        int SPDRP_BUSTYPEGUID = (0x00000013); // BusTypeGUID (R)
        int SPDRP_LEGACYBUSTYPE = (0x00000014);  // LegacyBusType (R)
        int SPDRP_BUSNUMBER = (0x00000015); // BusNumber (R)
        int SPDRP_ENUMERATOR_NAME = (0x00000016); // Enumerator Name (R)
        int SPDRP_SECURITY = (0x00000017); // Security (R/W, binary form)
        int SPDRP_SECURITY_SDS = (0x00000018); // Security (W, SDS form)
        int SPDRP_DEVTYPE = (0x00000019); // Device Type (R/W)
        int SPDRP_EXCLUSIVE = (0x0000001A); // Device is exclusive-access (R/W)
        int SPDRP_CHARACTERISTICS = (0x0000001B); // Device Characteristics (R/W)
        int SPDRP_ADDRESS = (0x0000001C);// Device Address (R)
        int SPDRP_UI_NUMBER_DESC_FORMAT = (0X0000001D); // UiNumberDescFormat (R/W)
        int SPDRP_DEVICE_POWER_DATA = (0x0000001E); // Device Power Data (R)
        int SPDRP_REMOVAL_POLICY = (0x0000001F); // Removal Policy (R)
        int SPDRP_REMOVAL_POLICY_HW_DEFAULT = (0x00000020);// Hardware Removal Policy (R)
        uint SPDRP_REMOVAL_POLICY_OVERRIDE = (0x00000021); // Removal Policy Override (RW)
        uint SPDRP_INSTALL_STATE = (0x00000022);// Device Install State (R)
        uint SPDRP_LOCATION_PATHS = (0x00000023); // Device Location Paths (R)
        int SPDRP_BASE_CONTAINERID = (0x00000024); // Base ContainerID (R)

        int SPDRP_MAXIMUM_PROPERTY = (0x00000025);// Upper bound on ordinals



    }


    public class DeviceInfo
    {
        public string Class { set; get; }
        public string ClassDescription { set; get; }
        public Guid ClassGuid { set; get; }
        public List<string> HardwareIDs { internal set; get; } = new List<string>();
        public string FriendlyName { internal set; get; }
        public string Description { set; get; }
        public string InstanceId { set; get; }
        public string Location { set; get; }
        public List<string> LocationPaths { set; get; } = new List<string>();
        internal void ChnageState(bool isenable)
        {

        }
    }

    //public static class IDeviceEnumable
    //{
    //    public static int Enable(this IEnumerable<DeviceInfo> src)
    //    {
    //        foreach (var oo in src)
    //        {
    //            oo.Enable();
    //        }
    //        return 0;
    //    }
    //}

}
