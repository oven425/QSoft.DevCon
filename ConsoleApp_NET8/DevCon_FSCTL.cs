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
        public static void READ_USN_JOURNAL(this SafeFileHandle src, (ulong journalid, long startusn) data)
        {
            var usn_read_jornal = new READ_USN_JOURNAL_DATA_V0()
            {
                StartUsn = data.startusn,

                // 全部事件
                ReasonMask = 0xFFFFFFFF,

                ReturnOnlyOnClose = 0,

                Timeout = 0,

                BytesToWaitFor = 0,

                UsnJournalID = data.journalid
            };
            var usn_read_jornal_buf = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref usn_read_jornal, 1));
            Span<byte> buffer = stackalloc byte[1024];
            var hr = DeviceIoControl(src, FSCTL_READ_USN_JOURNAL, usn_read_jornal_buf, (uint)usn_read_jornal_buf.Length, buffer, (uint)buffer.Length, out var bytesReturned, IntPtr.Zero);
            if(hr)
            {
                buffer = buffer[..(int)bytesReturned];
                var startusn = MemoryMarshal.Read<long>(buffer);
                buffer = buffer[sizeof(long) ..];
                while (!buffer.IsEmpty)
                {
                    var rr = MemoryMarshal.AsRef<USN_RECORD_V2>(buffer);
                    var name = MemoryMarshal.Cast<byte, char>( buffer.Slice(rr.FileNameOffset, rr.FileNameLength)).ToString();
                    buffer = buffer[(int)rr.RecordLength ..];
                }


            }
            
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct READ_USN_JOURNAL_DATA_V0
        {
            public long StartUsn;
            public uint ReasonMask;
            public uint ReturnOnlyOnClose;
            public ulong Timeout;
            public ulong BytesToWaitFor;
            public ulong UsnJournalID;
        }

        public static (ulong journalid, long startusn) QUERY_USN_JOURNAL(this SafeFileHandle src)
        {
            Span<byte> buffer = stackalloc byte[512];
            var hr = DeviceIoControl(src, FSCTL_QUERY_USN_JOURNAL, [], 0, buffer, (uint)buffer.Length, out var bytesReturned, IntPtr.Zero);
            var ss = MemoryMarshal.AsRef<USN_JOURNAL_DATA_V0>(buffer);
            return (ss.UsnJournalID, ss.FirstUsn);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USN_JOURNAL_DATA_V0
        {
            public ulong UsnJournalID;
            public long FirstUsn;
            public long NextUsn;
            public long LowestValidUsn;
            public long MaxUsn;
            public ulong MaximumSize;
            public ulong AllocationDelta;
        }


        public static void ENUM_USN_DATA(this SafeFileHandle src)
        {
            ulong nextfrn = 0;
            
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
                    buffer = buffer[sizeof(long)..];
                    while (!buffer.IsEmpty)
                    {
                        var v2 = MemoryMarshal.AsRef<USN_RECORD_V2>(buffer);
                        var ss = buffer.Slice(v2.FileNameOffset, v2.FileNameLength);
                        var filename = MemoryMarshal.Cast<byte, char>(ss).ToString();
                        rrs.Add((v2.FileReferenceNumber, v2.ParentFileReferenceNumber, filename));
                        buffer = buffer[(int)v2.RecordLength..];
                    }

                }
            }

            var selfdic = rrs.ToDictionary(x => x.self);
            Stack<string> sss = [];
            foreach(var (self, parent, name) in rrs)
            {
                sss.Clear();
                var usn_parent = parent;
                sss.Push(name);
                while (true)
                {
                    
                    if (selfdic.TryGetValue(usn_parent, out var selfnode))
                    {
                        sss.Push(selfnode.name);
                        if(selfnode.self == selfnode.parent)
                        {
                            break;
                        }
                        usn_parent = selfnode.parent;
                    }
                    else
                    {
                        break;
                    }
                }
                
                var fullname = $"C:\\{string.Join("\\", [.. sss])}";
                System.Diagnostics.Trace.WriteLine(fullname);
            }


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
            public UsnReason Reason;
            uint SourceInfo;
            uint SecurityId;
            public System.IO.FileAttributes FileAttributes;
            public ushort FileNameLength;
            public ushort FileNameOffset;
            //WCHAR FileName[1];

        }

        [Flags]
        public enum UsnReason : uint
        {
            None = 0,

            // Data
            DataOverwrite = 0x00000001,
            DataExtend = 0x00000002,
            DataTruncation = 0x00000004,

            // Named stream
            NamedDataOverwrite = 0x00000010,
            NamedDataExtend = 0x00000020,
            NamedDataTruncation = 0x00000040,

            // File lifecycle
            FileCreate = 0x00000100,
            FileDelete = 0x00000200,

            // Metadata
            EaChange = 0x00000400,
            SecurityChange = 0x00000800,

            // Rename / Move
            RenameOldName = 0x00001000,
            RenameNewName = 0x00002000,

            // Attributes
            IndexableChange = 0x00004000,
            BasicInfoChange = 0x00008000,

            // Links / Compression / Encryption
            HardLinkChange = 0x00010000,
            CompressionChange = 0x00020000,
            EncryptionChange = 0x00040000,

            // Object Id / Reparse
            ObjectIdChange = 0x00080000,
            ReparsePointChange = 0x00100000,

            // TxF (已淘汰但仍可能出現在 enum 中)
            TransactionChange = 0x00200000,

            // Integrity (ReFS)
            IntegrityChange = 0x00800000,

            // Desired Storage Class
            DesiredStorageClassChange = 0x01000000,

            // Close
            Close = 0x80000000,
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
        const uint FSCTL_ENUM_USN_DATA = 590003;
        const uint FSCTL_QUERY_USN_JOURNAL = 590068;
        const uint FSCTL_READ_USN_JOURNAL = 590011;

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    }
}
