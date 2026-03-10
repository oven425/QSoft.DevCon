using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    internal sealed class IntPtrMem<T> : IDisposable where T : struct
    {
        public IntPtr Pointer
        {
            get
            {
                IntPtr pointer = m_pBuffer;
                if (pointer == IntPtr.Zero && Size != 0)
                {
                    throw new ObjectDisposedException("Pointer");
                }

                return pointer;
            }
        }
        public int Size { private set; get; } = 0;
        public IntPtrMem(int size)
        {

            var s1 = Marshal.SizeOf<T>();

            Size = s1 * size;
            //if (typeof(T) == typeof(char))
            //{
            //    Size = Size * 2;
            //}
            m_pBuffer = Marshal.AllocHGlobal(Size);
        }

        ~IntPtrMem()
        {
            Dispose();
        }

        public static implicit operator IntPtr(IntPtrMem<T> h) => h.Pointer;

        public IntPtrMem(IntPtr ptr)
        {
            m_pBuffer = ptr;
        }

        IntPtr m_pBuffer = IntPtr.Zero;
        public void Dispose()
        {
            IntPtr intPtr = Interlocked.Exchange(ref m_pBuffer, IntPtr.Zero);
            if (intPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(intPtr);
            }
            GC.SuppressFinalize(this);
        }
    }

    //internal sealed class IntPtrMem1<T> : SafeHandle where T : struct
    //{
    //    public int Size { private set; get; } = 0;
    //    public IntPtr Pointer => handle;
    //    public override bool IsInvalid => handle == IntPtr.Zero;

    //    public IntPtrMem1(int size) : base(IntPtr.Zero, true)
    //    {

    //        Size = Marshal.SizeOf<T>() * size;
    //        if (Size > 0)
    //        {
    //            SetHandle(Marshal.AllocHGlobal(Size));
    //        }
    //    }



    //    protected override bool ReleaseHandle()
    //    {
    //        if (handle != IntPtr.Zero)
    //        {
    //            Marshal.FreeHGlobal(handle);
    //            return true;
    //        }
    //        return false;
    //    }

    //    public static implicit operator IntPtr(IntPtrMem1<T> h) => h.handle;
    //}

}
