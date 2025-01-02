using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static List<string> Childrens(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(DEVPKEY_Device_Children);

        public static string Parent(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_Parent);
        public static List<string> Siblings(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(DEVPKEY_Device_Siblings);
        public static string PowerRelations(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetString(DEVPKEY_Device_PowerRelations);
        public static List<string> BusRelations(this (IntPtr dev, SP_DEVINFO_DATA devdata) src)
            => src.GetStrings(DEVPKEY_Device_BusRelations);

    }
}
