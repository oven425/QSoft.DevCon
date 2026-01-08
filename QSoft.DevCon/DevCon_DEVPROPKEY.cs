using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    public static partial class DevConExtension
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct DEVPROPKEY(Guid fmt_id, int p_id)
        {
            readonly public Guid fmtid = fmt_id;
            readonly public UInt32 pid = (uint)p_id;

            public readonly void Deconstruct(out Guid fmtid, out uint pid)
            {
                fmtid = this.fmtid;
                pid = this.pid;
            }
        }

        //https://github.com/lostindark/DriverStoreExplorer/blob/08b0e81bd6b0dc77dceaab45df55205e230552bc/Rapr/Utils/DeviceHelper.cs#L180
        //https://www.magnumdb.com/search?q=filename%3A%22FunctionDiscoveryKeys_devpkey.h%22
        readonly internal static DEVPROPKEY DEVPKEY_Device_DevNodeStatus = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id:2);
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverVersion = new(fmt_id: Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), p_id: 3);
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverDate = new(fmt_id: Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), p_id: 2);
        readonly internal static DEVPROPKEY DPKEY_Device_DeviceDesc = new(fmt_id: Guid.Parse("{a45c254e-df1c-4efd-8020-67d146a850e0}"), p_id : 2);
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverInfSection = new(fmt_id: Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), p_id: 6);
        readonly internal static DEVPROPKEY DEVPKEY_Device_DriverProvider = new(fmt_id: Guid.Parse("{a8b865dd-2e3d-4094-ad97-e593a70c75d6}"), p_id: 9);
        readonly internal static DEVPROPKEY DEVPKEY_Device_ProblemCode = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id: 3);
        readonly internal static DEVPROPKEY DEVPKEY_Device_FirstInstallDate = new(fmt_id: new Guid(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29), p_id: 101);   // DEVPROP_TYPE_FILETIME
        readonly internal static DEVPROPKEY DEVPKEY_Device_IsPresent = new(fmt_id : new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), p_id: 5);
        readonly internal static DEVPROPKEY DEVPKEY_Device_BiosDeviceName = new(fmt_id: new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), p_id: 10);    // DEVPROP_TYPE_STRING
        readonly internal static DEVPROPKEY DEVPKEY_Device_IsConnected = new(fmt_id: Guid.Parse("{83DA6326-97A6-4088-9453-A1923F573B29}"), p_id: 15);
        readonly internal static DEVPROPKEY DEVPKEY_Device_InstanceId = new(fmt_id : Guid.Parse("{78c34fc8-104a-4aca-9ea4-524d52996e57}"), p_id : 256);

        //
        // Device properties
        // These DEVPKEYs correspond to a device's relations.
        //
        //DEFINE_DEVPROPKEY(DEVPKEY_Device_EjectionRelations,      0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 4);     // DEVPROP_TYPE_STRING_LIST
        //DEFINE_DEVPROPKEY(DEVPKEY_Device_RemovalRelations,       0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 5);     // DEVPROP_TYPE_STRING_LIST
        readonly internal static DEVPROPKEY DEVPKEY_Device_PowerRelations = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id: 6);
        readonly internal static DEVPROPKEY DEVPKEY_Device_BusRelations = new(fmt_id:new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), p_id:7);   // DEVPROP_TYPE_STRING_LIST

        readonly internal static DEVPROPKEY DEVPKEY_Device_Parent = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id: 8);
        readonly internal static DEVPROPKEY DEVPKEY_Device_Children = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id: 9);
        readonly internal static DEVPROPKEY DEVPKEY_Device_Siblings = new(fmt_id: Guid.Parse("{4340a6c5-93fa-4706-972c-7b648008a5a7}"), p_id: 10);
        // DEVPROP_TYPE_STRING_LIST
        //DEFINE_DEVPROPKEY(DEVPKEY_Device_TransportRelations,     0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 11);    // DEVPROP_TYPE_STRING_LIST

    }
}
