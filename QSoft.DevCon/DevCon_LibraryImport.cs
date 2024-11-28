using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsW")]
        internal static partial IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);
        //[LibraryImport("setupapi.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //internal static partial bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, Guid InterfaceClassGuid, uint MemberIndex, out SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);
        //[LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInterfaceDetailW", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //internal static partial bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, out SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, uint DeviceInterfaceDetailDataSize, out uint RequiredSize, out SP_DEVINFO_DATA DeviceInfoData);


        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInstanceIdW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryPropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiSetDeviceRegistryPropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, IntPtr PropertyBuffer, uint PropertyBufferSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiClassGuidsFromNameW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiClassGuidsFromName([MarshalAs(UnmanagedType.LPWStr)] string ClassName, IntPtr ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetClassDescriptionW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetClassDescription(Guid ClassGuid, IntPtr ClassDescription, uint ClassDescriptionSize, out uint RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiGetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, UInt32 flags);


        [LibraryImport("kernel32.dll", EntryPoint = "QueryDosDeviceW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static partial uint QueryDosDevice([MarshalAs(UnmanagedType.LPWStr)] string lpDeviceName, IntPtr lpTargetPath, int ucchMax);


        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiSetClassInstallParamsW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);


        [LibraryImport("Setupapi", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static partial IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope, uint hwProfile, uint parameterRegistryValueKind, int samDesired);

        [LibraryImport("kernel32.dll", EntryPoint = "FormatMessageW")]
        [return: MarshalAs(UnmanagedType.I4)]
        static internal partial int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource,
                                 int dwMessageId, int dwLanguageZId,
                                 ref IntPtr lpBuffer, int nSize, IntPtr Arguments);

        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static internal partial bool SetupDiGetDevicePropertyKeys(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, IntPtr PropertyKeyArray, uint PropertyKeyCount, out uint RequiredPropertyKeyCount, uint Flags);


    }
}

#endif
