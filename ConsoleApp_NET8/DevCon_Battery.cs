using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    public static partial class DevCon_Battery
    {
        public static void GetBatteryInfo(this SafeFileHandle src)
        {
            BATTERY_QUERY_INFORMATION info = new();
            info.InformationLevel = BatteryInformation;
            Span<byte> span_in = stackalloc byte[sizeof(uint)];
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref info.BatteryTag, 1));

            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);


            info.InformationLevel = BatteryManufactureName;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, [], 0, out reqsz, IntPtr.Zero);
            span_out = stackalloc byte[(int)reqsz*4];
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();

            BATTERY_WAIT_STATUS batter_wait_status = new()
            {
                BatteryTag = info.BatteryTag
            };
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_wait_status, 1));
            BATTERY_STATUS batter_status = new();
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_status, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_STATUS, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);

            
        }

        struct BATTERY_QUERY_INFORMATION
        {
            public uint BatteryTag;
            public int InformationLevel;
            public int AtRate;
        }

        public struct BATTERY_WAIT_STATUS 
        {
            public uint BatteryTag;
            public uint Timeout;
            public uint PowerState;
            public uint LowCapacity;
            public uint HighCapacity;
        }

        public enum PowerState : uint
        {
            BATTERY_POWER_ON_LINE = 0x00000001,
            BATTERY_DISCHARGING = 0x00000002,
            BATTERY_CHARGING = 0x00000004,
            BATTERY_CRITICAL = 0x00000008
        }

        public struct BATTERY_STATUS
        {
            public PowerState PowerState;
            public uint Capacity;
            public uint Voltage;
            public int Rate;
        }

        const uint IOCTL_BATTERY_QUERY_TAG = 0x00294040;
        const uint IOCTL_BATTERY_QUERY_STATUS = 0x0029404c;
        const uint IOCTL_BATTERY_QUERY_INFORMATION = 0x00294044;
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
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, ReadOnlySpan<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

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
