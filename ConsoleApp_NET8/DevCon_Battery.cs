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
            BATTERY_QUERY_INFORMATION info = new()
            {
                InformationLevel = BatteryInformation
            };
            Span<byte> span_in = stackalloc byte[sizeof(uint)];
            Span<byte> span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info.BatteryTag, 1));
            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);


            info.InformationLevel = BatteryInformation;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            BATTERY_INFORMATION batterinfo = new();
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batterinfo, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var sss = System.Text.Encoding.ASCII.GetString(batterinfo.Chemistry);

            info.InformationLevel = BatteryManufactureDate;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            BATTERY_MANUFACTURE_DATE battery_manufacture_date = new();
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_manufacture_date, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            

            info.InformationLevel = BatteryGranularityInformation;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            span_out = stackalloc byte[40];
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var scale = MemoryMarshal.Cast<byte, BATTERY_REPORTING_SCALE>(span_out);
            var err = Marshal.GetLastWin32Error();

            info.InformationLevel = BatteryTemperature;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            uint battery_temperature = 0;
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_temperature, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            err = Marshal.GetLastWin32Error();

            info.InformationLevel = BatteryEstimatedTime;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            uint battery_est = 0;
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_est, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var est = MemoryMarshal.Read<uint>(span_out);
            err = Marshal.GetLastWin32Error();

            info.InformationLevel = BatterySerialNumber;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            span_out = stackalloc byte[256];
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var SerialNumber = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            span_out.Clear();
            info.InformationLevel = BatteryDeviceName;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var DeviceName = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            span_out.Clear();
            info.InformationLevel = BatteryManufactureName;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var ManufactureName = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            span_out.Clear();
            info.InformationLevel = BatteryUniqueID;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var UniqueID = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();




            BATTERY_WAIT_STATUS batter_wait_status = new()
            {
                BatteryTag = info.BatteryTag
            };
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_wait_status, 1));
            BATTERY_STATUS batter_status = new();
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_status, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_STATUS, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);

            
        }

        struct BATTERY_REPORTING_SCALE
        {
            uint Granularity;
            uint Capacity;
        };
        

        struct BATTERY_MANUFACTURE_DATE
        {
            byte Day;
            byte Month;
            ushort Year;
        };

        [InlineArray(3)]
        public struct BufferReserved3 { private byte _element0; }

        [InlineArray(4)]
        public struct BufferChemistry { private byte _element0; }

        [Flags]
        public enum BatteryCapabilities:uint
        {
            BATTERY_SET_CHARGE_SUPPORTED = 0x00000001,
            BATTERY_SET_DISCHARGE_SUPPORTED = 0x00000002,
            BATTERY_IS_SHORT_TERM = 0x20000000,
            BATTERY_CAPACITY_RELATIVE = 0x40000000,
            BATTERY_SYSTEM_BATTERY = 0x80000000,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BATTERY_INFORMATION
        {
            public BatteryCapabilities Capabilities;
            public byte Technology;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public BufferReserved3 Reserved;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public BufferChemistry Chemistry;

            public uint DesignedCapacity;
            public uint FullChargedCapacity;
            public uint DefaultAlert1;
            public uint DefaultAlert2;
            public uint CriticalBias;
            public uint CycleCount;
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
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
