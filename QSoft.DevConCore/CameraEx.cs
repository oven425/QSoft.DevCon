using System;
using System.Collections;
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
        public static DEVPROPKEY DEVPKEY_Devices_PhysicalDeviceLocation = new() { fmtid = Guid.Parse("{540B947E-8B40-45BC-A8A2-6A0B894CBDA2}"), pid = 9 };
        public static CameraPanel Panel(this (IntPtr dev, SetupApi.SP_DEVINFO_DATA devdata) src)
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


            var myBA3 = new BitArray(lbuffer);

            Convert(myBA3.Get(69), myBA3.Get(68), myBA3.Get(67));
            
            int errcode = Marshal.GetLastWin32Error();
            Marshal.FreeHGlobal(buffer);

            return (CameraPanel)Convert(myBA3.Get(69), myBA3.Get(68), myBA3.Get(67));
        }

        static int Convert(params bool[] src)
        {
            int dd = 0;
            for (int i = 0; i < src.Length; i++)
            {

                if (i > 0)
                {
                    dd <<= 1;
                }
                int o = src[i] == true ? 1 : 0;
                dd = dd | o;


            }
            return dd;
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
