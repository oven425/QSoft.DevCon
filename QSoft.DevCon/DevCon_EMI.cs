using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QSoft.DevCon.EMI
{
    //https://learn.microsoft.com/zh-tw/windows-hardware/drivers/powermeter/energy-meter-interface
    static public partial class DevCon_EMI
    {
        public static ushort EMIVersion(this SafeFileHandle src)
        {
#if NET8_0_OR_GREATER
            ushort version = 0;
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref version, 1));
            DeviceIoControl(src, IOCTL_EMI_GET_VERSION, [], 0, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            return version;
#else
            using var mem_out = new IntPtrMem<ushort>(1);
            DeviceIoControl(src, IOCTL_EMI_GET_VERSION, IntPtr.Zero, 0, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);
            return unchecked((ushort)Marshal.ReadInt16(mem_out.Pointer));
#endif
        }


        public static EMI_Metadata EMIMetadata_v2(this SafeFileHandle src)
        {
#if NET8_0_OR_GREATER
            return EMIMetadataV2Modern(src);
#else
            using var mem_metasize = new IntPtrMem<int>(1);
            DeviceIoControl(src, IOCTL_EMI_GET_METADATA_SIZE, IntPtr.Zero, 0, mem_metasize.Pointer, (uint)mem_metasize.Size, out var reqsz, IntPtr.Zero);
            var metasize = Marshal.ReadInt32(mem_metasize.Pointer);
            using var mem_metadata = new IntPtrMem<byte>(metasize);
            DeviceIoControl(src, IOCTL_EMI_GET_METADATA, IntPtr.Zero, 0, mem_metadata.Pointer, (uint)mem_metadata.Size, out reqsz, IntPtr.Zero);

            var meta = Marshal.PtrToStructure<EMI_METADATA_V2>(mem_metadata.Pointer);
            var offset = Marshal.SizeOf<EMI_METADATA_V2>();
            var channelNames = new string[meta.ChannelCount];
            for (var i = 0; i < meta.ChannelCount; i++)
            {
                var channelPointer = IntPtr.Add(mem_metadata.Pointer, offset);
                var channel = Marshal.PtrToStructure<EMI_CHANNEL_V2>(channelPointer);
                offset = offset+Marshal.SizeOf<EMI_CHANNEL_V2>()-4;
                channelNames[i] = Marshal.PtrToStringUni(IntPtr.Add(mem_metadata.Pointer, offset)) ?? string.Empty;
                offset += channel.ChannelNameSize+2;
            }

            return new EMI_Metadata(meta.HardwareOEM.TrimEnd('\0'), meta.HardwareModel.TrimEnd('\0'), channelNames);
#endif
        }

#if NET8_0_OR_GREATER
        static EMI_Metadata EMIMetadataV2Modern(SafeFileHandle src)
        {
            int metasize = 0;
            var span_out_metasize = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref metasize, 1));
            DeviceIoControl(src, IOCTL_EMI_GET_METADATA_SIZE, [], 0, span_out_metasize, (uint)span_out_metasize.Length, out var reqsz, IntPtr.Zero);
            Span<byte> span_out = stackalloc byte[metasize];
            DeviceIoControl(src, IOCTL_EMI_GET_METADATA, [], 0, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var meta = MemoryMarshal.AsRef<EMI_METADATA_V2>(span_out);
            span_out = span_out[Unsafe.SizeOf<EMI_METADATA_V2>()..];
            var channelNames = new string[meta.ChannelCount];
            for (var i = 0; i < meta.ChannelCount; i++)
            {
                var channel = MemoryMarshal.AsRef<EMI_CHANNEL_V2>(span_out);
                var str = MemoryMarshal.Cast<byte, char>(span_out.Slice(4, channel.ChannelNameSize)).TrimEnd('\0');
                channelNames[i] = new string(str);
                span_out = span_out[(4 + channel.ChannelNameSize + 2)..];
            }
            return new EMI_Metadata(meta.HardwareOEM[..].TrimEnd('\0').ToString(), meta.HardwareModel[..].TrimEnd('\0').ToString(), channelNames);
        }
#endif

        public readonly struct EMI_Metadata(string oem, string model, string[] channels)
        {
            readonly public string OEM = oem;
            readonly public string Model = model;
            readonly public string[] Channels = channels;
        }


        public static EMI_Measurement[] EMI_Get_Measurement(this SafeFileHandle src, int count)
        {
#if NET8_0_OR_GREATER
            Span<EMI_CHANNEL_MEASUREMENT_DATA> ss = stackalloc EMI_CHANNEL_MEASUREMENT_DATA[count];
            var span_out_metasize = MemoryMarshal.AsBytes(ss);
            var hr = DeviceIoControl(src, IOCTL_EMI_GET_MEASUREMENT, [], 0, span_out_metasize, (uint)span_out_metasize.Length, out var reqsz, IntPtr.Zero);

            var mds = new EMI_Measurement[count];
            for(int i=0;i<count;i++)
            {
                mds[i] = new EMI_Measurement(ss[i].AbsoluteEnergy, TimeSpan.FromTicks((long)ss[i].AbsoluteTime));
            }
            return mds;
#else
            using var mem_out = new IntPtrMem<EMI_CHANNEL_MEASUREMENT_DATA>(count);
            DeviceIoControl(src, IOCTL_EMI_GET_MEASUREMENT, IntPtr.Zero, 0, mem_out.Pointer, (uint)mem_out.Size, out var reqsz, IntPtr.Zero);

            var measurements = new EMI_Measurement[count];
            var measurementSize = Marshal.SizeOf<EMI_CHANNEL_MEASUREMENT_DATA>();
            for (var i = 0; i < count; i++)
            {
                var measurement = Marshal.PtrToStructure<EMI_CHANNEL_MEASUREMENT_DATA>(IntPtr.Add(mem_out.Pointer, i * measurementSize));
                measurements[i] = new EMI_Measurement(measurement.AbsoluteEnergy, TimeSpan.FromTicks((long)measurement.AbsoluteTime));
            }

            return measurements;
#endif
        }

        public readonly struct EMI_Measurement(ulong energy, TimeSpan time)
        {
            readonly public ulong Energy = energy;
            readonly public TimeSpan Time = time;
        }


        [StructLayout(LayoutKind.Sequential)]
        struct EMI_CHANNEL_MEASUREMENT_DATA
        {
            public ulong AbsoluteEnergy;
            public ulong AbsoluteTime;
        };


        public enum EMI_MEASUREMENT_UNIT
        {
            EmiMeasurementUnitPicowattHours
        }

#if NET8_0_OR_GREATER
        public sealed record EMIChannelV2(EMI_MEASUREMENT_UNIT MeasurementUnit, string Name);
        public sealed record EMIMetadataV2(string HardwareOEM, string HardwareModel, ushort HardwareRevision, IReadOnlyList<EMIChannelV2> Channels);
#else
        public sealed class EMIChannelV2
        {
            public EMIChannelV2(EMI_MEASUREMENT_UNIT measurementUnit, string name)
            {
                MeasurementUnit = measurementUnit;
                Name = name;
            }

            public EMI_MEASUREMENT_UNIT MeasurementUnit { get; }
            public string Name { get; }
        }

        public sealed class EMIMetadataV2
        {
            public EMIMetadataV2(string hardwareOEM, string hardwareModel, ushort hardwareRevision, IReadOnlyList<EMIChannelV2> channels)
            {
                HardwareOEM = hardwareOEM;
                HardwareModel = hardwareModel;
                HardwareRevision = hardwareRevision;
                Channels = channels;
            }

            public string HardwareOEM { get; }
            public string HardwareModel { get; }
            public ushort HardwareRevision { get; }
            public IReadOnlyList<EMIChannelV2> Channels { get; }
        }
#endif
        //public fixed byte HardwareOEM[EmiConstants.EMI_NAME_MAX];
        //public fixed byte HardwareModel[EmiConstants.EMI_NAME_MAX];
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct EMI_METADATA_V1
        {
            public EMI_MEASUREMENT_UNIT MeasurementUnit;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string HardwareOEM;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string HardwareModel;
            public ushort HardwareRevision;
            public ushort MeteredHardwareNameSize;
            //char MeteredHardwareName[ANYSIZE_ARRAY];
        };

        [StructLayout(LayoutKind.Sequential)]
        struct EMI_CHANNEL_V2
        {
            EMI_MEASUREMENT_UNIT MeasurementUnit;
            public ushort ChannelNameSize;
            //WCHAR ChannelName[ANYSIZE_ARRAY];
        };

#if NET8_0_OR_GREATER

        [InlineArray(16)]
        public struct BufferChar16 { private char _element0; }
        [StructLayout(LayoutKind.Sequential)]
        struct EMI_METADATA_V2
        {
            public BufferChar16 HardwareOEM;
            public BufferChar16 HardwareModel;
            ushort HardwareRevision;
            public ushort ChannelCount;
            //EMI_CHANNEL_V2 Channels[ANYSIZE_ARRAY];
        };
#else
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct EMI_METADATA_V2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string HardwareOEM;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string HardwareModel;
            ushort HardwareRevision;
            public ushort ChannelCount;
            //EMI_CHANNEL_V2 Channels[ANYSIZE_ARRAY];
        };
#endif

        const uint IOCTL_EMI_GET_VERSION = 0x00224000;
        const uint IOCTL_EMI_GET_METADATA_SIZE = 0x00224004;
        const uint IOCTL_EMI_GET_METADATA = 0x00224008;
        const uint IOCTL_EMI_GET_MEASUREMENT = 0x0022400c;
        const ushort EMI_VERSION_V2 = 2;
        public static Guid Guid_EMI = new Guid("{45BD8344-7ED6-49cf-A440-C276C933B053}");
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);
#else
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);
#endif

    }

}

