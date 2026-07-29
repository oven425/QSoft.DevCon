using Microsoft.Win32.SafeHandles;
using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/powermeter/energy-meter-interface
    static unsafe public partial class DevCon_IEM
    {
        public static void Version(this SafeFileHandle src)
        {
            EMI_VERSION v1 = new EMI_VERSION();
            var span_out_v1 = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref v1, 1));
            var hr = DeviceIoControl(src, IOCTL_EMI_GET_VERSION, [], 0, span_out_v1, (uint)span_out_v1.Length, out var reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            EMI_METADATA_V1 v2 = new();
            var span_out_v2 = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref v2, 1));
            hr = DeviceIoControl(src, IOCTL_EMI_GET_VERSION, [], 0, span_out_v2, (uint)span_out_v2.Length, out reqsz, IntPtr.Zero);
            err = Marshal.GetLastWin32Error();

        }

        struct EMI_VERSION
        {
            ushort EmiVersion;
        };

        enum EMI_MEASUREMENT_UNIT
        {
            EmiMeasurementUnitPicowattHours
        };
        //public fixed byte HardwareOEM[EmiConstants.EMI_NAME_MAX];
        //public fixed byte HardwareModel[EmiConstants.EMI_NAME_MAX];
        struct EMI_METADATA_V1
        {
            EMI_MEASUREMENT_UNIT MeasurementUnit;
            fixed char HardwareOEM[16];
            fixed char HardwareModel[16];
            ushort HardwareRevision;
            ushort MeteredHardwareNameSize;
            //char MeteredHardwareName[ANYSIZE_ARRAY];
        };
        const uint IOCTL_EMI_GET_VERSION = 0x00224000;
        public static Guid Guid_IEM = new Guid("{45BD8344-7ED6-49cf-A440-C276C933B053}");
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
