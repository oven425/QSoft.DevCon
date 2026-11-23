using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    public static partial class DevCon_FSCTL
    {
        
        public static void Test(this SafeFileHandle src)
        {
            var FSCTL_ENUM_USN_DATA = 590003;
            DeviceIoControl(
    src,
    FSCTL_ENUM_USN_DATA,
    ref med,
    Marshal.SizeOf<MFT_ENUM_DATA_V0>(),
    buffer,
    buffer.Length,
    out var bytesReturned,
    IntPtr.Zero);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MFT_ENUM_DATA_V0
        {
            public ulong StartFileReferenceNumber;
            public long LowUsn;
            public long HighUsn;
        }

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
