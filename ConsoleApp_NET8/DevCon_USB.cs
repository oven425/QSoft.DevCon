using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(
        SafeFileHandle hDevice,
        uint dwIoControlCode,
        // 因為是同一個 buffer，可以標記為 In/Out
        // 為了簡單起見，我們在呼叫端傳入同一個 span 兩次
        ReadOnlySpan<byte> lpInBuffer,
        uint nInBufferSize,
        Span<byte> lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USB_HCD_DRIVERKEY_NAME
        {
            public uint ActualLength;
            public char DriverKeyName;
        }

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
