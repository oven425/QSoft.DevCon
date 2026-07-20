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
            var span_out = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref header, 1));
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, 0);
            span_out = stackalloc byte[(int)header.Size];
            hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out reqsz, 0);
            var desc = MemoryMarshal.Read<STORAGE_DEVICE_DESCRIPTOR>(span_out);

             var vendorid = Span2String(span_out, (int)desc.VendorIdOffset);
            var serilanumber = Span2String(span_out, (int)desc.SerialNumberOffset);
             var productid = Span2String(span_out, (int)desc.ProductIdOffset);
             var revision = Span2String(span_out, (int)desc.ProductRevisionOffset);
        }


        static string Span2String(Span<byte> src, int offset)
        {
            var buffer = src.Slice(offset);
            var index = buffer.IndexOf((byte)0);
            buffer = buffer[..index];
            return System.Text.Encoding.ASCII.GetString(buffer);
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
        public static HealthInfo Nvme_HealthInfoLog(this SafeFileHandle src)
        {
            NVME_DATA<NVME_HEALTH_INFO_LOG>(src, STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage, NVME_LOG_PAGES.NVME_LOG_PAGE_HEALTH_INFO, out var log);
            return new(log);
        }

        public static void NVME_SupportLogPage(this SafeFileHandle src)
        {
            NVME_DATA<NVME_SUPPORTED_LOG_PAGE_DATA>(src, STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage, NVME_LOG_PAGES.NVME_LOG_PAGE_SUPPORTED_LOG_PAGES, out var a);
        }

        static void NVME_DATA<T>(this SafeFileHandle src, STORAGE_PROTOCOL_NVME_DATA_TYPE type, NVME_LOG_PAGES page, out T result) where T : struct
        {
            var Query = new STORAGE_PROPERTY_QUERY
            {
                PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProtocolSpecificProperty,
                QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
            };
            var desc_len = Marshal.SizeOf<STORAGE_PROTOCOL_DATA_DESCRIPTOR>();
            var data_len = Marshal.SizeOf<T>();
            var ProtocolSpecific = new STORAGE_PROTOCOL_SPECIFIC_DATA
            {
                ProtocolType = STORAGE_PROTOCOL_TYPE.ProtocolTypeNvme,
                DataType = (uint)type,
                ProtocolDataRequestValue = (uint)page,
                ProtocolDataRequestSubValue =0,
                ProtocolDataOffset = (uint)Marshal.SizeOf<STORAGE_PROTOCOL_SPECIFIC_DATA>(),
                ProtocolDataLength = (uint)data_len,
                ProtocolDataRequestSubValue4 = 1
            };
            var bufferLength = (int)Marshal.OffsetOf<STORAGE_PROPERTY_QUERY>("AdditionalParameters") + Marshal.SizeOf<STORAGE_PROTOCOL_SPECIFIC_DATA>();
            Span<byte> span_in = stackalloc byte[bufferLength];

            MemoryMarshal.Write(span_in, in Query);

            int offsetProtocolSpecific = (int)Marshal.OffsetOf<STORAGE_PROPERTY_QUERY>("AdditionalParameters");

            MemoryMarshal.Write(span_in[offsetProtocolSpecific..], in ProtocolSpecific);

            Span<byte> span_out = stackalloc byte[data_len + desc_len];
            var hr = DeviceIoControl(src, IOCTL_STORAGE_QUERY_PROPERTY, span_in, (uint)span_in.Length, span_out, (uint)span_out.Length, out var reqsz, 0);
            var err = Marshal.GetLastWin32Error();
            var desc = MemoryMarshal.Read<STORAGE_PROTOCOL_DATA_DESCRIPTOR>(span_out);
            result = MemoryMarshal.Read<T>(span_out[Marshal.SizeOf<STORAGE_PROTOCOL_DATA_DESCRIPTOR>()..]);
            
        }

        [InlineArray(22)]
        public struct byte_22 { private byte _element0; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct NVME_ERROR_INFO_LOG
        {
            public ulong ErrorCount;               // 0  錯誤計數
            public ushort SQID;                     // 8  Submission Queue ID
            public ushort CMDID;                    // 10 Command ID
            public ushort StatusField;              // 12 [0]=Phase, [15:1]=Status Code
            public ushort ParameterErrorLocation;  // 14 參數錯誤位置
            public ulong Lba;                      // 16 邏輯區塊位址
            public uint NameSpace;               // 24 Namespace
            public byte VendorInfoValid;         // 28 廠商資訊是否有效
            public byte Trtype;                  // 29 Transport Type (NVMe 1.4+)
            public ushort Reserved0;               // 30
            public ulong CommandSpecificInfo;     // 32 命令特定資訊
            public ushort TrtypeSpecificInfo;      // 40 Transport Type 特定資訊
            byte_22 Reserved1;               // 42

            public bool Phase => (StatusField & 0x0001) != 0;
            public ushort StatusCode => (ushort)((StatusField >> 1) & 0x7FFF);
        }

        public static void Nvme_ErrorInfoLog(this SafeFileHandle src)
        {
            NVME_DATA<NVME_ERROR_INFO_LOG>(src, STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage, NVME_LOG_PAGES.NVME_LOG_PAGE_ERROR_INFO, out var log);
        }

        [InlineArray(7)] public struct byte_7 { private byte _element0; }
        [InlineArray(8)] public struct byte_8 { private byte _element0; }
        [InlineArray(448)] struct byte_448 { private byte _element0; }

        // 7 個插槽 × 8 bytes = 56 bytes
        [InlineArray(7)]
        struct NVME_FRS_ARRAY { private byte_8 _element0; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct NVME_FIRMWARE_SLOT_INFO_LOG
        {
            public byte AFI;        // 0  Active Firmware Info
                                    //    [2:0] = ActiveSlot (1-7)
                                    //    [6:4] = PendingActivateSlot (NVMe 1.2+)
            byte_7 Reserved0;  // 1
            public NVME_FRS_ARRAY FRS;     // 8  Firmware Revision Slot 1~7（各 8 bytes ASCII）
            byte_448 Reserved1;  // 64

            public int ActiveSlot => (AFI >> 0) & 0x07;  // 目前作用中插槽
            public int PendingActivateSlot => (AFI >> 4) & 0x07;  // 下次重啟後生效插槽
            public string GetRevision(int slot)
            {
                ref byte_8 frs = ref FRS[slot - 1];
                var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref frs, 1));
                return System.Text.Encoding.ASCII.GetString(span).TrimEnd('\0', ' ');
            }
        }

        public static void Nvme_FirmwareSlotInfo(this SafeFileHandle src)
        {
            NVME_DATA<NVME_FIRMWARE_SLOT_INFO_LOG>(src, STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage, NVME_LOG_PAGES.NVME_LOG_PAGE_FIRMWARE_SLOT_INFO, out var log);
        }

        public enum NVME_LOG_PAGES
        {
            NVME_LOG_PAGE_SUPPORTED_LOG_PAGES = 0x00,
            NVME_LOG_PAGE_ERROR_INFO = 0x01,
            NVME_LOG_PAGE_HEALTH_INFO = 0x02,
            NVME_LOG_PAGE_FIRMWARE_SLOT_INFO = 0x03,
            NVME_LOG_PAGE_CHANGED_NAMESPACE_LIST = 0x04,
            NVME_LOG_PAGE_COMMAND_EFFECTS = 0x05,
            NVME_LOG_PAGE_DEVICE_SELF_TEST = 0x06,
            NVME_LOG_PAGE_TELEMETRY_HOST_INITIATED = 0x07,
            NVME_LOG_PAGE_TELEMETRY_CTLR_INITIATED = 0x08,
            NVME_LOG_PAGE_ENDURANCE_GROUP_INFORMATION = 0x09,
            NVME_LOG_PAGE_PREDICTABLE_LATENCY_NVM_SET = 0x0A,
            NVME_LOG_PAGE_PREDICTABLE_LATENCY_EVENT_AGGREGATE = 0x0B,
            NVME_LOG_PAGE_ASYMMETRIC_NAMESPACE_ACCESS = 0x0C,
            NVME_LOG_PAGE_PERSISTENT_EVENT_LOG = 0x0D,
            NVME_LOG_PAGE_LBA_STATUS_INFORMATION = 0x0E,     // NVM Express NVM Command Set
            NVME_LOG_PAGE_ENDURANCE_GROUP_EVENT_AGGREGATE = 0x0F,
            NVME_LOG_PAGE_MEDIA_UNIT_STATUS = 0x10,
            NVME_LOG_PAGE_SUPPORTED_CAPACITY_CONFIGURATION_LIST = 0X11,
            NVME_LOG_PAGE_FEATURE_IDENTIFIERS_SUPPORTED_AND_EFFECTS = 0x12,
            NVME_LOG_PAGE_NVME_MI_COMMANDS_SUPPORTED_AND_EFFECTS = 0x13,
            NVME_LOG_PAGE_COMMAND_AND_FEATURE_LOCKDOWN = 0x14,
            NVME_LOG_PAGE_BOOT_PARTITON = 0x15,
            NVME_LOG_PAGE_ROTATIONAL_MEDIA_INFORMATION = 0x16,
            NVME_LOG_PAGE_DISCOVERY = 0x70,
            NVME_LOG_PAGE_RESERVATION_NOTIFICATION = 0x80,
            NVME_LOG_PAGE_SANITIZE_STATUS = 0x81,
            NVME_LOG_PAGE_CHANGED_ZONE_LIST = 0xBF,     // NVM Express Zoned Namespace Command Set

        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct NVME_SUPPORTED_LOG_PAGE_DATA   // 4 bytes
        {
            public uint AsUlong;

            public bool LSUPP => (AsUlong & (1u << 0)) != 0; // Log Page Supported
            public bool IOS => (AsUlong & (1u << 1)) != 0; // I/O Command Set Specific
            public bool EXS => (AsUlong & (1u << 2)) != 0; // Extended Data Structures
        }

        [InlineArray(256)]
        struct NVME_SUPPORTED_LOG_PAGES_ARRAY { private NVME_SUPPORTED_LOG_PAGE_DATA _element0; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct NVME_SUPPORTED_LOG_PAGES       // 256 × 4 = 1024 bytes
        {
            public NVME_SUPPORTED_LOG_PAGES_ARRAY LogPageData;
        }

        [InlineArray(16)]
        public struct byte_16 { private byte _element0; }
        [InlineArray(26)]
        public struct byte_26 { private byte _element0; }
        [InlineArray(296)]
        public struct byte_296 { private byte _element0; }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NVME_HEALTH_INFO_LOG
        {

            //    union {

            //struct {
            //        UCHAR AvailableSpaceLow   : 1;                    // If set to 1, then the available spare space has fallen below the threshold.
            //        UCHAR TemperatureThreshold : 1;                   // If set to 1, then a temperature is above an over temperature threshold or below an under temperature threshold.
            //        UCHAR ReliabilityDegraded : 1;                    // If set to 1, then the device reliability has been degraded due to significant media related  errors or any internal error that degrades device reliability.
            //        UCHAR ReadOnly            : 1;                    // If set to 1, then the media has been placed in read only mode
            //        UCHAR VolatileMemoryBackupDeviceFailed    : 1;    // If set to 1, then the volatile memory backup device has failed. This field is only valid if the controller has a volatile memory backup solution.
            //        UCHAR Reserved                            : 3;
            //    }
            //    DUMMYSTRUCTNAME;

            //UCHAR AsUchar;

            //}
            //CriticalWarning;    // This field indicates critical warnings for the state of the  controller. Each bit corresponds to a critical warning type; multiple bits may be set.
            public byte CriticalWarning;
            public byte_2 Temperature;                 // Temperature: Contains the temperature of the overall device (controller and NVM included) in units of Kelvin. If the temperature exceeds the temperature threshold, refer to section 5.12.1.4, then an asynchronous event completion may occur
            public byte AvailableSpare;                 // Available Spare:  Contains a normalized percentage (0 to 100%) of the remaining spare capacity available
            byte AvailableSpareThreshold;        // Available Spare Threshold:  When the Available Spare falls below the threshold indicated in this field, an asynchronous event  completion may occur. The value is indicated as a normalized percentage (0 to 100%).
            public byte PercentageUsed;                 // Percentage Used
            byte_26 Reserved0;

            byte_16 DataUnitRead;               // Data Units Read:  Contains the number of 512 byte data units the host has read from the controller; this value does not include metadata. This value is reported in thousands (i.e., a value of 1 corresponds to 1000 units of 512 bytes read)  and is rounded up.  When the LBA size is a value other than 512 bytes, the controller shall convert the amount of data read to 512 byte units. For the NVM command set, logical blocks read as part of Compare and Read operations shall be included in this value
            byte_16 DataUnitWritten;            // Data Units Written: Contains the number of 512 byte data units the host has written to the controller; this value does not include metadata. This value is reported in thousands (i.e., a value of 1 corresponds to 1000 units of 512 bytes written)  and is rounded up.  When the LBA size is a value other than 512 bytes, the controller shall convert the amount of data written to 512 byte units. For the NVM command set, logical blocks written as part of Write operations shall be included in this value. Write Uncorrectable commands shall not impact this value.
            byte_16 HostReadCommands;           // Host Read Commands:  Contains the number of read commands  completed by  the controller. For the NVM command set, this is the number of Compare and Read commands.
            byte_16 HostWrittenCommands;        // Host Write Commands:  Contains the number of write commands  completed by  the controller. For the NVM command set, this is the number of Write commands.
            byte_16 ControllerBusyTime;         // Controller Busy Time:  Contains the amount of time the controller is busy with I/O commands. The controller is busy when there is a command outstanding to an I/O Queue (specifically, a command was issued via an I/O Submission Queue Tail doorbell write and the corresponding  completion queue entry  has not been posted yet to the associated I/O Completion Queue). This value is reported in minutes.
            byte_16 PowerCycle;                 // Power Cycles: Contains the number of power cycles.
            byte_16 PowerOnHours;               // Power On Hours: Contains the number of power-on hours. This does not include time that the controller was powered and in a low power state condition.
            byte_16 UnsafeShutdowns;            // Unsafe Shutdowns: Contains the number of unsafe shutdowns. This count is incremented when a shutdown notification (CC.SHN) is not received prior to loss of power.
            byte_16 MediaErrors;                // Media Errors:  Contains the number of occurrences where the controller detected an unrecovered data integrity error. Errors such as uncorrectable ECC, CRC checksum failure, or LBA tag mismatch are included in this field.
            byte_16 ErrorInfoLogEntryCount;     // Number of Error Information Log Entries:  Contains the number of Error Information log entries over the life of the controller
            uint WarningCompositeTemperatureTime;     // Warning Composite Temperature Time: Contains the amount of time in minutes that the controller is operational and the Composite Temperature is greater than or equal to the Warning Composite Temperature Threshold (WCTEMP) field and less than the Critical Composite Temperature Threshold (CCTEMP) field in the Identify Controller data structure
            uint CriticalCompositeTemperatureTime;    // Critical Composite Temperature Time: Contains the amount of time in minutes that the controller is operational and the Composite Temperature is greater the Critical Composite Temperature Threshold (CCTEMP) field in the Identify Controller data structure
            ushort TemperatureSensor1;          // Contains the current temperature reported by temperature sensor 1.
            ushort TemperatureSensor2;          // Contains the current temperature reported by temperature sensor 2.
            ushort TemperatureSensor3;          // Contains the current temperature reported by temperature sensor 3.
            ushort TemperatureSensor4;          // Contains the current temperature reported by temperature sensor 4.
            ushort TemperatureSensor5;          // Contains the current temperature reported by temperature sensor 5.
            ushort TemperatureSensor6;          // Contains the current temperature reported by temperature sensor 6.
            ushort TemperatureSensor7;          // Contains the current temperature reported by temperature sensor 7.
            ushort TemperatureSensor8;          // Contains the current temperature reported by temperature sensor 8.
            byte_296 Reserved1;

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
            public uint ProtocolDataRequestSubValue4;
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

        public readonly struct HealthInfo
        {
            public byte CriticalWarning { get; }
            public double Temperature { get; }
            public byte AvailableSpare { get; }
            public byte PercentageUsed { get; }

            internal HealthInfo(in NVME_HEALTH_INFO_LOG log)
            {
                CriticalWarning = log.CriticalWarning;
                Temperature = BitConverter.ToInt16(log.Temperature) - 273.15;
                AvailableSpare = log.AvailableSpare;
                PercentageUsed = log.PercentageUsed;
            }
        }

    }

    
}
