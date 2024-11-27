using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        [Obsolete("Obsoleted, please use Service")]
        public static string GetService(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_SERVICE);
        [Obsolete("Obsoleted, please use Manufacturer")]
        public static string GetMFG(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(SPDRP_MFG);
    }
}
