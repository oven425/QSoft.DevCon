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
            ulong nextfrn = 0;
            uint FSCTL_ENUM_USN_DATA = 590003;
            var med = new MFT_ENUM_DATA_V0
            {
                StartFileReferenceNumber = nextfrn,
                HighUsn = long.MaxValue
            };
            var buf = new byte[1024*1024];
            List<(ulong self, ulong parent, string name)> rrs = [];
            while(true)
            {
                var meds = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref med, 1));
                Array.Clear(buf);
                Span<byte> buffer = buf.AsSpan();
                var hr = DeviceIoControl(src, FSCTL_ENUM_USN_DATA, meds, (uint)Marshal.SizeOf<MFT_ENUM_DATA_V0>(), buffer, (uint)buffer.Length, out var bytesReturned, IntPtr.Zero);
                var err = Marshal.GetLastWin32Error();
                if(!hr)
                {
                    break;
                }
                if (hr)
                {
                    buffer = buffer[..(int)bytesReturned];
                    med.StartFileReferenceNumber = MemoryMarshal.Read<ulong>(buffer);
                    System.Diagnostics.Trace.WriteLine($"nextfrn:{med.StartFileReferenceNumber}");
                    buffer = buffer[sizeof(long)..];
                    while (!buffer.IsEmpty)
                    {
                        var v2 = MemoryMarshal.AsRef<USN_RECORD_V2>(buffer);
                        var ss = buffer.Slice(v2.FileNameOffset, v2.FileNameLength);
                        var filename = MemoryMarshal.Cast<byte, char>(ss).ToString();
                        //System.Diagnostics.Trace.WriteLine(filename);
                        rrs.Add((v2.FileReferenceNumber, v2.ParentFileReferenceNumber, filename));
                        buffer = buffer[(int)v2.RecordLength..];
                    }

                }
            }

            var selfdic = rrs.ToDictionary(x => x.self);
            foreach(var (self, parent, name) in rrs)
            {
                if(selfdic.TryGetValue(parent, out (ulong self, ulong parent, string name) cccc))
                {
                }
            }


        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UsnRecordCommon
        {
            public uint RecordLength;
            public ushort MajorVersion;
            public ushort MinorVersion;
        }

        struct USN_RECORD_V2
        {

            public uint RecordLength;
            ushort MajorVersion;
            ushort MinorVersion;
            public ulong FileReferenceNumber;
            public ulong ParentFileReferenceNumber;
            long Usn;
            LARGE_INTEGER TimeStamp;
            uint Reason;
            uint SourceInfo;
            uint SecurityId;
            uint FileAttributes;
            public ushort FileNameLength;
            public ushort FileNameOffset;
            //WCHAR FileName[1];

        }
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct LARGE_INTEGER
        {
            [FieldOffset(0)] public uint LowPart;   // DWORD
            [FieldOffset(4)] public int HighPart;   // LONG
            [FieldOffset(0)] public long QuadPart;  // LONGLONG
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
