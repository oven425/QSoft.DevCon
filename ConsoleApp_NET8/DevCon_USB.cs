using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtensiona;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QSoft.DevCon
{
    static public partial class DevConExtensiona
    {
        public static string GetHCDDriverKeyName(this SafeFileHandle src)
        {
            Span<byte> buffer = stackalloc byte[6];
            var b1 = DeviceIoControl(src, 0x00220424, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, out var ss, IntPtr.Zero);
            var ii = BitConverter.ToUInt32(buffer);
            buffer = stackalloc byte[(int)ii];
            b1 = DeviceIoControl(src, 0x00220424, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, out ss, IntPtr.Zero);

            var vvv = buffer[4..^4];
            var ccs = MemoryMarshal.Cast<byte, char>(vvv);
            var str = new string(ccs);
            //{36fc9e60-c465-11cf-8056-444553540000}\0000
            var err = Marshal.GetLastWin32Error();
            return str;
        }
        static uint IOCTL_USB_GET_NODE_INFORMATION = 0x00220408;
        static uint IOCTL_USB_GET_ROOT_HUB_NAME = 0x00220408;
        static uint IOCTL_USB_GET_HUB_INFORMATION_EX = 0x00220454;
        static uint IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES = 0x00220458;
        static uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2 = 0x0022045c;
        static uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX = 0x00220448;


        public static string GetRootHubName(this SafeFileHandle src)
        {
            var myStruct = new USB_HCD_DRIVERKEY_NAME();
            var structSpan = MemoryMarshal.CreateSpan(ref myStruct, 1);
            var buffer = MemoryMarshal.AsBytes(structSpan);

            var success = DeviceIoControl(src, IOCTL_USB_GET_ROOT_HUB_NAME, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out var nBytes, IntPtr.Zero);

            buffer = stackalloc byte[(int)myStruct.ActualLength];
            success = DeviceIoControl(src, IOCTL_USB_GET_ROOT_HUB_NAME, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out nBytes, IntPtr.Zero);
            var vvv = buffer[4..^2];
            var ccs = MemoryMarshal.Cast<byte, char>(vvv);
            var str = new string(ccs);



            return str;
        }

        public static void  GET_NODE_INFORMATION(this SafeFileHandle src)
        {
            var nodeinfo = new USB_NODE_INFORMATION();
            var nodeinfo_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref nodeinfo, 1));
            var success1 = DeviceIoControl(src, IOCTL_USB_GET_NODE_INFORMATION, Span<byte>.Empty, 0, nodeinfo_buffer, (uint)nodeinfo_buffer.Length, out var nBytes1, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            var count = nodeinfo.HubInformation.HubDescriptor.bNumberOfPorts;

            var hubinfoex = new USB_HUB_INFORMATION_EX();
            var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref hubinfoex, 1));
            var success = DeviceIoControl(src, IOCTL_USB_GET_HUB_INFORMATION_EX, Span<byte>.Empty, 0, buffer, (uint)buffer.Length, out var nBytes, IntPtr.Zero);
            err = Marshal.GetLastWin32Error();
            for(uint i=1; i<= nodeinfo.HubInformation.HubDescriptor.bNumberOfPorts;i++)
            {
                var portcoonectproperties = new USB_PORT_CONNECTOR_PROPERTIES();
                portcoonectproperties.ConnectionIndex = i;
                var portcoonectproperties_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref portcoonectproperties, 1));
                
                success = DeviceIoControl(src, IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, out nBytes, IntPtr.Zero);


                err = Marshal.GetLastWin32Error();


                var usb_node_connection_info_v2 = new USB_NODE_CONNECTION_INFORMATION_EX_V2();
                usb_node_connection_info_v2.ConnectionIndex = i;
                usb_node_connection_info_v2.Length = (uint)Marshal.SizeOf<USB_NODE_CONNECTION_INFORMATION_EX_V2>();
                usb_node_connection_info_v2.SupportedUsbProtocols.Usb300 = true;
                var usb_node_connection_info_v2_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref usb_node_connection_info_v2, 1));
                success = DeviceIoControl(src, IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2, usb_node_connection_info_v2_buffer, (uint)usb_node_connection_info_v2_buffer.Length, usb_node_connection_info_v2_buffer, (uint)usb_node_connection_info_v2_buffer.Length, out nBytes, IntPtr.Zero);
                err = Marshal.GetLastWin32Error();


                var connectionInfoEx = new USB_NODE_CONNECTION_INFORMATION_EX();
                connectionInfoEx.ConnectionIndex = i;
                var connectionInfoEx_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref connectionInfoEx, 1));
                success = DeviceIoControl(src, IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, out nBytes, IntPtr.Zero);
                err = Marshal.GetLastWin32Error();

                if(connectionInfoEx.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                {
                    int aa = 0;
                    if(connectionInfoEx.NumberOfOpenPipes > 1)
                    {
                        System.Diagnostics.Trace.WriteLine($"NumberOfOpenPipes:{connectionInfoEx.NumberOfOpenPipes}");
                    }
                    aa = 1;
                }
            }



        }


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_PROTOCOLS
        {
            private uint ul;

            public uint Value
            {
                get => ul;
                set => ul = value;
            }

            public bool Usb110
            {
                get => (ul & 0x1) != 0;
                set => ul = value ? (ul | 0x1u) : (ul & ~0x1u);
            }

            public bool Usb200
            {
                get => (ul & 0x2) != 0;
                set => ul = value ? (ul | 0x2u) : (ul & ~0x2u);
            }

            public bool Usb300
            {
                get => (ul & 0x4) != 0;
                set => ul = value ? (ul | 0x4u) : (ul & ~0x4u);
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS
        {
            private uint ul;
            public uint Value
            {
                get => ul;
                set => ul = value;
            }

            public bool DeviceIsOperatingAtSuperSpeedOrHigher
            {
                get => (ul & 0x1) != 0;
                set => ul = value ? (ul | 0x1u) : (ul & ~0x1u);
            }

            public bool DeviceIsSuperSpeedCapableOrHigher
            {
                get => (ul & 0x2) != 0;
                set => ul = value ? (ul | 0x2u) : (ul & ~0x2u);
            }

            public bool DeviceIsOperatingAtSuperSpeedPlusOrHigher
            {
                get => (ul & 0x4) != 0;
                set => ul = value ? (ul | 0x4u) : (ul & ~0x4u);
            }

            public bool DeviceIsSuperSpeedPlusCapableOrHigher
            {
                get => (ul & 0x8) != 0;
                set => ul = value ? (ul | 0x8u) : (ul & ~0x8u);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_NODE_CONNECTION_INFORMATION_EX_V2
        {
            public uint ConnectionIndex;
            public uint Length;
            public USB_PROTOCOLS SupportedUsbProtocols;
            public USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS Flags;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_DEVICE_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public ushort bcdUSB;
            public byte bDeviceClass;
            public byte bDeviceSubClass;
            public byte bDeviceProtocol;
            public byte bMaxPacketSize0;
            public ushort idVendor;
            public ushort idProduct;
            public ushort bcdDevice;
            public byte iManufacturer;
            public byte iProduct;
            public byte iSerialNumber;
            public byte bNumConfigurations;
        }

        public enum USB_CONNECTION_STATUS
        {
            NoDeviceConnected,
            DeviceConnected,

            /* failure codes, these map to fail reasons */
            DeviceFailedEnumeration,
            DeviceGeneralFailure,
            DeviceCausedOvercurrent,
            DeviceNotEnoughPower,
            DeviceNotEnoughBandwidth,
            DeviceHubNestedTooDeeply,
            DeviceInLegacyHub,
            DeviceEnumerating,
            DeviceReset
        };

        public enum USB_DEVICE_SPEED:byte
        {
            UsbLowSpeed,
            UsbFullSpeed,
            UsbHighSpeed,
            UsbSuperSpeed
        }

        struct USB_ENDPOINT_DESCRIPTOR
        {
            byte bLength;
            byte bDescriptorType;
            byte bEndpointAddress;
            byte bmAttributes;
            ushort wMaxPacketSize;
            byte bInterval;
        };

        struct USB_PIPE_INFO
        {
            USB_ENDPOINT_DESCRIPTOR EndpointDescriptor;
            uint ScheduleOffset;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_NODE_CONNECTION_INFORMATION_EX
        {
            /// <summary>
            /// INPUT: The one-based port number.
            /// </summary>
            public uint ConnectionIndex;
            public USB_DEVICE_DESCRIPTOR DeviceDescriptor;
            public byte CurrentConfigurationValue;
            public USB_DEVICE_SPEED Speed;
            [MarshalAs(UnmanagedType.U1)]
            public bool DeviceIsHub;
            public ushort DeviceAddress;
            public uint NumberOfOpenPipes;
            public USB_CONNECTION_STATUS ConnectionStatus;
            USB_PIPE_INFO PipeList;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_PORT_PROPERTIES
        {

            public uint ul;


            public bool PortIsUserConnectable
            {
                get { return (ul & 0x01) != 0; }
                set { ul = value ? (ul | 0x01u) : (ul & ~0x01u); }
            }

            public bool PortIsDebugCapable
            {
                get { return (ul & 0x02) != 0; }
                set { ul = value ? (ul | 0x02u) : (ul & ~0x02u); }
            }

            public bool PortHasMultipleCompanions
            {
                get { return (ul & 0x04) != 0; }
                set { ul = value ? (ul | 0x04u) : (ul & ~0x04u); }
            }

            public bool PortConnectorIsTypeC
            {
                get { return (ul & 0x08) != 0; }
                set { ul = value ? (ul | 0x08u) : (ul & ~0x08u); }
            }
        }

        public struct USB_PORT_CONNECTOR_PROPERTIES
        {
            // one based port number
            public uint ConnectionIndex;

            // The number of bytes required to hold the entire USB_PORT_CONNECTOR_PROPERTIES
            // structure, including the full CompanionHubSymbolicLinkName string
            public uint ActualLength;

            // bitmask of flags indicating properties and capabilities of the port
            public USB_PORT_PROPERTIES UsbPortProperties;

            // Zero based index number of the companion port being queried.
            public ushort CompanionIndex;

            // Port number of the companion port
            public ushort CompanionPortNumber;

            // Symbolic link name for the companion hub
            public char CompanionHubSymbolicLinkName;
        };

        enum USB_HUB_NODE
        {
            UsbHub,
            UsbMIParent
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_HUB_INFORMATION
        {

            public USB_HUB_DESCRIPTOR HubDescriptor;
            public bool HubIsBusPowered;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_MI_PARENT_INFORMATION
        {
            uint NumberOfInterfaces;
        };
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        struct USB_NODE_INFORMATION
        {
            [FieldOffset(0)]
            public USB_HUB_NODE NodeType;        /* hub, mi parent */
            [FieldOffset(4)]
            public USB_HUB_INFORMATION HubInformation;
            [FieldOffset(4)]
            public USB_MI_PARENT_INFORMATION MiParentInformation;
        };


        [StructLayout(LayoutKind.Sequential, Pack =1, CharSet = CharSet.Unicode)]
        public struct USB_HCD_DRIVERKEY_NAME
        {
            public uint ActualLength;
            public char DriverKeyName;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct USB_HUB_DESCRIPTOR
        {
            public byte bDescriptorLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            public ushort wHubCharacteristics;
            public byte bPowerOnToPowerGood;
            public byte bHubControlCurrent;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            //public byte[] DeviceRemovable;
            public fixed byte DeviceRemovable[64];
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_30_HUB_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            public ushort wHubCharacteristics;
            public byte bPowerOnToPowerGood;
            public byte bHubControlCurrent;
            public byte bHubHdrDecLat;
            public ushort wHubDelay;
            public ushort DeviceRemovable;
        };

        public enum USB_HUB_TYPE
        {
            UsbRootHub = 1,
            Usb20Hub = 2,
            Usb30Hub = 3
        };
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct USB_HUB_INFORMATION_EX
        {
            [FieldOffset(0)]
            public USB_HUB_TYPE HubType;

            [FieldOffset(4)]
            public ushort HighestPortNumber;
            [FieldOffset(6)]
            public USB_HUB_DESCRIPTOR UsbHubDescriptor;
            [FieldOffset(6)]
            public USB_30_HUB_DESCRIPTOR Usb30HubDescriptor;

        };

    enum WDMUSB_POWER_STATE
        {

            WdmUsbPowerNotMapped = 0,

            WdmUsbPowerSystemUnspecified = 100,
            WdmUsbPowerSystemWorking,
            WdmUsbPowerSystemSleeping1,
            WdmUsbPowerSystemSleeping2,
            WdmUsbPowerSystemSleeping3,
            WdmUsbPowerSystemHibernate,
            WdmUsbPowerSystemShutdown,

            WdmUsbPowerDeviceUnspecified = 200,
            WdmUsbPowerDeviceD0,
            WdmUsbPowerDeviceD1,
            WdmUsbPowerDeviceD2,
            WdmUsbPowerDeviceD3

        };


        enum USB_USER_ERROR_CODE
        {
            UsbUserSuccess = 0,
            UsbUserNotSupported,
            UsbUserInvalidRequestCode,
            UsbUserFeatureDisabled,
            UsbUserInvalidHeaderParameter,
            UsbUserInvalidParameter,
            UsbUserMiniportError,
            UsbUserBufferTooSmall,
            UsbUserErrorNotMapped,
            UsbUserDeviceNotStarted,
            UsbUserNoDeviceConnected

        };


        struct USBUSER_REQUEST_HEADER
        {
            uint UsbUserRequest;
            /*
                status code returned by port driver
            */
            USB_USER_ERROR_CODE UsbUserStatusCode;
            /*
                size of client input/output buffer
                we always use the same buffer for input
                and output
            */
            uint RequestBufferLength;
            /*
                size of buffer required to get all of the data
            */
            uint ActualBufferLength;

        };

        struct USB_POWER_INFO
        {

            /* input */
            WDMUSB_POWER_STATE SystemState;
            /* output */
            WDMUSB_POWER_STATE HcDevicePowerState;
            WDMUSB_POWER_STATE HcDeviceWake;
            WDMUSB_POWER_STATE HcSystemWake;

            WDMUSB_POWER_STATE RhDevicePowerState;
            WDMUSB_POWER_STATE RhDeviceWake;
            WDMUSB_POWER_STATE RhSystemWake;

            WDMUSB_POWER_STATE LastSystemSleepState;

            bool CanWakeup;
            bool IsPowered;

        };

        struct USBUSER_POWER_INFO_REQUEST
        {

            USBUSER_REQUEST_HEADER Header;
            USB_POWER_INFO PowerInformation;

        };


        static uint IOCTL_GET_HCD_DRIVERKEY_NAME = CTL_CODE(
            FILE_DEVICE_USB, // 0x7
            HCD_GET_DRIVERKEY_NAME,                      // 功能碼
            METHOD_BUFFERED,  // 0
            FILE_ANY_ACCESS   // 0
        );


        static uint HCD_GET_DRIVERKEY_NAME = 265;

        static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method);
        }


        const uint METHOD_BUFFERED = 0;
        public const uint METHOD_IN_DIRECT = 1;
        public const uint METHOD_OUT_DIRECT = 2;
        public const uint METHOD_NEITHER = 3;

        public const uint FILE_ANY_ACCESS = 0;
        public const uint FILE_READ_ACCESS = 0x0001;
        public const uint FILE_WRITE_ACCESS = 0x0002;

        public const uint FILE_DEVICE_BEEP = 0x00000001;
        public const uint FILE_DEVICE_CD_ROM = 0x00000002;
        public const uint FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x00000003;
        public const uint FILE_DEVICE_CONTROLLER = 0x00000004;
        public const uint FILE_DEVICE_DATALINK = 0x00000005;
        public const uint FILE_DEVICE_DFS = 0x00000006;
        public const uint FILE_DEVICE_DISK = 0x00000007;
        public const uint FILE_DEVICE_DISK_FILE_SYSTEM = 0x00000008;
        public const uint FILE_DEVICE_FILE_SYSTEM = 0x00000009;
        public const uint FILE_DEVICE_KEYBOARD = 0x0000000b;
        public const uint FILE_DEVICE_MOUSE = 0x0000000c;
        public const uint FILE_DEVICE_NETWORK = 0x00000012;
        public const uint FILE_DEVICE_SERIAL_PORT = 0x0000001b;
        public const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        public const uint FILE_DEVICE_USB = 0x00000022; // 與 UNKNOWN 相同
        public const uint FILE_DEVICE_VIDEO = 0x00000023;
    }
}
