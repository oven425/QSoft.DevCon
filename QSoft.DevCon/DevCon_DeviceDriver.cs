using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static string GetDriverVersion(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_DriverVersion);

        public static string DriverInfSection(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_DriverInfSection);

        public static DateTime GetDriverDate(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetDateTime(DEVPKEY_Device_DriverDate);

        public static string DriverProvider(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_DriverProvider);

    }
}
