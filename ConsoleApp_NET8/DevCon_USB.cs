using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    static public partial class DevConExtension
    {
        public static string GetHCDDriverKeyName(this SafeFileHandle src)
        {

            return "";
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
