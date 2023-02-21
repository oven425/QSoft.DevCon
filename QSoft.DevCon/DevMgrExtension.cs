using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.SetupApi;

namespace QSoft.DevCon
{
    public static class DevMgrExtension
    {
        public static uint SPDRP_FRIENDLYNAME = (0x0000000C);
        
        public static uint SPDRP_DEVICEDESC = 0x00000000; // DeviceDesc (R/W)
        public static uint SPDRP_HARDWAREID = (0x00000001);  // HardwareID (R/W)
        public static uint SPDRP_CLASS = (0x00000007); // Class (R--tied to ClassGUID)
        public static uint SPDRP_CLASSGUID = (0x00000008); // ClassGUID (R/W)
        public static string GetFriendName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            StringBuilder strb = new StringBuilder(2048);
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_FRIENDLYNAME, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
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
            guid = Guid.Parse(strb.ToString());

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

        public static string GetDescription(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            StringBuilder strb = new StringBuilder(2048);
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

        public static IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> Devices(this Guid guid)
        {
            uint index = 0;
            uint flags = DIGCF_PRESENT | DIGCF_PROFILE;
            if (guid == Guid.Empty)
            {
                flags  = flags | DIGCF_ALLCLASSES;
            }
            IntPtr hDevInfo = SetupApi.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            while (true)
            {
                SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                if (SetupApi.SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo) == false)
                {
                    var err = Marshal.GetLastWin32Error();
                    SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
                    yield break;
                }
                else
                {
                    yield return (hDevInfo, devinfo);
                }
                index++;
            }
            //SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
        }

        public static void Enable(this IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> src)
        {
            foreach (var oo in src)
            {
                oo.ChangeState(true);
            }
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

        public static void Do<T>(this IEnumerable<T> src, Action<T> action)
        {
            foreach (var oo in src)
            {
                action(oo);
            }
        }

        //public static void Do(this IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> src, Action<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> action)
        //{
        //    foreach (var oo in src)
        //    {
        //        action(oo);
        //    }
        //}

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
        [DllImport("Setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, byte[] PropertyBuffer, uint PropertyBufferSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetClassDescription(ref Guid ClassGuid, StringBuilder ClassDescription, int ClassDescriptionSize, IntPtr RequiredSize);

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
        public static extern int CM_Get_DevNode_Status(out UInt32 status, out UInt32 probNum, UInt32 devInst, int flags);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);
        public static int DIF_PROPERTYCHANGE = (0x00000012);
        public static int DICS_FLAG_GLOBAL = (0x00000001);

        [StructLayout(LayoutKind.Sequential)]
        public class SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
            public int StateChange;
            public int Scope;
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


    }

}
