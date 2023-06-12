using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.SetupApi;

namespace QSoft.DevCon
{
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-physicaldevicelocation
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/stream/camera-device-orientation
    public static partial class DevMgrExtension
    {
        public static DEVPROPKEY DEVPKEY_Devices_PhysicalDeviceLocation = new DEVPROPKEY() { fmtid = Guid.Parse("{540B947E-8B40-45BC-A8A2-6A0B894CBDA2}"), pid = 9 };
        public static int Panel(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            uint propertytype = 0;
            
            int reqsz = 0;
            var hr = SetupApi.SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, null, 0, out reqsz, 0);
            if (hr == false)
            {
                var err = Marshal.GetLastWin32Error();
            }
            var buffer = Marshal.AllocHGlobal((int)reqsz);
            hr = SetupApi.SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, buffer, reqsz, out reqsz, 0);
            byte[] lbuffer = new byte[reqsz];
            Marshal.Copy(buffer, lbuffer, 0, (int)reqsz);
            int errcode = Marshal.GetLastWin32Error();
            Marshal.FreeHGlobal(buffer);

            return 0;
        }
    }

    public enum CameraPanel
    {
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back,
        Unknow
    }
}
