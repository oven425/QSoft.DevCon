using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtension;

namespace QSoft.DevCon
{
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/install/devpkey-device-physicaldevicelocation
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/stream/camera-device-orientation
    public static partial class DevMgrExtension
    {
        static DEVPROPKEY DEVPKEY_Devices_PhysicalDeviceLocation = new DEVPROPKEY() { fmtid = Guid.Parse("{540B947E-8B40-45BC-A8A2-6A0B894CBDA2}"), pid = 9 };
        public static CameraPanel Panel(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            uint propertytype = 0;

            int reqsz = 0;
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, IntPtr.Zero, 0, out reqsz, 0);


            using var mem = new IntPtrMem<byte>(reqsz);
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref DEVPKEY_Devices_PhysicalDeviceLocation, out propertytype, mem.Pointer, reqsz, out reqsz, 0);
            byte[] lbuffer = new byte[reqsz];
            Marshal.Copy(mem.Pointer, lbuffer, 0, reqsz);


            BitArray myBA3 = new BitArray(lbuffer);

            Convert(myBA3.Get(69), myBA3.Get(68), myBA3.Get(67));

            return (CameraPanel)Convert(myBA3.Get(69), myBA3.Get(68), myBA3.Get(67));

        }

        static int Convert(params bool[] src)
        {
            int dd = 0;
            for (int i = 0; i < src.Length; i++)
            {

                if (i > 0)
                {
                    dd = dd << 1;
                }
                int o = src[i] ? 1 : 0;
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
