using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon.Battery
{
    public static partial class DevCon_Battery
    {
        public static IEnumerable<SafeFileHandle> Release(this IEnumerable<SafeFileHandle> src)
        {
            foreach (var item in src)
            {
                item.Dispose();
                yield return item;
            }
        }

        public static IEnumerable<T> Release<T>(this IEnumerable<T> src, Func<T, SafeFileHandle> ff)
        {
            foreach (var item in src)
            {
                ff(item).Dispose();
                yield return item;
            }
        }

        public static (SafeFileHandle handle, uint tag) BatteryTag(this SafeFileHandle src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                InformationLevel = BatteryInformationLevel.Information
            };
            Span<byte> span_in = stackalloc byte[sizeof(uint)];
            Span<byte> span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info.BatteryTag, 1));
            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            return (src, info.BatteryTag);
        }

        public static BATTERY_STATUS BatteryStatus(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_WAIT_STATUS batter_wait_status = new()
            {
                BatteryTag = src.batteryTag
            };
            Span<byte> span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_wait_status, 1));
            BATTERY_STATUS batter_status = new();
            Span<byte> span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_status, 1));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_STATUS, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            return batter_status;
        }

        public static BATTERY_INFORMATION BatteryInfo(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                InformationLevel = BatteryInformationLevel.Information,
                BatteryTag = src.batteryTag
            };

            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            BATTERY_INFORMATION batterinfo = new();
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batterinfo, 1));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            var sss = System.Text.Encoding.ASCII.GetString(batterinfo.Chemistry);
            return batterinfo;
        }

        public static string BatterySerialNumber(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.SerialNumber
            };
            return src.handle.GetBatteryString(info);
        }

        public static string BatteryDeviceName(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.DeviceName
            };
            return src.handle.GetBatteryString(info);
        }

        public static string BatteryManufactureName(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.ManufactureName
            };
            return src.handle.GetBatteryString(info);
        }

        public static string BatteryUniqueID(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.UniqueID
            };
            return src.handle.GetBatteryString(info);
        }

        static string GetBatteryString(this SafeFileHandle handle, BATTERY_QUERY_INFORMATION query)
        {
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref query, 1));
            Span<byte> span_out = stackalloc byte[256];
            var hr = DeviceIoControl(handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            var str = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();
            return str;
        }




        public static void GetBatteryInfo(this SafeFileHandle src)
        {
            bool hr = false;
            uint reqsz = 0;
            BATTERY_QUERY_INFORMATION info = new()
            {
                InformationLevel = BatteryInformationLevel.Information
            };
            Span<byte> span_in = stackalloc byte[sizeof(uint)];
            Span<byte> span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info.BatteryTag, 1));
            //var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            
            var battery = src.BatteryTag();
            var batterystatus = battery.BatteryStatus();
            var batterinfo = battery.BatteryInfo();
            var batterysn = battery.BatterySerialNumber();
            var batterydevicename = battery.BatteryDeviceName();
            var batterymn = battery.BatteryManufactureName();
            var batteryid = battery.BatteryUniqueID();

            //info.InformationLevel = BatteryInformation;
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            //BATTERY_INFORMATION batterinfo = new();
            //span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batterinfo, 1));
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            //var sss = System.Text.Encoding.ASCII.GetString(batterinfo.Chemistry);

            info.InformationLevel = BatteryInformationLevel.ManufactureDate;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            BATTERY_MANUFACTURE_DATE battery_manufacture_date = new();
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_manufacture_date, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            

            info.InformationLevel = BatteryInformationLevel.GranularityInformation;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            span_out = stackalloc byte[40];
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var scale = MemoryMarshal.Cast<byte, BATTERY_REPORTING_SCALE>(span_out);
            var err = Marshal.GetLastWin32Error();

            info.InformationLevel = BatteryInformationLevel.Temperature;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            uint battery_temperature = 0;
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_temperature, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            err = Marshal.GetLastWin32Error();

            info.InformationLevel = BatteryInformationLevel.EstimatedTime;
            span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            uint battery_est = 0;
            span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_est, 1));
            hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var est = MemoryMarshal.Read<uint>(span_out);
            err = Marshal.GetLastWin32Error();

            //info.InformationLevel = BatterySerialNumber;
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            //span_out = stackalloc byte[256];
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            //var SerialNumber = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            //span_out.Clear();
            //info.InformationLevel = BatteryInformationLevel.DeviceName;
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            //var DeviceName = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            //span_out.Clear();
            //info.InformationLevel = BatteryInformationLevel.ManufactureName;
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            //var ManufactureName = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();

            //span_out.Clear();
            //info.InformationLevel = BatteryInformationLevel.UniqueID;
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            //var UniqueID = MemoryMarshal.Cast<byte, char>(span_out).TrimEnd('\0').ToString();




            //BATTERY_WAIT_STATUS batter_wait_status = new()
            //{
            //    BatteryTag = info.BatteryTag
            //};
            //span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_wait_status, 1));
            //BATTERY_STATUS batter_status = new();
            //span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_status, 1));
            //hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_STATUS, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);

            
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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BATTERY_QUERY_INFORMATION
        {
            public uint BatteryTag;
            public BatteryInformationLevel InformationLevel;
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

        public readonly struct BATTERY_STATUS
        {
            public readonly PowerState PowerState;
            public readonly uint Capacity;
            public readonly uint Voltage;
            public readonly int Rate;
        }

        const uint IOCTL_BATTERY_QUERY_TAG = 0x00294040;
        const uint IOCTL_BATTERY_QUERY_STATUS = 0x0029404c;
        const uint IOCTL_BATTERY_QUERY_INFORMATION = 0x00294044;
        public enum BatteryInformationLevel
        {
            Information = 0,
            GranularityInformation = 1,
            Temperature = 2,
            EstimatedTime = 3,
            DeviceName = 4,
            ManufactureDate = 5,
            ManufactureName = 6,
            UniqueID = 7,
            SerialNumber = 8
        }
        //const int BatteryInformation = 0;
        //const int BatteryGranularityInformation = 1;
        //const int BatteryTemperature = 2;
        //const int BatteryEstimatedTime = 3;
        //const int BatteryDeviceName = 4;
        //const int BatteryManufactureDate = 5;
        //const int BatteryManufactureName = 6;
        //const int BatteryUniqueID = 7;
        //const int BatterySerialNumber = 8;


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
