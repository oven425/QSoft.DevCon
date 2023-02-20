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
        public static string GetFriendName(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            StringBuilder friendlyname = new StringBuilder(2048);
            var hr = SetupApi.SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_FRIENDLYNAME, IntPtr.Zero, friendlyname, friendlyname.Capacity, IntPtr.Zero);
            return friendlyname.ToString();

        }

        public static IEnumerable<(IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata)> Devices(this Guid guid)
        {
            UInt32 RequiredSize = 0;
            Guid[] GuidArray = new Guid[1];
            // read Guids
            bool Status = SetupDiClassGuidsFromName("Ports", ref GuidArray[0], 1, out RequiredSize);
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
            uint index = 0;
            if (guid == Guid.Empty)
            {

            }
            IntPtr hDevInfo = SetupApi.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_PROFILE);
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
                oo.devdata.ChangeState(true);
            }
        }

        static public void ChangeState(this SP_DEVINFO_DATA devinfo, bool isenable)
        {
            //uint status;
            //uint problem;
            //var hr = CM_Get_DevNode_Status(out status, out problem, devinfo.DevInst, 0);
            //var enable = status & DN_STARTED;
            //if (isenable == true && enable != DN_STARTED)
            //{
            //    return;
            //}
            //var disable = status & DN_DISABLEABLE;
            //if (isenable == false && disable != DN_DISABLEABLE)
            //{
            //    return;
            //}

            //SP_PROPCHANGE_PARAMS params1 = new SP_PROPCHANGE_PARAMS();
            //params1.ClassInstallHeader.cbSize = Marshal.SizeOf(params1.ClassInstallHeader.GetType());
            //params1.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
            //params1.Scope = DICS_FLAG_GLOBAL;
            //params1.StateChange = isenable == true ? DICS_ENABLE : DICS_DISABLE;

            //// setup proper parameters            
            ////if (!SetupDiSetClassInstallParams(hDevInfo, ptrToDevInfoData, ClassInstallParams, Marshal.SizeOf(params1.GetType())))
            //if (!SetupDiSetClassInstallParams(m_hDev.Value, devinfo, params1, Marshal.SizeOf(params1.GetType())))
            //{
            //    int errorcode = Marshal.GetLastWin32Error();
            //    errorcode = 0;
            //}

            //// use parameters
            //if (!SetupDiCallClassInstaller((uint)DIF_PROPERTYCHANGE, m_hDev.Value, ref devinfo))
            //{
            //    int errorcode = Marshal.GetLastWin32Error(); // error here  
            //    var msg = GetLastErrorMessage(errorcode);
            //    throw new Exception(msg);
            //}
        }

    }


    public static class SetupApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public UIntPtr Reserved;
        }
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, StringBuilder propertyBuffer, int propertyBufferSize, IntPtr requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

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

    }

}
