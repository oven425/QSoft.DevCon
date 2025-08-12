using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtensiona;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QSoft.DevCon
{
    static public partial class DevConExtensiona
    {
        public static string GetHCDDriverKeyName(this SafeFileHandle src)
        {
            Span<byte> buffer = stackalloc byte[6];
            var b1 = DeviceIoControl(src, 0x00220424, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, out var ss, IntPtr.Zero);
            var ii = BitConverter.ToUInt32(buffer);
            buffer = stackalloc byte[(int)ii];
            b1 = DeviceIoControl(src, 0x00220424, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, out ss, IntPtr.Zero);

            var vvv = buffer[4..^4];
            var ccs = MemoryMarshal.Cast<byte, char>(vvv);
            var str = new string(ccs);
            //{36fc9e60-c465-11cf-8056-444553540000}\0000
            var err = Marshal.GetLastWin32Error();
            return str;
        }
        static uint IOCTL_USB_GET_NODE_INFORMATION = 0x00220408;
        static uint IOCTL_USB_GET_ROOT_HUB_NAME = 0x00220408;
        public static string GetRootHubName(this SafeFileHandle src)
        {
            var myStruct = new USB_HCD_DRIVERKEY_NAME();
            var structSpan = MemoryMarshal.CreateSpan(ref myStruct, 1);
            var buffer = MemoryMarshal.AsBytes(structSpan);

            var success = DeviceIoControl(src, IOCTL_USB_GET_ROOT_HUB_NAME, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out var nBytes, IntPtr.Zero);

            buffer = stackalloc byte[(int)myStruct.ActualLength];
            success = DeviceIoControl(src, IOCTL_USB_GET_ROOT_HUB_NAME, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out nBytes, IntPtr.Zero);
            var vvv = buffer[4..^2];
            var ccs = MemoryMarshal.Cast<byte, char>(vvv);
            var str = new string(ccs);



            return str;
        }

        public static void  GetGET_NODE_INFORMATION(this SafeFileHandle src)
        {


            var sz = Marshal.SizeOf<USB_HUB_INFORMATION_EX>();
            var myStruct = new USB_HCD_DRIVERKEY_NAME();
            var structSpan = MemoryMarshal.CreateSpan(ref myStruct, 1);
            var buffer = MemoryMarshal.AsBytes(structSpan);

            var success = DeviceIoControl(src, IOCTL_USB_GET_ROOT_HUB_NAME, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out var nBytes, IntPtr.Zero);



        }


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);




        [StructLayout(LayoutKind.Sequential, Pack =1, CharSet = CharSet.Unicode)]
        public struct USB_HCD_DRIVERKEY_NAME
        {
            public uint ActualLength;
            public char DriverKeyName;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct USB_HUB_DESCRIPTOR
        {
            public byte bDescriptorLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            public ushort wHubCharacteristics;
            public byte bPowerOnToPowerGood;
            public byte bHubControlCurrent;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            //public byte[] DeviceRemovable;
            public fixed byte DeviceRemovable[64];
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_30_HUB_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            public ushort wHubCharacteristics;
            public byte bPowerOnToPowerGood;
            public byte bHubControlCurrent;
            public byte bHubHdrDecLat;
            public ushort wHubDelay;
            public ushort DeviceRemovable;
        };

        public enum USB_HUB_TYPE
        {
            UsbRootHub = 1,
            Usb20Hub = 2,
            Usb30Hub = 3
        };
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct USB_HUB_INFORMATION_EX
        {
            [FieldOffset(0)]
            public USB_HUB_TYPE HubType;

            [FieldOffset(4)]
            public ushort HighestPortNumber;
            [FieldOffset(6)]
            public USB_HUB_DESCRIPTOR UsbHubDescriptor;
            [FieldOffset(6)]
            public USB_30_HUB_DESCRIPTOR Usb30HubDescriptor;

        };

    enum WDMUSB_POWER_STATE
        {

            WdmUsbPowerNotMapped = 0,

            WdmUsbPowerSystemUnspecified = 100,
            WdmUsbPowerSystemWorking,
            WdmUsbPowerSystemSleeping1,
            WdmUsbPowerSystemSleeping2,
            WdmUsbPowerSystemSleeping3,
            WdmUsbPowerSystemHibernate,
            WdmUsbPowerSystemShutdown,

            WdmUsbPowerDeviceUnspecified = 200,
            WdmUsbPowerDeviceD0,
            WdmUsbPowerDeviceD1,
            WdmUsbPowerDeviceD2,
            WdmUsbPowerDeviceD3

        };


        enum USB_USER_ERROR_CODE
        {
            UsbUserSuccess = 0,
            UsbUserNotSupported,
            UsbUserInvalidRequestCode,
            UsbUserFeatureDisabled,
            UsbUserInvalidHeaderParameter,
            UsbUserInvalidParameter,
            UsbUserMiniportError,
            UsbUserBufferTooSmall,
            UsbUserErrorNotMapped,
            UsbUserDeviceNotStarted,
            UsbUserNoDeviceConnected

        };


        struct USBUSER_REQUEST_HEADER
        {
            uint UsbUserRequest;
            /*
                status code returned by port driver
            */
            USB_USER_ERROR_CODE UsbUserStatusCode;
            /*
                size of client input/output buffer
                we always use the same buffer for input
                and output
            */
            uint RequestBufferLength;
            /*
                size of buffer required to get all of the data
            */
            uint ActualBufferLength;

        };

        struct USB_POWER_INFO
        {

            /* input */
            WDMUSB_POWER_STATE SystemState;
            /* output */
            WDMUSB_POWER_STATE HcDevicePowerState;
            WDMUSB_POWER_STATE HcDeviceWake;
            WDMUSB_POWER_STATE HcSystemWake;

            WDMUSB_POWER_STATE RhDevicePowerState;
            WDMUSB_POWER_STATE RhDeviceWake;
            WDMUSB_POWER_STATE RhSystemWake;

            WDMUSB_POWER_STATE LastSystemSleepState;

            bool CanWakeup;
            bool IsPowered;

        };

        struct USBUSER_POWER_INFO_REQUEST
        {

            USBUSER_REQUEST_HEADER Header;
            USB_POWER_INFO PowerInformation;

        };


        static uint IOCTL_GET_HCD_DRIVERKEY_NAME = CTL_CODE(
            FILE_DEVICE_USB, // 0x7
            HCD_GET_DRIVERKEY_NAME,                      // 功能碼
            METHOD_BUFFERED,  // 0
            FILE_ANY_ACCESS   // 0
        );


        static uint HCD_GET_DRIVERKEY_NAME = 265;

        static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method);
        }


        const uint METHOD_BUFFERED = 0;
        public const uint METHOD_IN_DIRECT = 1;
        public const uint METHOD_OUT_DIRECT = 2;
        public const uint METHOD_NEITHER = 3;

        public const uint FILE_ANY_ACCESS = 0;
        public const uint FILE_READ_ACCESS = 0x0001;
        public const uint FILE_WRITE_ACCESS = 0x0002;

        public const uint FILE_DEVICE_BEEP = 0x00000001;
        public const uint FILE_DEVICE_CD_ROM = 0x00000002;
        public const uint FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x00000003;
        public const uint FILE_DEVICE_CONTROLLER = 0x00000004;
        public const uint FILE_DEVICE_DATALINK = 0x00000005;
        public const uint FILE_DEVICE_DFS = 0x00000006;
        public const uint FILE_DEVICE_DISK = 0x00000007;
        public const uint FILE_DEVICE_DISK_FILE_SYSTEM = 0x00000008;
        public const uint FILE_DEVICE_FILE_SYSTEM = 0x00000009;
        public const uint FILE_DEVICE_KEYBOARD = 0x0000000b;
        public const uint FILE_DEVICE_MOUSE = 0x0000000c;
        public const uint FILE_DEVICE_NETWORK = 0x00000012;
        public const uint FILE_DEVICE_SERIAL_PORT = 0x0000001b;
        public const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        public const uint FILE_DEVICE_USB = 0x00000022; // 與 UNKNOWN 相同
        public const uint FILE_DEVICE_VIDEO = 0x00000023;
    }
}
