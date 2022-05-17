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
        public IEnumerable<DeviceInfo> AllDevice()
        {
            List<DeviceInfo> dds = new List<DeviceInfo>();


            return dds;
        }
        Guid GUID_DEVINTERFACE_DISK = new Guid(0x53f56307, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x00, 0xa0, 0xc9, 0x1e, 0xfb, 0x8b);
        void Test()
        {
            Guid DiskGUID = GUID_DEVINTERFACE_DISK;
            IntPtr h = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        }
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        const int DIGCF_DEFAULT = 0x1;
        const int DIGCF_PRESENT = 0x2;
        const int DIGCF_ALLCLASSES = 0x4;
        const int DIGCF_PROFILE = 0x8;
        const int DIGCF_DEVICEINTERFACE = 0x10;

    }


    public class DeviceInfo
    {
        public string HardwareID { internal set; get; }
        public void Enable()
        {

        }
    }

    public static class IDeviceEnumable
    {
        public static int Enable(this IEnumerable<DeviceInfo> src)
        {
            foreach (var oo in src)
            {
                oo.Enable();
            }
            return 0;
        }
    }
}
