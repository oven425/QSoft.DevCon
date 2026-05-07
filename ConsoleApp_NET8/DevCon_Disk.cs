using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp_NET8
{
    public  static partial class DevCon_Disk
    {
        public static void DESCRIPTOR(this SafeFileHandle src)
        {
            var query = new STORAGE_PROPERTY_QUERY
            {
                PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty,
                QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
            };
            var header = new STORAGE_DESCRIPTOR_HEADER();
            var span_in =MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref query, 1));
            //var spin_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref desc, 1));
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref header, 1));
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, 0);
            span_out = stackalloc byte[(int)header.Size];
            hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, 0);
            var desc = MemoryMarshal.Read<STORAGE_DEVICE_DESCRIPTOR>(span_out);
            var offset = span_out.Slice((int)desc.ProductIdOffset);
            var index = offset.IndexOf((byte)0);
            offset = offset[.. index];
            var aaa = System.Text.Encoding.ASCII.GetString(offset);
            
        }

        public static void DeviceTemperature(this SafeFileHandle src)
        {
            var query = new STORAGE_PROPERTY_QUERY
            {
                PropertyId = STORAGE_PROPERTY_ID.StorageDeviceTemperatureProperty,
                QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
            };
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref query, 1));
            var header = new STORAGE_DESCRIPTOR_HEADER();
            var span_out_header = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref header, 1));
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out_header, (uint)span_out_header.Length, out var reqsz, 0);
            var err = Marshal.GetLastWin32Error();
            Span<byte> span_out_buf = stackalloc byte[(int)header.Size];
            hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out_buf, (uint)span_out_buf.Length, out reqsz, 0);
            var temperature_desc = MemoryMarshal.Cast<byte, STORAGE_TEMPERATURE_DATA_DESCRIPTOR>(span_out_buf)[0];
            var span_infos = span_out_buf[Marshal.SizeOf<STORAGE_TEMPERATURE_DATA_DESCRIPTOR>()..];
            var infos  = MemoryMarshal.Cast<byte, STORAGE_TEMPERATURE_INFO>(span_infos);

        }

        public static void AdapterTemperature(this SafeFileHandle src)
        {
            var query = new STORAGE_PROPERTY_QUERY
            {
                PropertyId = STORAGE_PROPERTY_ID.StorageAdapterTemperatureProperty,
                QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
            };
            var span_in = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref query, 1));
            var header = new STORAGE_DESCRIPTOR_HEADER();
            var span_out_header = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref header, 1));
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out_header, (uint)span_out_header.Length, out var reqsz, 0);
            var err = Marshal.GetLastWin32Error();
            Span<byte> span_out_buf = stackalloc byte[(int)header.Size];
            hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out_buf, (uint)span_out_buf.Length, out reqsz, 0);
            var temperature_desc = MemoryMarshal.Cast<byte, STORAGE_TEMPERATURE_DATA_DESCRIPTOR>(span_out_buf)[0];
            var span_infos = span_out_buf[Marshal.SizeOf<STORAGE_TEMPERATURE_DATA_DESCRIPTOR>()..];
            var infos = MemoryMarshal.Cast<byte, STORAGE_TEMPERATURE_INFO>(span_infos);

        }

        //https://zhung.com.tw/article/%E5%88%A9%E7%94%A8windows%E5%86%85%E5%BB%BA%E7%9A%84driver%E9%80%8F%E9%81%8Eioctl%E7%99%BC%E9%80%81nvme-command/
        public static void NVME_SMART(this SafeFileHandle src)
        {
            var Query = new STORAGE_PROPERTY_QUERY
            {
                PropertyId = STORAGE_PROPERTY_ID.StorageAdapterProtocolSpecificProperty,
                QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
            };

            var ProtocolSpecific = new STORAGE_PROTOCOL_SPECIFIC_DATA
            {
                ProtocolType = STORAGE_PROTOCOL_TYPE.ProtocolTypeNvme,
                DataType = (uint)STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage,
                ProtocolDataRequestValue = 0,
                ProtocolDataRequestSubValue = 2,
                ProtocolDataOffset = (uint)Marshal.SizeOf<STORAGE_PROTOCOL_SPECIFIC_DATA>(),
                ProtocolDataLength = 4096,
            };
            var bufferLength = (int)Marshal.OffsetOf<STORAGE_PROPERTY_QUERY>("AdditionalParameters") + Marshal.SizeOf<STORAGE_PROTOCOL_SPECIFIC_DATA>() + 4096;
            Span<byte> span_in = stackalloc byte[bufferLength];

            
            MemoryMarshal.Write(span_in, in Query);

            // 計算 ProtocolSpecific 的起始位置
            int offsetProtocolSpecific =
                (int)Marshal.OffsetOf<STORAGE_PROPERTY_QUERY>("AdditionalParameters");

            // 寫入 ProtocolSpecific
            MemoryMarshal.Write(span_in.Slice(offsetProtocolSpecific), in ProtocolSpecific);
            var strb = new StringBuilder();
            for(int i=0;i< span_in.Length; i++)
            {
                strb.AppendLine($"{i}:{span_in[i]} ");
            }
            File.WriteAllText("nvme.txt", strb.ToString());
            var bufferbin = File.ReadAllBytes("buffer.bin");
            span_in = bufferbin.AsSpan();
            Span<byte> span_out_header = stackalloc byte[8192];
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_in, (uint)span_in.Length, out var reqsz, 0);
            var err = Marshal.GetLastWin32Error();

        }
        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_QUERY_WITH_PROTOCOL_SPECIFIC
        {
            public STORAGE_PROPERTY_QUERY Query;
            public STORAGE_PROTOCOL_SPECIFIC_DATA ProtocolSpecific;
        }

        enum STORAGE_PROTOCOL_NVME_DATA_TYPE
        {
            NVMeDataTypeUnknown,
            NVMeDataTypeIdentify,
            NVMeDataTypeLogPage,
            NVMeDataTypeFeature,
            NVMeDataTypeLogPageEx,
            NVMeDataTypeFeatureEx
        }

        enum STORAGE_PROTOCOL_TYPE
        {
            ProtocolTypeUnknown = 0x00,
            ProtocolTypeScsi,
            ProtocolTypeAta,
            ProtocolTypeNvme,
            ProtocolTypeSd,
            ProtocolTypeUfs,
            ProtocolTypeProprietary = 0x7E,
            ProtocolTypeMaxReserved = 0x7F
        }
        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_PROTOCOL_SPECIFIC_DATA
        {
            public STORAGE_PROTOCOL_TYPE ProtocolType;
            public uint DataType;
            public uint ProtocolDataRequestValue;
            public uint ProtocolDataRequestSubValue;
            public uint ProtocolDataOffset;
            public uint ProtocolDataLength;
            uint FixedProtocolReturnData;
            uint ProtocolDataRequestSubValue2;
            uint ProtocolDataRequestSubValue3;
            uint ProtocolDataRequestSubValue4;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_PROTOCOL_DATA_DESCRIPTOR
        {
            uint Version;
            uint Size;
            STORAGE_PROTOCOL_SPECIFIC_DATA ProtocolSpecificData;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_TEMPERATURE_INFO
        {
            ushort Index;
            short Temperature;
            short OverThreshold;
            short UnderThreshold;
            byte OverThresholdChangable;
            byte UnderThresholdChangable;
            byte EventGenerated;
            byte Reserved0;
            uint Reserved1;
        }
        [InlineArray(1)]
        public struct byte_1 { private byte _element0; }
        [InlineArray(2)]
        public struct byte_2 { private byte _element0; }
        [InlineArray(2)]
        public struct uint_2 { private uint _element0; }


        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_TEMPERATURE_DATA_DESCRIPTOR
        {
            uint Version;
            uint Size;
            short CriticalTemperature;
            short WarningTemperature;
            ushort InfoCount;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            byte_2 Reserved0;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            uint_2 Reserved1;
            //STORAGE_TEMPERATURE_INFO TemperatureInfo[ANYSIZE_ARRAY];
        }

        struct STORAGE_DESCRIPTOR_HEADER
        {
            uint Version;
            public uint Size;
        };
        const uint IOCTL_STORAGE_FIRMWARE_GET_INFO = 0x002D0C14;
        const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x002D1400;
        enum STORAGE_PROPERTY_ID
        {
            StorageDeviceProperty = 0,
            StorageAdapterProperty,
            StorageDeviceIdProperty,
            StorageDeviceUniqueIdProperty,
            StorageDeviceWriteCacheProperty,
            StorageMiniportProperty,
            StorageAccessAlignmentProperty,
            StorageDeviceSeekPenaltyProperty,
            StorageDeviceTrimProperty,
            StorageDeviceWriteAggregationProperty,
            StorageDeviceDeviceTelemetryProperty,
            StorageDeviceLBProvisioningProperty,
            StorageDevicePowerProperty,
            StorageDeviceCopyOffloadProperty,
            StorageDeviceResiliencyProperty,
            StorageDeviceMediumProductType,
            StorageAdapterRpmbProperty,
            StorageAdapterCryptoProperty,
            StorageDeviceIoCapabilityProperty = 48,
            StorageAdapterProtocolSpecificProperty,
            StorageDeviceProtocolSpecificProperty,
            StorageAdapterTemperatureProperty,
            StorageDeviceTemperatureProperty,
            StorageAdapterPhysicalTopologyProperty,
            StorageDevicePhysicalTopologyProperty,
            StorageDeviceAttributesProperty,
            StorageDeviceManagementStatus,
            StorageAdapterSerialNumberProperty,
            StorageDeviceLocationProperty,
            StorageDeviceNumaProperty,
            StorageDeviceZonedDeviceProperty,
            StorageDeviceUnsafeShutdownCount,
            StorageDeviceEnduranceProperty,
            StorageDeviceLedStateProperty,
            StorageDeviceSelfEncryptionProperty = 64,
            StorageFruIdProperty,
            StorageStackProperty,
            StorageAdapterProtocolSpecificPropertyEx,
            StorageDeviceProtocolSpecificPropertyEx
        }
        enum STORAGE_QUERY_TYPE
        {
            PropertyStandardQuery = 0,
            PropertyExistsQuery,
            PropertyMaskQuery,
            PropertyQueryMaxDefined
        }

        [StructLayout(LayoutKind.Sequential)]
        struct STORAGE_PROPERTY_QUERY 
        {
            public STORAGE_PROPERTY_ID PropertyId;
            public STORAGE_QUERY_TYPE QueryType;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            //public byte[] AdditionalParameters;
            public byte_1 AdditionalParameters;
        }

        public enum STORAGE_BUS_TYPE
        {
            BusTypeUnknown = 0x00,
            BusTypeScsi,
            BusTypeAtapi,
            BusTypeAta,
            BusType1394,
            BusTypeSsa,
            BusTypeFibre,
            BusTypeUsb,
            BusTypeRAID,
            BusTypeiScsi,
            BusTypeSas,
            BusTypeSata,
            BusTypeSd,
            BusTypeMmc,
            BusTypeVirtual,
            BusTypeFileBackedVirtual,
            BusTypeSpaces,
            BusTypeNvme,
            BusTypeSCM,
            BusTypeUfs,
            BusTypeNvmeof,
            BusTypeMax,
            BusTypeMaxReserved = 0x7F
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_DEVICE_DESCRIPTOR
        {
            public uint Version;
            public uint Size;
            public byte DeviceType;
            public byte DeviceTypeModifier;
            [MarshalAs(UnmanagedType.U1)]
            public bool RemovableMedia;
            [MarshalAs(UnmanagedType.U1)]
            public bool CommandQueueing;
            public uint VendorIdOffset;
            public uint ProductIdOffset;
            public uint ProductRevisionOffset;
            public uint SerialNumberOffset;
            public STORAGE_BUS_TYPE BusType;
            public uint RawPropertiesLength;
            // Flexible array member → represent as IntPtr or byte[]
            // If using P/Invoke, usually IntPtr is safer
            public byte RawDeviceProperties;
        }


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
