using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace QSoft.DevCon
{
    public static partial class DevConExtension
    {
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static string GetComPortName(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
        {
            var hKey1 = SetupDiOpenDevRegKey(src.dev, ref src.devdata, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            using var hKey = new SafeRegistryHandle(hKey1, true);

            if (!hKey.IsInvalid)
            {
                using var reg = RegistryKey.FromHandle(hKey);
                var portname = reg?.GetValue("PortName")?.ToString();
                return portname ?? "";
            }

            return "";
        }

    }
}
