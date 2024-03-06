using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClassLibrary1
{
    public partial class Class1
    {
        public string? AA()
        {
            return null;
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
        [LibraryImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder? DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);
#else
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder? DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);

#endif

    }
}
