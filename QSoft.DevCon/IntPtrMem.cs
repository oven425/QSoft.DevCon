using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NET8_0_OR_GREATER

#else
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
            m_pBuffer = Marshal.AllocHGlobal(Size);
            DevConExtension.ZeroMemory(m_pBuffer, Size);
        }

        ~IntPtrMem()
        {
            Dispose();
        }

        public static implicit operator IntPtr(IntPtrMem<T> h) => h.Pointer;


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


}
#endif