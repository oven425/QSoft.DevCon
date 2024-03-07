using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClassLibrary1
{
    static public partial class Class1
    {
        public static Guid[] GetDevClass(this string src)
        {
            Guid[]? GuidArray = null;
            // read Guids
            bool Status = SetupDiClassGuidsFromName(src, IntPtr.Zero, 0, out var RequiredSize);
            GuidArray = new Guid[RequiredSize];
            Status = SetupDiClassGuidsFromName(src, ref GuidArray[0], RequiredSize, out var RequiredSize1);
            if (Status)
            {
                //if (1 < RequiredSize)
                //{
                //    GuidArray = new Guid[RequiredSize];
                //    SetupDiClassGuidsFromName("class name here", ref GuidArray[0], RequiredSize, out RequiredSize);
                //}
            }
            else
            {
                
            }
            var ErrorCode = Marshal.GetLastWin32Error();
            return GuidArray;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public UIntPtr Reserved;
        }
#if NET8_0_OR_GREATER
        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInstanceIdW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, string? DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);

        [LibraryImport("setupapi.dll",EntryPoint = "SetupDiClassGuidsFromNameW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

        [LibraryImport("setupapi.dll", EntryPoint = "SetupDiClassGuidsFromNameW", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetupDiClassGuidsFromName(string ClassName,  IntPtr guids, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);


#else
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);
        
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiClassGuidsFromName(string ClassName, IntPtr guids, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder? DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);

#endif

    }
}
