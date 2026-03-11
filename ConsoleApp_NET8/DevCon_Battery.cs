using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    public static partial class DevCon_Battery
    {
        public static void GetBatteryInfo(this SafeFileHandle src)
        {
            uint queryTagWait = 0; // 0 表示立即回傳，不等待
            uint batteryTag = 0;
            uint bytesReturned = 0;

            bool success = DeviceIoControl(
                src,
                IOCTL_BATTERY_QUERY_TAG,
                ref queryTagWait,
                sizeof(uint),
                ref batteryTag,
                sizeof(uint),
                out bytesReturned,
                IntPtr.Zero
            );

            BATTERY_QUERY_INFORMATION info = new BATTERY_QUERY_INFORMATION();
            info.InformationLevel = 0;
            
            uint dwWait = 0;
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref dwWait, 1));
            //var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref info.BatteryTag, 1));
            Span<byte> span_out = stackalloc byte[4];
            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
        }

        struct BATTERY_QUERY_INFORMATION
        {
            public uint BatteryTag;
            public int InformationLevel;
            public int AtRate;
        }
        const uint IOCTL_BATTERY_QUERY_TAG = 0x290000;
        const int BatteryInformation = 0;
        const int BatteryGranularityInformation = 1;
        const int BatteryTemperature = 2;
        const int BatteryEstimatedTime = 3;
        const int BatteryDeviceName = 4;
        const int BatteryManufactureDate = 5;
        const int BatteryManufactureName = 6;
        const int BatteryUniqueID = 7;
        const int BatterySerialNumber = 8;


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
    SafeFileHandle hDevice,
    uint dwIoControlCode,
    ref uint lpInBuffer,
    uint nInBufferSize,
    ref uint lpOutBuffer,
    uint nOutBufferSize,
    out uint lpBytesReturned,
    IntPtr lpOverlapped);
    }
}
