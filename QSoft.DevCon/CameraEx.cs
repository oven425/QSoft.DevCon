using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.SetupApi;

namespace QSoft.DevCon.Camera
{
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-physicaldevicelocation
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/stream/camera-device-orientation
    public static class CameraEx
    {
        public static DEVPROPKEY DEVPKEY_Devices_PhysicalDeviceLocation = new DEVPROPKEY() { fmtid = Guid.Parse("{540B947E-8B40-45BC-A8A2-6A0B894CBDA2}"), pid = 9 };
        public static int Panel(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
        {
            uint propertytype = 0;
            
            int reqsz = 0;
            int property = 0;
            var hr = SetupApi.SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, null, 0, out reqsz, 0);
            if (hr == false)
            {
                var err = Marshal.GetLastWin32Error();
            }
            byte[] buffer = null;
            hr = SetupApi.SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, out buffer, reqsz, out reqsz, 0);

            var oo = property & SetupApi.DN_NEEDS_LOCKING;
            return 0;
        }
    }
}
