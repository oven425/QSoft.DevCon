using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QSoft.DevCon
{
    public static partial class DevConExtension
    {
        public static (SafeFileHandle handle, uint tag) BatteryTag(this SafeFileHandle src)
        {
#if NET8_0_OR_GREATER
            Span<byte> span_in = stackalloc byte[sizeof(uint)];
            Span<byte> span_out = stackalloc byte[sizeof(uint)];
            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            return (src, MemoryMarshal.Read<uint>(span_out));

#else
            using var mem_out = new IntPtrMem<uint>(1);
            using var mem_in = new IntPtrMem<uint>(1);
            var hr = DeviceIoControl(src, IOCTL_BATTERY_QUERY_TAG, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            return (src, (uint)Marshal.ReadInt32(mem_out.Pointer));
#endif
        }

        public static BATTERY_STATUS BatteryStatus(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_WAIT_STATUS batter_wait_status = new()
            {
                BatteryTag = src.batteryTag
            };
            BATTERY_STATUS battery_status = new();
#if NET8_0_OR_GREATER
            Span<byte> span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batter_wait_status, 1));
            Span<byte> span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_status, 1));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_STATUS, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
#else
            using var mem_in = new IntPtrMem<BATTERY_WAIT_STATUS>(1);
            using var mem_out = new IntPtrMem<BATTERY_STATUS>(1);
            Marshal.StructureToPtr(batter_wait_status, mem_in.Pointer, false);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_STATUS, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            battery_status = Marshal.PtrToStructure<BATTERY_STATUS>(mem_out.Pointer);
#endif

            return battery_status;
        }

        public static BATTERY_INFORMATION BatteryInfo(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                InformationLevel = BatteryInformationLevel.Information,
                BatteryTag = src.batteryTag
            };
            BATTERY_INFORMATION batterinfo = new();
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref batterinfo, 1));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
#else
            using var mem_in = new IntPtrMem<BATTERY_QUERY_INFORMATION>(1);
            using var mem_out = new IntPtrMem<BATTERY_INFORMATION>(1);
            Marshal.StructureToPtr(info, mem_in.Pointer, false);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            batterinfo = Marshal.PtrToStructure<BATTERY_INFORMATION>(mem_out.Pointer);
#endif

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
            string str = "";
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref query, 1));
            Span<byte> span_out = stackalloc byte[256];
            var hr = DeviceIoControl(handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            str = MemoryMarshal.Cast<byte, char>(span_out[..(int)reqsz]).TrimEnd('\0').ToString();
#else
            using var mem_out = new IntPtrMem<byte>(256);
            using var mem_in = new IntPtrMem<BATTERY_QUERY_INFORMATION>(1);
            Marshal.StructureToPtr(query, mem_in.Pointer, false);
            DeviceIoControl(handle, IOCTL_BATTERY_QUERY_INFORMATION, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            if (reqsz > 0)
                str = Marshal.PtrToStringUni(mem_out.Pointer) ?? "";
#endif

            return str;
        }

        public static uint BatteryEstimatedTime(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.EstimatedTime
            };
            uint battery_est = 0;
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            var span_out = MemoryMarshal.AsBytes(new Span<uint>(ref battery_est));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
#else
            using var mem_in = new IntPtrMem<BATTERY_QUERY_INFORMATION>(1);
            using var mem_out = new IntPtrMem<uint>(1);
            Marshal.StructureToPtr(info, mem_in.Pointer, false);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            battery_est = (uint)Marshal.ReadInt32(mem_out.Pointer);

#endif


            return battery_est;
        }

        public static uint BatteryTemperature(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.Temperature
            };
            uint battery_temperature = 0;
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            var span_out = MemoryMarshal.AsBytes(new Span<uint>(ref battery_temperature));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
#else
            using var mem_in = new IntPtrMem<BATTERY_QUERY_INFORMATION>(1);
            using var mem_out = new IntPtrMem<uint>(1);
            Marshal.StructureToPtr(info, mem_in.Pointer, false);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            battery_temperature = (uint)Marshal.ReadInt32(mem_out.Pointer);
#endif


            return battery_temperature;
        }

        public static BATTERY_REPORTING_SCALE[] BatteryGranularityInformation(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.GranularityInformation
            };
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            Span<byte> span_out = MemoryMarshal.AsBytes(stackalloc BATTERY_REPORTING_SCALE[5]);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            var scale = MemoryMarshal.Cast<byte, BATTERY_REPORTING_SCALE>(span_out[..(int)reqsz]);
            return scale.ToArray();
#else
            using var mem_in = new IntPtrMem<BATTERY_QUERY_INFORMATION>(1);
            using var mem_out = new IntPtrMem<BATTERY_REPORTING_SCALE>(5);
            Marshal.StructureToPtr(info, mem_in.Pointer, false);
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, mem_in.Pointer, (uint)mem_in.Size, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            var ss = Marshal.SizeOf<BATTERY_REPORTING_SCALE>();
            var count = reqsz / ss;
            var scales = new BATTERY_REPORTING_SCALE[count];
            for (int i = 0; i < count; i++)
            {
                var scale = Marshal.PtrToStructure<BATTERY_REPORTING_SCALE>(mem_out.Pointer + i * ss);
                scales[i] = scale;
            }
            return scales;
#endif
        }

        public static BATTERY_MANUFACTURE_DATE BatteryManufactureDate(this (SafeFileHandle handle, uint batteryTag) src)
        {
            BATTERY_QUERY_INFORMATION info = new()
            {
                BatteryTag = src.batteryTag,
                InformationLevel = BatteryInformationLevel.ManufactureDate
            };
            
            BATTERY_MANUFACTURE_DATE battery_manufacture_date = new();
#if NET8_0_OR_GREATER
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref battery_manufacture_date, 1));
            var hr = DeviceIoControl(src.handle, IOCTL_BATTERY_QUERY_INFORMATION, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
#else
            
#endif

            return battery_manufacture_date;
        }

        public static void GetBatteryInfo(this SafeFileHandle src)
        {
            var battery = src.BatteryTag();
            var batterystatus = battery.BatteryStatus();
            var batterinfo = battery.BatteryInfo();
            var batterysn = battery.BatterySerialNumber();
            var batterydevicename = battery.BatteryDeviceName();
            var batterymn = battery.BatteryManufactureName();
            var batteryid = battery.BatteryUniqueID();
            var batteryest = battery.BatteryEstimatedTime();
            var batterytemp = battery.BatteryTemperature();
            var batterygray = battery.BatteryGranularityInformation();
            var battermd = battery.BatteryManufactureDate();
        }

        public static void Throw<T>(this T? src) where T : struct
        {
            if (!src.HasValue)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static T Throw<T>(this T src) where T : struct
        {
            var err = Marshal.GetLastWin32Error();
            if (err != 0)
                throw new Win32Exception(err);
            return src;
        }


        public struct BATTERY_REPORTING_SCALE
        {
            uint Granularity;
            uint Capacity;
        };


        public struct BATTERY_MANUFACTURE_DATE
        {
            byte Day;
            byte Month;
            ushort Year;
        };

#if NET8_0_OR_GREATER
        [InlineArray(3)]
        public struct BufferReserved3 { private byte _element0; }

        [InlineArray(4)]
        public struct BufferChemistry { private byte _element0; }
#endif

        [Flags]
        public enum BatteryCapabilities : uint
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
#if NET8_0_OR_GREATER
            public BufferReserved3 Reserved;
            public BufferChemistry Chemistry;
#else
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Chemistry;
#endif
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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);
#else

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(Microsoft.Win32.SafeHandles.SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);
#endif
    }

}
