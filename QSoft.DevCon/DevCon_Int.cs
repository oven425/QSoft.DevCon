using System;
using System.Runtime.InteropServices;

namespace QSoft.DevCon
{
    //public ref struct QueryInt(int value, int errorcode)
    //{
    //    public int ErrorCode { private set; get; } = errorcode;
    //    public int Value { get; private set; } = value;
    //    public static implicit operator int(QueryInt q) => q.ThrowIfError();

    //    public readonly int ThrowIfError()
    //    {
    //        if (ErrorCode != 0)
    //        {
    //            throw new System.ComponentModel.Win32Exception(ErrorCode);
    //        }
    //        return Value;
    //    }
    //}

    public readonly struct Query<T>(T value)
    {
        public int ErrorCode { get; } = Marshal.GetLastWin32Error();
        public T Value { get; } = value;
        public static implicit operator T(in Query<T> q) => q.Value;
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
            return new Query<int>(str);
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
