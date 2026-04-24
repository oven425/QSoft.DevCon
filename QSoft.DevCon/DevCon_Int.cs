using System;
using System.Runtime.InteropServices;

namespace QSoft.DevCon
{
    public readonly ref struct Query<T>(T value, int errorcode)
    {
        public int ErrorCode { get; } = errorcode;
        public T Value { get; } = value;
        public readonly T ThrowIfError()
        {
            if (ErrorCode != 0)
            {
                throw new System.ComponentModel.Win32Exception(ErrorCode);
            }
            return Value;
        }
        public bool IsSuccess => ErrorCode == 0;
        public bool IsError => ErrorCode != 0;
    }
    
    static public partial class DevConExtension
    {
        public static Query<int> GetInt32_(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = 0;
            int errorcode = 0;
#if NET8_0_OR_GREATER
            SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out var property_type, [], 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                Span<byte> mem = stackalloc byte[reqsize];
                SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out property_type, mem, reqsize, out reqsize, 0);
                errorcode = Marshal.GetLastWin32Error();
                str = MemoryMarshal.Read<int>(mem);
            }
#else
            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                errorcode = Marshal.GetLastWin32Error();
                str = Marshal.ReadInt32(mem.Pointer);
            }
#endif
            return new Query<int>(str, errorcode);
        }

        static int GetInt32(this (IntPtr dev, SP_DEVINFO_DATA devdata) src, DEVPROPKEY devkey)
        {
            var str = 0;
#if NET8_0_OR_GREATER
            SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out var property_type, [], 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                Span<byte> mem = stackalloc byte[reqsize];
                SetupDiGetDeviceProperty(src.dev, src.devdata, devkey, out property_type, mem, reqsize, out reqsize, 0);
                str = MemoryMarshal.Read<int>(mem);
            }
#else


            SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out var property_type, IntPtr.Zero, 0, out var reqsize, 0);
            if (reqsize > 0)
            {
                using var mem = new IntPtrMem<byte>(reqsize);
                SetupDiGetDeviceProperty(src.dev, ref src.devdata, ref devkey, out property_type, mem.Pointer, reqsize, out reqsize, 0);
                str = Marshal.ReadInt32(mem.Pointer);
            }
#endif
            return str;
        }
    }
}
