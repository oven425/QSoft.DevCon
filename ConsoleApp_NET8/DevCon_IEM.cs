using Microsoft.Win32.SafeHandles;
using QSoft.DevCon;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public static ushort EMIVersion(this SafeFileHandle src)
        {
            ushort version = 0;
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref version, 1));
            var hr = DeviceIoControl(src, IOCTL_EMI_GET_VERSION, [], 0, span_out, (uint)span_out.Length, out var reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            return version;
        }


        public static void EMIMetadata_v2(this SafeFileHandle src)
        {
            int metasize = 0;
            var span_out_metasize = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref metasize, 1));
            var hr = DeviceIoControl(src, IOCTL_EMI_GET_METADATA_SIZE, [], 0, span_out_metasize, (uint)span_out_metasize.Length, out var reqsz, IntPtr.Zero);
            Span<byte> span_out = stackalloc byte[metasize];
            DeviceIoControl(src, IOCTL_EMI_GET_METADATA, [], 0, span_out, (uint)span_out.Length, out reqsz, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            var meta = MemoryMarshal.AsRef<EMI_METADATA_V2>(span_out);
            span_out = span_out[Unsafe.SizeOf<EMI_METADATA_V2>()..];
            for(int i=0;i<meta.ChannelCount;i++)
            {
                var channel = MemoryMarshal.AsRef<EMI_CHANNEL_V2>(span_out);
                var str = MemoryMarshal.Cast<byte, char>(span_out.Slice(4, channel.ChannelNameSize)).TrimEnd('\0');
                span_out = span_out[(4 + channel.ChannelNameSize+2)..];
            }
        }

        



        public enum EMI_MEASUREMENT_UNIT
        {
            EmiMeasurementUnitPicowattHours
        }

        public sealed record EMIChannelV2(EMI_MEASUREMENT_UNIT MeasurementUnit, string Name);
        public sealed record EMIMetadataV2(string HardwareOEM, string HardwareModel, ushort HardwareRevision, IReadOnlyList<EMIChannelV2> Channels);
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

        struct EMI_CHANNEL_V2
        {
            EMI_MEASUREMENT_UNIT MeasurementUnit;
            public ushort ChannelNameSize;
            //WCHAR ChannelName[ANYSIZE_ARRAY];
        };


        struct EMI_METADATA_V2
        {
            fixed char HardwareOEM[16];
            fixed char HardwareModel[16];
            ushort HardwareRevision;
            public ushort ChannelCount;
            //EMI_CHANNEL_V2 Channels[ANYSIZE_ARRAY];
        };

        const uint IOCTL_EMI_GET_VERSION = 0x00224000;
        const uint IOCTL_EMI_GET_METADATA_SIZE = 0x00224004;
        const uint IOCTL_EMI_GET_METADATA = 0x00224008;
        const ushort EMI_VERSION_V2 = 2;
        public static Guid Guid_IEM = new Guid("{45BD8344-7ED6-49cf-A440-C276C933B053}");
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }

}
