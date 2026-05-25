// ConsoleApplication_SMBIOS.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <windows.h>
#include <vector>
#include <map>
#include <span>
typedef unsigned __int64 QWORD;
// ===== Type 0: BIOS Information (根據版本分類) =====

    // Type 0 - SMBIOS 2.0
struct Type0_BIOS_v2_0 {
    BYTE Vendor;                      // String
    BYTE Version;                     // String
    WORD StartingAddressSegment;
    BYTE ReleaseDate;                 // String
    BYTE ROMSize;
    QWORD Characteristics;
};

// Type 0 - SMBIOS 2.4 (新增: CharacteristicsExt1, CharacteristicsExt2, 版本號)
struct Type0_BIOS_v2_4 {
    BYTE Vendor;
    BYTE Version;
    WORD StartingAddressSegment;
    BYTE ReleaseDate;
    BYTE ROMSize;
    QWORD Characteristics;
    BYTE CharacteristicsExt1;
    BYTE CharacteristicsExt2;
};

// Type 0 - SMBIOS 2.6 (新增: BIOS 主次版本號)
struct Type0_BIOS_v2_6 {
    BYTE Vendor;
    BYTE Version;
    WORD StartingAddressSegment;
    BYTE ReleaseDate;
    BYTE ROMSize;
    QWORD Characteristics;
    BYTE CharacteristicsExt1;
    BYTE CharacteristicsExt2;
    BYTE SystemBIOSMajorRelease;
    BYTE SystemBIOSMinorRelease;
    BYTE EmbeddedControllerFirmwareMajor;
    BYTE EmbeddedControllerFirmwareMinor;
};

// ===== Type 1: System Information (根據版本分類) =====

// Type 1 - SMBIOS 2.0
struct Type1_System_v2_0 {
    BYTE Manufacturer;                // String
    BYTE ProductName;                 // String
    BYTE Version;                     // String
    BYTE SerialNumber;                // String
};

// Type 1 - SMBIOS 2.1 (新增: UUID, WakeupType)
struct Type1_System_v2_1 {
    BYTE Manufacturer;
    BYTE ProductName;
    BYTE Version;
    BYTE SerialNumber;
    GUID UUID;
    BYTE WakeupType;
};

// Type 1 - SMBIOS 2.4 (新增: SKUNumber, Family)
struct Type1_System_v2_4 {
    BYTE Manufacturer;
    BYTE ProductName;
    BYTE Version;
    BYTE SerialNumber;
    GUID UUID;
    BYTE WakeupType;
    BYTE SKUNumber;                   // String
    BYTE Family;                      // String
};

// ===== Type 2: Baseboard Information (根據版本分類) =====

// Type 2 - SMBIOS 2.0
struct Type2_Baseboard_v2_0 {
    BYTE Manufacturer;                // String
    BYTE ProductName;                 // String
    BYTE Version;                     // String
    BYTE SerialNumber;                // String
    BYTE AssetTag;                    // String
    BYTE FeatureFlags;
    BYTE LocationInChassis;           // String
    WORD ChassisHandle;
    BYTE BoardType;
};

// Type 2 - SMBIOS 2.6 (新增: NumberOfContainedObjectHandles)
struct Type2_Baseboard_v2_6 {
    BYTE Manufacturer;
    BYTE ProductName;
    BYTE Version;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE FeatureFlags;
    BYTE LocationInChassis;
    WORD ChassisHandle;
    BYTE BoardType;
    BYTE NumberOfContainedObjectHandles;
};

// ===== Type 3: System Chassis (根據版本分類) =====

// Type 3 - SMBIOS 2.0
struct Type3_Chassis_v2_0 {
    BYTE Manufacturer;                // String
    BYTE Type;
    BYTE Version;                     // String
    BYTE SerialNumber;                // String
    BYTE AssetTag;                    // String
    BYTE BootupState;
    BYTE PowerSupplyState;
    BYTE ThermalState;
    BYTE SecurityStatus;
    DWORD OEMSpecific;
};

// Type 3 - SMBIOS 2.3 (新增: Height, NumberOfPowerCords)
struct Type3_Chassis_v2_3 {
    BYTE Manufacturer;
    BYTE Type;
    BYTE Version;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE BootupState;
    BYTE PowerSupplyState;
    BYTE ThermalState;
    BYTE SecurityStatus;
    DWORD OEMSpecific;
    BYTE Height;
    BYTE NumberOfPowerCords;
};

// Type 3 - SMBIOS 2.7 (新增: NumberOfContainedElements, ContainedElementMinimumLength)
struct Type3_Chassis_v2_7 {
    BYTE Manufacturer;
    BYTE Type;
    BYTE Version;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE BootupState;
    BYTE PowerSupplyState;
    BYTE ThermalState;
    BYTE SecurityStatus;
    DWORD OEMSpecific;
    BYTE Height;
    BYTE NumberOfPowerCords;
    BYTE NumberOfContainedElements;
    BYTE ContainedElementMinimumLength;
};

// ===== Type 4: Processor Information (根據版本分類) =====

// Type 4 - SMBIOS 2.0
struct Type4_Processor_v2_0 {
    BYTE SocketDesignation;           // String
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;       // String
    QWORD ProcessorID;
    BYTE ProcessorVersion;            // String
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
};

// Type 4 - SMBIOS 2.1 (新增: L1/L2/L3 Cache Handle)
struct Type4_Processor_v2_1 {
    BYTE SocketDesignation;
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;
    QWORD ProcessorID;
    BYTE ProcessorVersion;
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
    WORD L1CacheHandle;
    WORD L2CacheHandle;
    WORD L3CacheHandle;
};

// Type 4 - SMBIOS 2.3 (新增: SerialNumber, AssetTag, PartNumber)
struct Type4_Processor_v2_3 {
    BYTE SocketDesignation;
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;
    QWORD ProcessorID;
    BYTE ProcessorVersion;
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
    WORD L1CacheHandle;
    WORD L2CacheHandle;
    WORD L3CacheHandle;
    BYTE SerialNumber;                // String
    BYTE AssetTag;                    // String
    BYTE PartNumber;                  // String
};

// Type 4 - SMBIOS 2.5 (新增: CoreCount, CoreEnabled, ThreadCount)
struct Type4_Processor_v2_5 {
    BYTE SocketDesignation;
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;
    QWORD ProcessorID;
    BYTE ProcessorVersion;
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
    WORD L1CacheHandle;
    WORD L2CacheHandle;
    WORD L3CacheHandle;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE CoreCount;
    BYTE CoreEnabled;
    BYTE ThreadCount;
    WORD ProcessorCharacteristics;
};

// Type 4 - SMBIOS 2.6 (新增: ProcessorFamily2)
struct Type4_Processor_v2_6 {
    BYTE SocketDesignation;
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;
    QWORD ProcessorID;
    BYTE ProcessorVersion;
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
    WORD L1CacheHandle;
    WORD L2CacheHandle;
    WORD L3CacheHandle;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE CoreCount;
    BYTE CoreEnabled;
    BYTE ThreadCount;
    WORD ProcessorCharacteristics;
    WORD ProcessorFamily2;
};

// Type 4 - SMBIOS 3.0 (新增: 64位 CoreCount, CoreEnabled, ThreadCount)
struct Type4_Processor_v3_0 {
    BYTE SocketDesignation;
    BYTE ProcessorType;
    BYTE ProcessorFamily;
    BYTE ProcessorManufacturer;
    QWORD ProcessorID;
    BYTE ProcessorVersion;
    BYTE Voltage;
    WORD ExternalClockSpeed;
    WORD MaxSpeed;
    WORD CurrentSpeed;
    BYTE Status;
    BYTE ProcessorUpgrade;
    WORD L1CacheHandle;
    WORD L2CacheHandle;
    WORD L3CacheHandle;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE CoreCount;
    BYTE CoreEnabled;
    BYTE ThreadCount;
    WORD ProcessorCharacteristics;
    WORD ProcessorFamily2;
    WORD CoreCount2;
    WORD CoreEnabled2;
    WORD ThreadCount2;
};

// ===== Type 7: Cache Information (根據版本分類) =====

// Type 7 - SMBIOS 2.0
struct Type7_Cache_v2_0 {
    BYTE SocketDesignation;           // String
    WORD CacheConfiguration;
    WORD MaximumCacheSize;
    WORD InstalledSize;
    WORD SupportedSRAMType;
    WORD CurrentSRAMType;
    BYTE CacheSpeed;
    BYTE ErrorCorrectionType;
    BYTE SystemCacheType;
    BYTE Associativity;
};

// Type 7 - SMBIOS 3.1 (新增: 64位 CacheSize)
struct Type7_Cache_v3_1 {
    BYTE SocketDesignation;
    WORD CacheConfiguration;
    WORD MaximumCacheSize;
    WORD InstalledSize;
    WORD SupportedSRAMType;
    WORD CurrentSRAMType;
    BYTE CacheSpeed;
    BYTE ErrorCorrectionType;
    BYTE SystemCacheType;
    BYTE Associativity;
    DWORD MaximumCacheSize2;
    DWORD InstalledSize2;
};

// ===== Type 8: Port Connector Information =====

// Type 8 - SMBIOS 2.0
struct Type8_PortConnector_v2_0 {
    BYTE InternalReferenceDesignator; // String
    BYTE InternalConnectorType;
    BYTE ExternalReferenceDesignator; // String
    BYTE ExternalConnectorType;
    BYTE PortType;
};

// ===== Type 9: System Slot Information (根據版本分類) =====

// Type 9 - SMBIOS 2.0
struct Type9_SystemSlot_v2_0 {
    BYTE SlotDesignation;             // String
    BYTE SlotType;
    BYTE SlotDataBusWidth;
    BYTE CurrentUsage;
    BYTE SlotLength;
    WORD SlotID;
    BYTE SlotCharacteristics1;
};

// Type 9 - SMBIOS 2.6 (新增: SlotCharacteristics2)
struct Type9_SystemSlot_v2_6 {
    BYTE SlotDesignation;
    BYTE SlotType;
    BYTE SlotDataBusWidth;
    BYTE CurrentUsage;
    BYTE SlotLength;
    WORD SlotID;
    BYTE SlotCharacteristics1;
    BYTE SlotCharacteristics2;
};

// Type 9 - SMBIOS 3.1 (新增: Segment/Bus/Device資訊)
struct Type9_SystemSlot_v3_1 {
    BYTE SlotDesignation;
    BYTE SlotType;
    BYTE SlotDataBusWidth;
    BYTE CurrentUsage;
    BYTE SlotLength;
    WORD SlotID;
    BYTE SlotCharacteristics1;
    BYTE SlotCharacteristics2;
    WORD SegmentGroupNumber;
    BYTE BusNumber;
    BYTE DeviceFunctionNumber;
};

// ===== Type 10: On Board Devices Information =====

// Type 10 - SMBIOS 2.0
struct Type10_OnBoardDevices_v2_0 {
    BYTE DeviceType;
    BYTE DeviceTypeInstance;
    BYTE DeviceStatus;
    BYTE DeviceDescription;           // String
};

// ===== Type 11: OEM Strings =====

// Type 11 - SMBIOS 2.0
struct Type11_OEMStrings_v2_0 {
    BYTE Count;
};

// ===== Type 12: System Configuration Options =====

// Type 12 - SMBIOS 2.0
struct Type12_ConfigurationOptions_v2_0 {
    BYTE Count;
};

// ===== Type 13: BIOS Language Information =====

// Type 13 - SMBIOS 2.0
struct Type13_BIOSLanguage_v2_0 {
    BYTE InstallableLanguages;
};

// ===== Type 14: Group Associations =====

// Type 14 - SMBIOS 2.0
struct Type14_GroupAssociations_v2_0 {
    BYTE GroupName;                   // String
    BYTE ItemType;
    WORD ItemHandle;
};

// ===== Type 16: Physical Memory Array (根據版本分類) =====

// Type 16 - SMBIOS 2.1
struct Type16_PhysicalMemoryArray_v2_1 {
    BYTE Location;
    BYTE Use;
    BYTE MemoryErrorCorrection;
    DWORD MaximumCapacity;            // KB
    WORD MemoryErrorInformationHandle;
    WORD NumberOfMemoryDevices;
};

// Type 16 - SMBIOS 2.7 (新增: 64位容量)
struct Type16_PhysicalMemoryArray_v2_7 {
    BYTE Location;
    BYTE Use;
    BYTE MemoryErrorCorrection;
    DWORD MaximumCapacity;
    WORD MemoryErrorInformationHandle;
    WORD NumberOfMemoryDevices;
    QWORD MaximumCapacity2;           // Bytes
};

// ===== Type 17: Memory Device (根據版本分類) =====

// Type 17 - SMBIOS 2.1
struct Type17_MemoryDevice_v2_1 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;               // String
    BYTE BankLocator;                 // String
    BYTE MemoryType;
    WORD TypeDetail;
};

// Type 17 - SMBIOS 2.3 (新增: Speed, Manufacturer, SerialNumber, AssetTag, PartNumber)
struct Type17_MemoryDevice_v2_3 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;                // String
    BYTE SerialNumber;                // String
    BYTE AssetTag;                    // String
    BYTE PartNumber;                  // String
};

// Type 17 - SMBIOS 2.6 (新增: Attributes)
struct Type17_MemoryDevice_v2_6 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE Attributes;
};

// Type 17 - SMBIOS 2.7 (新增: ExtendedSize, ConfiguredMemorySpeed)
struct Type17_MemoryDevice_v2_7 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE Attributes;
    DWORD ExtendedSize;               // MB
    WORD ConfiguredMemorySpeed;
};

// Type 17 - SMBIOS 2.8 (新增: 電壓資訊)
struct Type17_MemoryDevice_v2_8 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE Attributes;
    DWORD ExtendedSize;
    WORD ConfiguredMemorySpeed;
    WORD MinimumVoltage;
    WORD MaximumVoltage;
    WORD ConfiguredVoltage;
};

// Type 17 - SMBIOS 3.2 (新增: 內存技術、固件版本等)
struct Type17_MemoryDevice_v3_2 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE Attributes;
    DWORD ExtendedSize;
    WORD ConfiguredMemorySpeed;
    WORD MinimumVoltage;
    WORD MaximumVoltage;
    WORD ConfiguredVoltage;
    BYTE MemoryTechnology;
    WORD MemoryOperatingModeCapability;
    BYTE FirmwareVersion;             // String
    WORD ModuleManufacturerID;
    WORD ModuleProductID;
    WORD MemorySubsystemControllerManufacturerID;
    WORD MemorySubsystemControllerProductID;
    QWORD NonvolatileSize;
    QWORD VolatileSize;
    QWORD CacheSize;
    QWORD LogicalSize;
};

// Type 17 - SMBIOS 3.3 (新增: 擴展速度)
struct Type17_MemoryDevice_v3_3 {
    WORD PhysicalMemoryArrayHandle;
    WORD MemoryErrorInformationHandle;
    WORD TotalWidth;
    WORD DataWidth;
    WORD Size;
    BYTE FormFactor;
    BYTE DeviceSet;
    BYTE DeviceLocator;
    BYTE BankLocator;
    BYTE MemoryType;
    WORD TypeDetail;
    WORD Speed;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTag;
    BYTE PartNumber;
    BYTE Attributes;
    DWORD ExtendedSize;
    WORD ConfiguredMemorySpeed;
    WORD MinimumVoltage;
    WORD MaximumVoltage;
    WORD ConfiguredVoltage;
    BYTE MemoryTechnology;
    WORD MemoryOperatingModeCapability;
    BYTE FirmwareVersion;
    WORD ModuleManufacturerID;
    WORD ModuleProductID;
    WORD MemorySubsystemControllerManufacturerID;
    WORD MemorySubsystemControllerProductID;
    QWORD NonvolatileSize;
    QWORD VolatileSize;
    QWORD CacheSize;
    QWORD LogicalSize;
    DWORD ExtendedSpeed;
};

// ===== Type 19: Memory Array Mapped Address (根據版本分類) =====

// Type 19 - SMBIOS 2.1
struct Type19_MemoryArrayMappedAddress_v2_1 {
    DWORD StartingAddress;            // KB
    DWORD EndingAddress;              // KB
    WORD MemoryArrayHandle;
    BYTE PartitionWidth;
};

// Type 19 - SMBIOS 2.7 (新增: 64位地址)
struct Type19_MemoryArrayMappedAddress_v2_7 {
    DWORD StartingAddress;
    DWORD EndingAddress;
    WORD MemoryArrayHandle;
    BYTE PartitionWidth;
    QWORD StartingAddressExtended;    // Bytes
    QWORD EndingAddressExtended;      // Bytes
};

// ===== Type 20: Memory Device Mapped Address (根據版本分類) =====

// Type 20 - SMBIOS 2.1
struct Type20_MemoryDeviceMappedAddress_v2_1 {
    DWORD StartingAddress;            // KB
    DWORD EndingAddress;              // KB
    WORD MemoryDeviceHandle;
    WORD MemoryArrayMappedAddressHandle;
    BYTE PartitionRowPosition;
    BYTE InterleavePosition;
    BYTE InterleavedDataDepth;
};

// Type 20 - SMBIOS 2.7 (新增: 64位地址)
struct Type20_MemoryDeviceMappedAddress_v2_7 {
    DWORD StartingAddress;
    DWORD EndingAddress;
    WORD MemoryDeviceHandle;
    WORD MemoryArrayMappedAddressHandle;
    BYTE PartitionRowPosition;
    BYTE InterleavePosition;
    BYTE InterleavedDataDepth;
    QWORD StartingAddressExtended;
    QWORD EndingAddressExtended;
};

// ===== Type 26: Voltage Probe =====

// Type 26 - SMBIOS 2.2
struct Type26_VoltageProbe_v2_2 {
    BYTE Description;                 // String
    BYTE LocationAndStatus;
    WORD MaximumValue;                // mV
    WORD MinimumValue;                // mV
    WORD Resolution;
    WORD Tolerance;
    WORD Accuracy;
    DWORD OEMSpecific;
    WORD NominalValue;
};

// ===== Type 27: Cooling Device (根據版本分類) =====

// Type 27 - SMBIOS 2.2
struct Type27_CoolingDevice_v2_2 {
    WORD TemperatureProbeHandle;
    BYTE DeviceTypeAndStatus;
    BYTE CoolingUnitGroup;
    DWORD OEMSpecific;
    WORD NominalSpeed;
};

// Type 27 - SMBIOS 2.7 (新增: Description)
struct Type27_CoolingDevice_v2_7 {
    WORD TemperatureProbeHandle;
    BYTE DeviceTypeAndStatus;
    BYTE CoolingUnitGroup;
    DWORD OEMSpecific;
    WORD NominalSpeed;
    BYTE Description;                 // String
};

// ===== Type 28: Temperature Probe (根據版本分類) =====

// Type 28 - SMBIOS 2.2
struct Type28_TemperatureProbe_v2_2 {
    BYTE Description;                 // String
    BYTE LocationAndStatus;
    WORD MaximumTemperature;          // 1/10°C
    WORD MinimumTemperature;          // 1/10°C
    WORD Resolution;
    WORD Tolerance;
    WORD Accuracy;
    DWORD OEMSpecific;
    WORD NominalTemperature;
};

// Type 28 - SMBIOS 3.2 (新增: 擴展溫度)
struct Type28_TemperatureProbe_v3_2 {
    BYTE Description;
    BYTE LocationAndStatus;
    WORD MaximumTemperature;
    WORD MinimumTemperature;
    WORD Resolution;
    WORD Tolerance;
    WORD Accuracy;
    DWORD OEMSpecific;
    WORD NominalTemperature;
    DWORD MaximumTemperatureExtended;
    DWORD MinimumTemperatureExtended;
};

// ===== Type 29: Electrical Current Probe =====

// Type 29 - SMBIOS 2.2
struct Type29_ElectricalCurrentProbe_v2_2 {
    BYTE Description;                 // String
    BYTE LocationAndStatus;
    WORD MaximumValue;                // mA
    WORD MinimumValue;                // mA
    WORD Resolution;
    WORD Tolerance;
    WORD Accuracy;
    DWORD OEMSpecific;
    WORD NominalValue;
};

// ===== Type 32: System Boot Information =====

// Type 32 - SMBIOS 2.0
struct Type32_SystemBoot_v2_0 {
    BYTE Reserved[6];
    BYTE BootStatus;
};

// ===== Type 34: Management Device =====

// Type 34 - SMBIOS 2.3
struct Type34_ManagementDevice_v2_3 {
    BYTE Description;                 // String
    BYTE DeviceType;
    DWORD Address;
    BYTE AddressType;
    BYTE ControllerHandle;
};

// ===== Type 39: Power Supply (根據版本分類) =====

// Type 39 - SMBIOS 2.5
struct Type39_PowerSupply_v2_5 {
    BYTE PowerUnitGroup;
    BYTE Location;                    // String
    BYTE DeviceName;                  // String
    BYTE Manufacturer;                // String
    BYTE SerialNumber;                // String
    BYTE AssetTagNumber;              // String
    BYTE ModelPartNumber;             // String
    BYTE RevisionLevel;               // String
    WORD MaxPowerCapacity;            // W
    WORD PowerSupplyCharacteristics;
    WORD InputVoltageProbeHandle;
    WORD CoolingDeviceHandle;
    WORD InputCurrentProbeHandle;
};

// Type 39 - SMBIOS 3.0 (新增: 64位容量)
struct Type39_PowerSupply_v3_0 {
    BYTE PowerUnitGroup;
    BYTE Location;
    BYTE DeviceName;
    BYTE Manufacturer;
    BYTE SerialNumber;
    BYTE AssetTagNumber;
    BYTE ModelPartNumber;
    BYTE RevisionLevel;
    WORD MaxPowerCapacity;
    WORD PowerSupplyCharacteristics;
    WORD InputVoltageProbeHandle;
    WORD CoolingDeviceHandle;
    WORD InputCurrentProbeHandle;
    DWORD MaxPowerCapacity2;          // W
};

// ===== Type 41: Onboard Devices Extended Information =====

// Type 41 - SMBIOS 2.6
struct Type41_OnboardDevicesExtended_v2_6 {
    BYTE ReferenceDesignation;        // String
    BYTE DeviceType;
    BYTE DeviceTypeInstance;
    WORD SegmentGroupNumber;
    BYTE BusNumber;
    BYTE DeviceFunctionNumber;
};

// ===== Type 43: TPM Device =====

// Type 43 - SMBIOS 2.5
struct Type43_TPM_v2_5 {
    BYTE VendorIDDescription;         // String
    BYTE VendorID[4];
    BYTE FirmwareVersion[4];
    BYTE DescriptionString;           // String
    QWORD Characteristics;
    DWORD OEMSpecific;
};

// ===== Type 44: Processor Additional Information =====

// Type 44 - SMBIOS 2.7
struct Type44_ProcessorAdditionalInfo_v2_7 {
    BYTE ReferencedHandle;            // Type 4 Handle
    BYTE ProcessorSpecificBlock;
};

// ===== Type 127: End-of-Table =====

// Type 127 - SMBIOS 2.0
struct Type127_EndOfTable_v2_0 {
    // 無數據字段，只是標記結束
};

int main()
{
    DWORD signature = 'RSMB'; // 也可以寫成 0x52534D42 ('B' 'M' 'S' 'R')
    DWORD size = EnumSystemFirmwareTables(signature, NULL, 0);
    std::vector<DWORD> tableIds(size / sizeof(DWORD));
    EnumSystemFirmwareTables(signature, tableIds.data(), size);


    UINT bufSize = GetSystemFirmwareTable(signature, 0, NULL, 0);

    if (bufSize == 0) {
        std::cerr << "無法取得 SMBIOS 資料長度，錯誤代碼: " << GetLastError() << std::endl;
        return 1;
    }

    std::vector<BYTE> buffer(bufSize);
    UINT bytesWritten = GetSystemFirmwareTable(signature, 0, buffer.data(), bufSize);
    if (bytesWritten == 0) {
        std::cerr << "複製 SMBIOS 資料失敗，錯誤代碼: " << GetLastError() << std::endl;
        return 1;
    }
#pragma pack(push, 1)
    struct RawSMBIOSData
    {
        BYTE    Used20CallingMethod;
        BYTE    SMBIOSMajorVersion;
        BYTE    SMBIOSMinorVersion;
        BYTE    DmiRevision;
        DWORD   Length;
        BYTE SMBIOSTableData[1];
    };

    struct SMBIOSHeader {
        BYTE Type;
        BYTE Length;
        WORD Handle;
    };
#pragma pack(pop)
	auto rawbios = (RawSMBIOSData*)buffer.data();
    auto span_raw = std::span(buffer).subspan(8);
    
    //std::map<byte, int> types;
    std::map<byte, std::vector<std::span<BYTE>>> types1;
    int allcount = 0;
    int bb = 0;
    for (auto i = 0; i < span_raw.size(); i++)
    {
        bb = i;
        allcount = allcount + 1;
        auto type = span_raw[i];
        //types[type]++;
        auto len = span_raw[i+1];
        printf("Type: %d, Length: %d\n", type, len);
        i = i + len;

        for (int j = i; j < span_raw.size(); j++)
        {
            auto s1 = span_raw[j];
            auto s2 = span_raw[j + 1];
            if (s1 == 0 && s2 == 0)
            {
                auto aa = span_raw.subspan(bb, j-bb);
                types1[type].push_back(aa);

                i = j + 1;
                break;
            }
        }

    }
    for (auto i = types1.begin(); i != types1.end(); i++)
    {
        printf("Type: %d, count: %I64d\n", i->first, i->second.size());
    }
     printf("\n");
     return 0;
}
