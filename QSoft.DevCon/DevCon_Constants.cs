using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        readonly public static Guid GUID_DEVINTERFACE_I2C = new("2564AA4F-DDDB-4495-B497-6AD4A84163D7");
        readonly public static Guid GUID_DEVINTERFACE_VOLUME = new("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        readonly public static Guid GUID_DEVINTERFACE_DISK = new("53F56307-B6BF-11D0-94F2-00A0C91EFB8B");
        readonly public static Guid GUID_DEVINTERFACE_COMPORT = new("86E0D1E0-8089-11D0-9CE4-08003E301F73");
        readonly public static Guid GUID_DEVINTERFACE_MOUSE = new("378DE44C-56EF-11D1-BC8C-00A0C91405DD");
        readonly public static Guid GUID_DEVINTERFACE_IMAGE = new("6BDD1FC6-810F-11D0-BEC7-08002BE2092F");
        readonly public static Guid GUID_DEVINTERFACE_HID = new("4D1E55B2-F16F-11CF-88CB-001111000030");
    }
}
