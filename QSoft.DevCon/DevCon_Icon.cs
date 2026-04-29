using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtension;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace QSoft.DevCon.WPF
{
    public static partial class DevConExtension
    {
        //public static System.Windows.Media.Imaging.BitmapSource? Icon(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        //{
        //    if(SetupDiLoadDeviceIcon(src.dev, ref src.devdata, 96, 96, 0, out var iconptr))
        //    {
        //        try
        //        {
        //            var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(iconptr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        //            return icon;
        //        }
        //        finally
        //        {
        //            DestroyIcon(iconptr);
        //        }
        //    }

        //    return null;
        //}

        public static SafeIconHandle? Icon(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            if (SetupDiLoadDeviceIcon(src.dev, ref src.devdata, 96, 96, 0, out var iconptr))
            {
                return new SafeIconHandle(iconptr);
            }
            return null;
        }

    }

    public class SafeIconHandle : SafeHandle
    {
        public SafeIconHandle() : base(IntPtr.Zero, true) { }
        public SafeIconHandle(IntPtr handle) : base(IntPtr.Zero, true)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            return DestroyIcon(handle);
        }
    }


}
