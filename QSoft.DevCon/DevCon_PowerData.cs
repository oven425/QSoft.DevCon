using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static CM_Power_Data? PowerData(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
#if NET8_0_OR_GREATER
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICE_POWER_DATA, out var property_type, [], 0, out var reqsize);
            if (reqsize > 0)
            {
                Span<byte> mem = stackalloc byte[(int)reqsize];
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICE_POWER_DATA, out property_type, mem, reqsize, out reqsize);
                
                
                var pd = MemoryMarshal.AsRef<CM_Power_Data>(mem);
                return pd;
            }
#else
            SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICE_POWER_DATA, out var property_type, IntPtr.Zero, 0, out var reqsize);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<CM_Power_Data>((int)reqsize);
                SetupDiGetDeviceRegistryProperty(src.dev, ref src.devdata, SPDRP_DEVICE_POWER_DATA, out property_type, mem.Pointer, reqsize, out reqsize);
                var pd = Marshal.PtrToStructure<CM_Power_Data>(mem.Pointer);
                var ddd = ((PDCAP)pd.PD_Capabilities);
                return pd;
            }
#endif
            return null;
        }

        [Flags]
        public enum PDCAP
        {
            PDCAP_D0_SUPPORTED = 0x0001,
            PDCAP_D1_SUPPORTED = 0x0002,
            PDCAP_D2_SUPPORTED = 0x0004,
            PDCAP_D3_SUPPORTED = 0x0008,
            PDCAP_WAKE_FROM_D0_SUPPORTED = 0x0010,
            PDCAP_WAKE_FROM_D1_SUPPORTED = 0x0020,
            PDCAP_WAKE_FROM_D2_SUPPORTED = 0x0030,
            PDCAP_WAKE_FROM_D3_SUPPORTED = 0x0080,
            PDCAP_WARM_EJECT_SUPPORTED = 0x1000
        }
        public enum DEVICE_POWER_STATE
        {
            PowerDeviceUnspecified = 0,
            PowerDeviceD0,
            PowerDeviceD1,
            PowerDeviceD2,
            PowerDeviceD3,
            PowerDeviceMaximum
        };
#if NET8_0_OR_GREATER
        [InlineArray(7)] // 指定陣列大小為 7
        public struct DevicePowerStateInlineArray
        {
            private DEVICE_POWER_STATE _element0; // 內聯陣列的內部實現細節
        }
#endif
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CM_Power_Data
        {
            public uint PD_Size;
            DEVICE_POWER_STATE PD_MostRecentPowerState;
            public uint PD_Capabilities;
            public uint PD_D1Latency;
            public uint PD_D2Latency;
            public uint PD_D3Latency;
#if NET8_0_OR_GREATER
            public DevicePowerStateInlineArray PD_PowerStateMapping;
#else
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public DEVICE_POWER_STATE[] PD_PowerStateMapping;
#endif

            public SYSTEM_POWER_STATE PD_DeepestSystemWake;
            public override string ToString()
            {
                
                var sb = new StringBuilder();
                sb.AppendLine($"目前的電源狀態:");
                sb.AppendLine($"{PD_MostRecentPowerState}");
                sb.AppendLine();
                sb.AppendLine($"電源能力:");
                sb.AppendLine($"{PD_Capabilities:x08}");
                var dps = (PDCAP)PD_Capabilities;
                foreach (var oo in Enum.GetValues(typeof(PDCAP)).Cast<PDCAP>().Where(x => dps.HasFlag(x)))
                {
                    sb.AppendLine($"{oo}");
                }
                sb.AppendLine();

                int index = 0;
                foreach (var oo in PD_PowerStateMapping)
                {
                    if(index >0)
                    {
                        sb.AppendLine($"S{index-1}-> {oo}");
                    }
                    index++;
                }
                return sb.ToString();
            }
        };

        public enum SYSTEM_POWER_STATE
        {
            PowerSystemUnspecified = 0,
            PowerSystemWorking = 1,
            PowerSystemSleeping1 = 2,
            PowerSystemSleeping2 = 3,
            PowerSystemSleeping3 = 4,
            PowerSystemHibernate = 5,
            PowerSystemShutdown = 6,
            PowerSystemMaximum = 7
        };
    }
}
