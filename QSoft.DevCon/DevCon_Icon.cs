﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using static QSoft.DevCon.DevConExtension;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace QSoft.DevCon.WPF
{
    public static partial class DevConExtension
    {
        public static System.Windows.Media.Imaging.BitmapSource Icon(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            SetupDiLoadDeviceIcon(src.dev, ref src.devdata, 96, 96, 0, out var iconptr);
            var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(iconptr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DestroyIcon(iconptr);
            
            return icon;
        }

    }

    
}
