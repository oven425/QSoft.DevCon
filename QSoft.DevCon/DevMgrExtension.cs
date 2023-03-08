using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.SetupApi;

namespace QSoft.DevCon
{
    public static class DevMgrExtension
    {
        public static string GetComPortName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            var hKey = SetupDiOpenDevRegKey(src.dev, ref src.devdata, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            //Console.WriteLine($"hKey.IsInvalid:{hKey.IsInvalid}");
            if (hKey.IsInvalid == false)
            {
                var reg = RegistryKey.FromHandle(hKey);
                var subkeynames = reg.GetSubKeyNames();
                if(subkeynames.Contains("PortName") == true)
                {
                    var portname = reg.GetValue("PortName").ToString();
                    reg.Dispose();
                    return portname;
                }
                
            }
            return "";
        }

        public static string GetService(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_SERVICE, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            if (hr == false)
            {
            }
            return strb.ToString();
        }

        //public static int GetInstallState(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        //{
        //    uint installstate = 0;
        //    var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_INSTALL_STATE, IntPtr.Zero, out installstate, 4, IntPtr.Zero);
        //    if (hr == false)
        //    {
        //        Console.WriteLine($"GetInstallState: {(uint)Marshal.GetLastWin32Error()}");
        //    }
        //    return 0;
        //}

        public static bool IsConnect(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            uint installstate = 0;
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_INSTALL_STATE, IntPtr.Zero, out installstate, 4, IntPtr.Zero);

            return hr;
        }

        public static string GetHardwaeeID(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_HARDWAREID, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            if (hr == false)
            {
            }
            return null;
        }

        public static List<string> GetHardwaeeIDs(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            List<string> ids = new List<string>();
            int reqsize = 0;
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_HARDWAREID, IntPtr.Zero, null, 0, out reqsize);
            if(reqsize == 0)
            {
                return ids;
            }
            byte[] bb = new byte[reqsize];
            hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_HARDWAREID, IntPtr.Zero, bb, bb.Length, out reqsize);
            ids = bb.Split(reqsize);
            return ids;
        }

        public static uint GetCapabilities(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            uint dd = 0;
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_CAPABILITIES, IntPtr.Zero, out dd, 8, IntPtr.Zero);
            return dd;
        }

        public static string GetDisplayName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb=null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_FRIENDLYNAME, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            if(hr == false)
            {
                hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICEDESC, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            }
            return strb.ToString();
        }

        public static string GetFriendName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_FRIENDLYNAME, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            if(hr == false)
            {
                //throw new Exception($"err:{Marshal.GetLastWin32Error()}");
                //Console.WriteLine($"err:{Marshal.GetLastWin32Error()}");
            }
            return strb.ToString();
        }

        public static List<string> GetLocationPaths(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            byte[] bb = null;
            int reqsize = 0;
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_LOCATION_PATHS, IntPtr.Zero, null, 0, out reqsize);
            if(reqsize == 0)
            {
                return new List<string>();
            }
            bb = new byte[reqsize];
            hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_LOCATION_PATHS, IntPtr.Zero, bb, bb.Length, out reqsize);
            return bb.Split(reqsize);
        }

        static List<string> Split(this byte[] src, int src_length)
        {
            int src_len = src_length-2;
            List<string> list = new List<string>();
            int startindex = 0;
            while (true)
            {
                var findindex = Array.IndexOf(src, (byte)0, startindex);
                if (findindex <= 0)
                {
                    var stirng1 = Encoding.UTF8.GetString(src, startindex, src_len - startindex);
                    list.Add(stirng1);
                    break;
                }
                else if (findindex >= src_len)
                {
                    var stirng1 = Encoding.UTF8.GetString(src, startindex, src_len - startindex);
                    list.Add(stirng1);
                    break;
                }

                var stirng = Encoding.UTF8.GetString(src, startindex, findindex - startindex);
                list.Add(stirng);
                startindex = findindex + 1;
            }

            return list;
        }

        public static string GetLoationInformation(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb=null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_LOCATION_INFORMATION, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        public static void SetFriendName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, string data)
        {
            var buf1 = Encoding.Unicode.GetBytes(data);
            var buf2 = new byte[buf1.Length+2];
            Array.Copy(buf1, buf2, buf1.Length);
            var hr = SetupApi.SetupDiSetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_FRIENDLYNAME, buf2, (uint)buf2.Length);
        }


        public static string GetClass(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb=null)
        {
            if(strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_CLASS, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        public static Guid GetClassGuid(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            Guid guid = Guid.Empty;
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_CLASSGUID, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            if(Guid.TryParse(strb.ToString(), out guid) == false)
            {
                return guid;
            }

            return guid;
        }



        public static string GetClassDescription(this Guid src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            SetupApi.SetupDiGetClassDescription(ref src, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        public static string GetDescription(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICEDESC, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        public static string GetInstanceId(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb=null)
        {
            if(strb == null)
            {
                strb = new StringBuilder(2048);
            }
            SetupDiGetDeviceInstanceId(src.dev, ref src.devdata, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        //Ports: SerialPort
        public static Guid[] GetDevClass(this string src)
        {
            UInt32 RequiredSize = 0;
            Guid[] GuidArray = new Guid[1];
            // read Guids
            bool Status = SetupDiClassGuidsFromName(src, ref GuidArray[0], 1, out RequiredSize);
            if (true == Status)
            {
                if (1 < RequiredSize)
                {
                    GuidArray = new Guid[RequiredSize];
                    SetupDiClassGuidsFromName("class name here", ref GuidArray[0], RequiredSize, out RequiredSize);
                }
            }
            else
            {
                var ErrorCode = Marshal.GetLastWin32Error();
            }
            return GuidArray;
        }

        public static string GetMFG(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb=null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_MFG, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        public static string GetDriver(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DRIVER, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
            return strb.ToString();
        }

        static public string GetLowerFilters(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, StringBuilder strb = null)
        {
            if (strb == null)
            {
                strb = new StringBuilder(2048);
            }
            int reqsize;
            //bytep[] buf;
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_ENUMERATOR_NAME, IntPtr.Zero, null, 0, out reqsize);
            var err = Marshal.GetLastWin32Error();
            return strb.ToString();
        }

        public static IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> Devices(this Guid guid, bool showhiddendevice=false)
        {
            uint index = 0;

            uint flags = DIGCF_PRESENT | DIGCF_PROFILE;
            if(showhiddendevice==true)
            {
                flags = DIGCF_PROFILE;
            }
            if (guid == Guid.Empty)
            {
                flags  = flags | DIGCF_ALLCLASSES;
            }
            IntPtr hDevInfo = SetupApi.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            try
            {
                while (true)
                {
                    SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                    devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                    if (SetupApi.SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo) == false)
                    {
                        var err = Marshal.GetLastWin32Error();
                        //SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
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
                SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
            }

            //SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
        }

        public static int Remove(this IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> src)
        {
            int index = 0;
            foreach (var oo in src)
            {
                var devdata = oo.devdata;
                var hr = SetupApi.SetupDiRemoveDevice(oo.dev, ref devdata);
                if(hr==false)
                {
                    throw new Exception($"SetupDiRemoveDevice fail, errcoed{Marshal.GetLastWin32Error()}");
                }
                index = index + 1;
            }
            return index;
        }

        public static int Enable(this IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> src)
        {
            int index = 0;
            foreach (var oo in src)
            {
                oo.ChangeState(true);
                index = index + 1;
            }
            return index;
        }

        public static int Disable(this IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> src)
        {
            int index = 0;
            foreach (var oo in src)
            {
                oo.ChangeState(false);
                index = index + 1;
            }
            return index;
        }

        //public static void Enable(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        //{
        //    src.ChangeState(true);
        //}

        //public static void Disable(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        //{
        //    src.ChangeState(false);
        //}

        public static int Do<T>(this IEnumerable<T> src, Action<T> action)
        {
            foreach (var oo in src)
            {
                action(oo);
            }
            return 0;
        }

        public static int Do<T>(this IEnumerable<T> src, Action<T, int> action)
        {
            int index = 0;
            foreach (var oo in src)
            {
                action(oo, index);
                index++;

            }
            return 0;
        }

        static public void ChangeState(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src, bool isenable)
        {
            uint status;
            uint problem;
            var hr = CM_Get_DevNode_Status(out status, out problem, src.devdata.DevInst, 0);
            var enable = status & DN_STARTED;
            //if (isenable == true && enable != DN_STARTED)
            //{
            //    return;
            //}
            //var disable = status & DN_DISABLEABLE;
            //if (isenable == false && disable != DN_DISABLEABLE)
            //{
            //    return;
            //}

            SP_PROPCHANGE_PARAMS params1 = new SP_PROPCHANGE_PARAMS();
            params1.ClassInstallHeader.cbSize = Marshal.SizeOf(params1.ClassInstallHeader.GetType());
            params1.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
            params1.Scope = DICS_FLAG_GLOBAL;
            params1.StateChange = isenable == true ? DICS_ENABLE : DICS_DISABLE;

            // setup proper parameters            
            //if (!SetupDiSetClassInstallParams(hDevInfo, ptrToDevInfoData, ClassInstallParams, Marshal.SizeOf(params1.GetType())))
            if (!SetupDiSetClassInstallParams(src.dev, src.devdata, params1, Marshal.SizeOf(params1.GetType())))
            {
                int errorcode = Marshal.GetLastWin32Error();
                errorcode = 0;
            }

            // use parameters
            if (!SetupDiCallClassInstaller((uint)DIF_PROPERTYCHANGE, src.dev, ref src.devdata))
            {
                int errorcode = Marshal.GetLastWin32Error(); // error here  
                var msg = errorcode.GetLastErrorMessage();
                throw new Exception(msg);
            }
        }

        public static string GetLastErrorMessage(this int error)
        {
            IntPtr lpBuff = IntPtr.Zero;
            string sMsg = "";
            if (0 != FormatMessage(FORMAT_MESSAGE.ALLOCATE_BUFFER
                   | FORMAT_MESSAGE.FROM_SYSTEM
                   | FORMAT_MESSAGE.IGNORE_INSERTS,
                   IntPtr.Zero,
                   error,
                   0,
                   ref lpBuff,
                   0,
                   IntPtr.Zero))
            {
                sMsg = Marshal.PtrToStringUni(lpBuff);            //結果爲“重疊 I/O 操作在進行中”，完全正確
                Marshal.FreeHGlobal(lpBuff);
            }
            return sMsg;
        }
    }


    public static class SetupApi
    {
        public enum FORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageZId, ref IntPtr lpBuffer, int nSize, IntPtr Arguments);

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public UIntPtr Reserved;
        }
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, StringBuilder propertyBuffer, int propertyBufferSize, IntPtr requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, out int requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, out uint propertyBuffer, int propertyBufferSize, IntPtr requiredSize);

        [DllImport("Setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, byte[] PropertyBuffer, uint PropertyBufferSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetClassDescription(ref Guid ClassGuid, StringBuilder ClassDescription, int ClassDescriptionSize, IntPtr RequiredSize);
        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeRegistryHandle SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope, uint hwProfile, uint parameterRegistryValueKind, int samDesired);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
    IntPtr deviceInfoSet,
    ref SP_DEVINFO_DATA deviceInfoData,
    uint property,
    out UInt32 propertyRegDataType,
    IntPtr propertyBuffer, // the difference between this signature and the one above.
    uint propertyBufferSize,
    out UInt32 requiredSize
    );


        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr pDeviceInfoSet, ref SP_DEVINFO_DATA pDeviceInfoData, uint pProperty, string pPropertyBuffer, int pPropertyBufferSize);

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
        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern bool SetupDiRemoveDevice(IntPtr pDeviceInfoSet, ref SP_DEVINFO_DATA pDeviceInfoData);

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern int CM_Get_DevNode_Status(out UInt32 status, out UInt32 probNum, UInt32 devInst, int flags);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);
        public static int DIF_PROPERTYCHANGE = (0x00000012);

        [StructLayout(LayoutKind.Sequential)]
        public class SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
            public int StateChange;
            public uint Scope;
            public int HwProfile;
        };
        [StructLayout(LayoutKind.Sequential)]
        public class SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        };
        public static uint DN_ROOT_ENUMERATED = 0x00000001; // Was enumerated by ROOT
        public static uint DN_DRIVER_LOADED = 0x00000002; // Has Register_Device_Driver
        public static uint DN_ENUM_LOADED = 0x00000004; // Has Register_Enumerator
        public static uint DN_STARTED = 0x00000008; // Is currently configured
        public static uint DN_MANUAL = 0x00000010; // Manually installed
        public static uint DN_NEED_TO_ENUM = 0x00000020; // May need reenumeration
        public static uint DN_NOT_FIRST_TIME = 0x00000040; // Has received a config
        public static uint DN_HARDWARE_ENUM = 0x00000080; // Enum generates hardware ID
        public static uint DN_LIAR = 0x00000100; // Lied about can reconfig once
        public static uint DN_HAS_MARK = 0x00000200; // Not CM_Create_DevInst lately
        public static uint DN_HAS_PROBLEM = 0x00000400; // Need device installer
        public static uint DN_FILTERED = 0x00000800; // Is filtered
        public static uint DN_MOVED = 0x00001000; // Has been moved
        public static uint DN_DISABLEABLE = 0x00002000; // Can be disabled
        public static uint DN_REMOVABLE = 0x00004000; // Can be removed
        public static uint DN_PRIVATE_PROBLEM = 0x00008000; // Has a private problem
        public static uint DN_MF_PARENT = 0x00010000; // Multi function parent
        public static uint DN_MF_CHILD = 0x00020000; // Multi function child
        public static uint DN_WILL_BE_REMOVED = 0x00040000; // DevInst is being removed


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

        public const uint DICS_FLAG_GLOBAL = 0x00000001;  // make change in all hardware profiles
        public const uint DICS_FLAG_CONFIGSPECIFIC = 0x00000002;  // make change in specified profile only
        public const uint DICS_FLAG_CONFIGGENERAL = 0x00000004;  // 1 or more hardware profile-specific
        public const uint DIREG_DEV = 0x00000001;         // Open/Create/Delete device key
        public const uint DIREG_DRV = 0x00000002;        // Open/Create/Delete driver key
        public const uint DIREG_BOTH = 0x00000004;        // Delete both driver and Device key

        public const uint CM_DEVCAP_LOCKSUPPORTED = (0x00000001);
        public const uint CM_DEVCAP_EJECTSUPPORTED = (0x00000002);
        public const uint CM_DEVCAP_REMOVABLE = (0x00000004);
        public const uint CM_DEVCAP_DOCKDEVICE = (0x00000008);
        public const uint CM_DEVCAP_UNIQUEID = (0x00000010);
        public const uint CM_DEVCAP_SILENTINSTALL = (0x00000020);
        public const uint CM_DEVCAP_RAWDEVICEOK = (0x00000040);
        public const uint CM_DEVCAP_SURPRISEREMOVALOK = (0x00000080);
        public const uint CM_DEVCAP_HARDWAREDISABLED = (0x00000100);
        public const uint CM_DEVCAP_NONDYNAMIC = (0x00000200);
        public const uint CM_DEVCAP_SECUREDEVICE = (0x00000400);

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

    }

    public class DeviceInfo
    {
        internal SetupApi.SP_DEVINFO_DATA m_DevInfo;
        public DeviceInfo(IntPtr handle, SP_DEVINFO_DATA devinfo)
        {
            this.InstanceId = (handle, devinfo).GetInstanceId();
            this.Class = (handle,devinfo).GetClass();
            this.ClassGuid = (handle,devinfo).GetClassGuid();
            this.ClassDescription = this.ClassGuid.GetClassDescription();
            this.Description =(handle,devinfo).GetDescription();
            this.Manufacturer = (handle, devinfo).GetMFG();
            this.m_DevInfo = devinfo;
        }
        public string Class { internal set; get; }
        public string ClassDescription { internal set; get; }
        public Guid ClassGuid { internal set; get; }
        public List<string> HardwareIDs { internal set; get; } = new List<string>();
        public string FriendlyName { internal set; get; }
        public string Description { internal set; get; }
        public string InstanceId { internal set; get; }
        public string Location { internal set; get; }
        public List<string> LocationPaths { internal set; get; } = new List<string>();
        public string Manufacturer { private set; get; }
        //internal void ChangeState(bool isenable, IntPtr dev)
        //{
        //    uint status;

        //}
    }

}
