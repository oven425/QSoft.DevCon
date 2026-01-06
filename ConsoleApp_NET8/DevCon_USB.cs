using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DevConExtensiona;


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
            var err = Marshal.GetLastWin32Error();
            return str;
        }
        static uint IOCTL_USB_GET_NODE_INFORMATION = 0x00220408;
        static uint IOCTL_USB_GET_ROOT_HUB_NAME = 0x00220408;
        static uint IOCTL_USB_GET_HUB_INFORMATION_EX = 0x00220454;
        static uint IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES = 0x00220458;
        static uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2 = 0x0022045c;
        static uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX = 0x00220448;
        static uint IOCTL_USB_GET_NODE_CONNECTION_NAME = 0x0022044c;
        static uint IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = 0x00220410;


        public static string GetRootHubName(this SafeFileHandle src) => src.GetString(IOCTL_USB_GET_ROOT_HUB_NAME);

        public static USB_PORT_CONNECTOR_PROPERTIES GetPortConntorProperties(this SafeFileHandle src, uint index)
        {
            var portcoonectproperties = new USB_PORT_CONNECTOR_PROPERTIES
            {
                ConnectionIndex = index
            };
            var portcoonectproperties_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref portcoonectproperties, 1));

            var success = DeviceIoControl(src, IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, out var nBytes, IntPtr.Zero);


            var err = Marshal.GetLastWin32Error();
            return portcoonectproperties;
        }

        public static USB_NODE_CONNECTION_INFORMATION_EX GetNodeConnectionInformationEX(this SafeFileHandle src, uint index)
        {
            var connectionInfoEx = new USB_NODE_CONNECTION_INFORMATION_EX
            {
                ConnectionIndex = index
            };
            var connectionInfoEx_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref connectionInfoEx, 1));
            var success = DeviceIoControl(src, IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, out var nBytes, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            return connectionInfoEx;

        }

        public static USB_NODE_INFORMATION NodeInfo(this SafeFileHandle src)
        {
            var nodeinfo = new USB_NODE_INFORMATION();
            var nodeinfo_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref nodeinfo, 1));
            var success = DeviceIoControl(src, IOCTL_USB_GET_NODE_INFORMATION, [], 0, nodeinfo_buffer, (uint)nodeinfo_buffer.Length, out var nBytes, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            return nodeinfo;

        }


        public static void  GET_NODE_INFORMATION(this SafeFileHandle src)
        {
            var nodeinfo = new USB_NODE_INFORMATION();
            var nodeinfo_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref nodeinfo, 1));
            var success = DeviceIoControl(src, IOCTL_USB_GET_NODE_INFORMATION, [], 0, nodeinfo_buffer, (uint)nodeinfo_buffer.Length, out var nBytes, IntPtr.Zero);
            var err = Marshal.GetLastWin32Error();
            var count = nodeinfo.HubInformation.HubDescriptor.bNumberOfPorts;

            //var hubinfoex = new USB_HUB_INFORMATION_EX();
            //var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref hubinfoex, 1));
            //var success = DeviceIoControl(src, IOCTL_USB_GET_HUB_INFORMATION_EX, [], 0, buffer, (uint)buffer.Length, out var nBytes, IntPtr.Zero);
            //err = Marshal.GetLastWin32Error();
            for(uint i=1; i<= nodeinfo.HubInformation.HubDescriptor.bNumberOfPorts;i++)
            {
                //var portcoonectproperties = new USB_PORT_CONNECTOR_PROPERTIES
                //{
                //    ConnectionIndex = i
                //};
                //var portcoonectproperties_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref portcoonectproperties, 1));
                
                //success = DeviceIoControl(src, IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, portcoonectproperties_buffer, (uint)portcoonectproperties_buffer.Length, out nBytes, IntPtr.Zero);


                err = Marshal.GetLastWin32Error();


                var usb_node_connection_info_v2 = new USB_NODE_CONNECTION_INFORMATION_EX_V2
                {
                    ConnectionIndex = i,
                    Length = (uint)Marshal.SizeOf<USB_NODE_CONNECTION_INFORMATION_EX_V2>()
                };
                usb_node_connection_info_v2.SupportedUsbProtocols.Usb300 = true;
                var usb_node_connection_info_v2_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref usb_node_connection_info_v2, 1));
                success = DeviceIoControl(src, IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2, usb_node_connection_info_v2_buffer, (uint)usb_node_connection_info_v2_buffer.Length, usb_node_connection_info_v2_buffer, (uint)usb_node_connection_info_v2_buffer.Length, out nBytes, IntPtr.Zero);
                err = Marshal.GetLastWin32Error();


                var connectionInfoEx = new USB_NODE_CONNECTION_INFORMATION_EX
                {
                    ConnectionIndex = i
                };
                var connectionInfoEx_buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref connectionInfoEx, 1));
                success = DeviceIoControl(src, IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, connectionInfoEx_buffer, (uint)connectionInfoEx_buffer.Length, out nBytes, IntPtr.Zero);
                err = Marshal.GetLastWin32Error();

                if(connectionInfoEx.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                {
                    //if(i==6)
                    {
                        var buf = src.GetConfigDescriptor(i, 0);
                        DisplayConfigDesc(usb_node_connection_info_v2, buf);
                    }
                    
                    int aa = 0;
                    if(connectionInfoEx.DeviceIsHub)
                    {
                        System.Diagnostics.Trace.WriteLine($"Port {i} is Hub");
                    }
                    
                    if (connectionInfoEx.NumberOfOpenPipes > 1)
                    {
                        System.Diagnostics.Trace.WriteLine($"NumberOfOpenPipes:{connectionInfoEx.NumberOfOpenPipes}");
                    }
                }
            }
        }

        public static (List<Range> ranges, byte[] raw) ParseConfig(this byte[] src)
        {
            var commonDesc_buf = src;
            var commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(commonDesc_buf);
            List<Range> ll = [];
            int index = 0;
            while (commonDesc_buf.Length > 0)
            {
                var commonDesc1 = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(commonDesc_buf);
                if (commonDesc1.bLength == 0 || commonDesc_buf.Length < commonDesc1.bLength)
                    break;
                System.Diagnostics.Trace.WriteLine($"bDescriptorType:{commonDesc1.bDescriptorType}, bLength:{commonDesc1.bLength}");
                commonDesc_buf = commonDesc_buf[commonDesc1.bLength..];
                ll.Add(index..(index + commonDesc1.bLength));

                index = index + commonDesc1.bLength;
            }


            foreach (var oo in ll)
            {
                var oi = src[oo];
                commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(oi);
                switch (commonDesc.bDescriptorType)
                {
                    case USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE:
                        var quilty = MemoryMarshal.Read<USB_DEVICE_QUALIFIER_DESCRIPTOR>(oi);
                        break;
                    case USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE:
                        break;
                    case USB_CONFIGURATION_DESCRIPTOR_TYPE:
                        var usb_configuration_desc = MemoryMarshal.Read<USB_CONFIGURATION_DESCRIPTOR>(oi);
                        break;
                    case USB_INTERFACE_DESCRIPTOR_TYPE:
                        var cc = MemoryMarshal.Read<USB_INTERFACE_DESCRIPTOR>(oi);
                        //bInterfaceClass = cc.bInterfaceClass;
                        //bInterfaceSubClass = cc.bInterfaceSubClass;
                        //bInterfaceProtocol = cc.bInterfaceProtocol;
                        break;
                    case USB_ENDPOINT_DESCRIPTOR_TYPE:
                        var end = MemoryMarshal.Read<USB_ENDPOINT_DESCRIPTOR>(oi);
                        break;
                    case USB_HID_DESCRIPTOR_TYPE:
                        //var hid = MemoryMarshal.Read<USB_HID_DESCRIPTOR>(oi);
                        break;
                    default:
                        {
                            //switch (bInterfaceClass)
                            //{
                            //    case USB_DEVICE_CLASS_VIDEO:
                            //        var ccc = MemoryMarshal.Read<VIDEO_SPECIFIC>(oi);
                            //        break;
                            //}
                        }
                        break;
                }


            }
            return (ll, src);
        }

        public static byte[] GetConfigDescriptor(this SafeFileHandle hHubDevice, uint ConnectionIndex, byte DescriptorIndex=0)
        {
            bool success = false;
            uint nBytes = 0;
            uint nBytesReturned = 0;

            var sz1 = Marshal.SizeOf<SetupPacket>();
            var reqsz = Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>();
            var configdescsz = Marshal.SizeOf<USB_CONFIGURATION_DESCRIPTOR>();
            var configDescReqBuf = new byte[Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>() + Marshal.SizeOf<USB_CONFIGURATION_DESCRIPTOR>()];
            Span<byte> configDescReqSpan = configDescReqBuf.AsSpan();


            USB_DESCRIPTOR_REQUEST configDescReq = new();
            USB_CONFIGURATION_DESCRIPTOR configDesc = new();

            nBytes = (uint)configDescReqBuf.Length;

            configDescReq.ConnectionIndex = ConnectionIndex;

            configDescReq.SetupPacket.wValue = (ushort)((USB_CONFIGURATION_DESCRIPTOR_TYPE << 8) | DescriptorIndex);

            configDescReq.SetupPacket.wLength = (ushort)(nBytes - Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>());

            MemoryMarshal.Write(configDescReqBuf, in configDescReq);


            success = DeviceIoControl(hHubDevice,
                                      IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      configDescReqBuf,
                                      nBytes,
                                      configDescReqBuf,
                                      nBytes,
                                      out nBytesReturned,
                                      IntPtr.Zero);
            var offset = Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>() ;
            configDesc = MemoryMarshal.Read< USB_CONFIGURATION_DESCRIPTOR>(configDescReqBuf[offset..]);

            if (!success)
            {
                return [];
            }

            if (nBytes != nBytesReturned)
            {
                return [];
            }

            if (configDesc.wTotalLength < Marshal.SizeOf<USB_CONFIGURATION_DESCRIPTOR>())
            {
                return [];
            }

            // Now request the entire Configuration Descriptor using a dynamically
            // allocated buffer which is sized big enough to hold the entire descriptor
            //
            nBytes = (uint)Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>();
            nBytes = (uint)(Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>() + configDesc.wTotalLength);

            //configDescReq = (PUSB_DESCRIPTOR_REQUEST)ALLOC(nBytes);

            //if (configDescReq == NULL)
            //{
            //    return NULL;
            //}

            configDescReqBuf = new byte[(int)nBytes];
            configDescReqSpan = configDescReqBuf.AsSpan();
            //configDesc = (PUSB_CONFIGURATION_DESCRIPTOR)(configDescReq + 1);

            // Indicate the port from which the descriptor will be requested
            //
            configDescReq.ConnectionIndex = ConnectionIndex;

            //
            // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
            // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
            //
            // USBD will automatically initialize these fields:
            //     bmRequest = 0x80
            //     bRequest  = 0x06
            //
            // We must inititialize these fields:
            //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
            //     wIndex    = Zero (or Language ID for String Descriptors)
            //     wLength   = Length of descriptor buffer
            //
            configDescReq.SetupPacket.wValue = (ushort)((USB_CONFIGURATION_DESCRIPTOR_TYPE << 8)| DescriptorIndex);

            configDescReq.SetupPacket.wLength = (ushort)(nBytes - Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>());

            // Now issue the get descriptor request.
            //
            MemoryMarshal.Write(configDescReqBuf, in configDescReq);

            success = DeviceIoControl(hHubDevice,
                                      IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      configDescReqBuf,
                                      nBytes,
                                      configDescReqBuf,
                                      nBytes,
                                      out nBytesReturned,
                                      IntPtr.Zero);
            configDesc = MemoryMarshal.Read<USB_CONFIGURATION_DESCRIPTOR>(configDescReqBuf[Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>()..]);
            configDescReqBuf = configDescReqBuf[Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>()..];
            if (!success)
            {
                return [];
            }

            if (nBytes != nBytesReturned)
            {
                return [];
            }

            if (configDesc.wTotalLength != (nBytes - Marshal.SizeOf<USB_DESCRIPTOR_REQUEST>()))
            {
                return [];
            }

            var ty = configDesc.bDescriptorType;

            return configDescReqBuf;
        }


        struct USB_HID_DESCRIPTOR
        {
            byte bLength;
            byte bDescriptorType;
            ushort bcdHID;
            byte bCountryCode;
            byte bNumDescriptors;
            //struct
            //{
            //    byte bDescriptorType;
            //    ushort wDescriptorLength;
            //}
            //OptionalDescriptors[1];
        };
        public static void DisplayConfigDesc(USB_NODE_CONNECTION_INFORMATION_EX_V2 ConnectionInfoV2, byte[] ConfigDesc/*, PSTRING_DESCRIPTOR_NODE StringDescs*/)
        {
            var ConfigDesc_buf = ConfigDesc.AsSpan();
            byte bInterfaceClass = 0;
            byte bInterfaceSubClass = 0;
            byte bInterfaceProtocol = 0;
            bool displayUnknown = false;
            var isSS = ConnectionInfoV2.Flags.DeviceIsOperatingAtSuperSpeedOrHigher;

            var commonDesc_buf = ConfigDesc_buf;
            var commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(commonDesc_buf);
            List<Range> ll = [];
            int index = 0;
            while (commonDesc_buf.Length > 0)
            {
                var commonDesc1 = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(commonDesc_buf);
                if (commonDesc1.bLength == 0 || commonDesc_buf.Length < commonDesc1.bLength)
                    break;
                System.Diagnostics.Trace.WriteLine($"bDescriptorType:{commonDesc1.bDescriptorType}, bLength:{commonDesc1.bLength}");
                commonDesc_buf = commonDesc_buf[commonDesc1.bLength..];
                ll.Add(index..(index+commonDesc1.bLength));

                index = index + commonDesc1.bLength;
            }
            

            foreach (var oo in ll)
            {
                var oi = ConfigDesc_buf[oo];
                commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(oi);
                switch(commonDesc.bDescriptorType)
                {
                    case USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE:
                        var quilty = MemoryMarshal.Read<USB_DEVICE_QUALIFIER_DESCRIPTOR>(oi);
                        break;
                    case USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE:                        
                        break;
                    case USB_CONFIGURATION_DESCRIPTOR_TYPE:
                        var usb_configuration_desc = MemoryMarshal.Read<USB_CONFIGURATION_DESCRIPTOR>(oi);
                        break;
                    case USB_INTERFACE_DESCRIPTOR_TYPE:
                        var cc = MemoryMarshal.Read<USB_INTERFACE_DESCRIPTOR>(oi);
                        bInterfaceClass = cc.bInterfaceClass;
                        bInterfaceSubClass = cc.bInterfaceSubClass;
                        bInterfaceProtocol = cc.bInterfaceProtocol;
                        System.Diagnostics.Trace.WriteLine($"Class:{bInterfaceClass} SubClass:{bInterfaceSubClass} Protocol:{bInterfaceProtocol}");
                        break;
                    case USB_ENDPOINT_DESCRIPTOR_TYPE:
                        var end = MemoryMarshal.Read<USB_ENDPOINT_DESCRIPTOR>(oi);
                        break;
                    case USB_HID_DESCRIPTOR_TYPE:
                        var hid = MemoryMarshal.Read<USB_HID_DESCRIPTOR>(oi);
                        break;
                    default:
                        {
                            switch (bInterfaceClass)
                            {
                                case USB_DEVICE_CLASS_VIDEO:
                                    oi.GetUVC(bInterfaceClass, bInterfaceSubClass);
                                    break;
                            }
                        }
                        break;
                }

                
            }
            return;
            commonDesc_buf = ConfigDesc_buf;
            do
            {
                //displayUnknown = FALSE;

                switch (commonDesc.bDescriptorType)
                {
                    case USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE:
                        ////@@DisplayConfigDesc - Device Qualifier Descriptor
                        //if (commonDesc.bLength != Marshal.SizeOf<USB_DEVICE_QUALIFIER_DESCRIPTOR>())
                        //{
                        //    ////@@TestCase A2.1
                        //    ////@@ERROR
                        //    ////@@Descriptor Field - bLength
                        //    ////@@The declared length in the device descriptor is not equal to the
                        //    ////@@  required length in the USB Device Specification
                        //    //AppendTextBuffer("*!*ERROR:  bLength of %d for Device Qualifier incorrect, "\

                        //    //    "should be %d\r\n",
                        //    //    commonDesc->bLength,
                        //    //    (UCHAR)sizeof(USB_DEVICE_QUALIFIER_DESCRIPTOR));
                        //    //OOPS();
                        //    //displayUnknown = TRUE;
                        //    break;
                        //}
                        //DisplayDeviceQualifierDescriptor((PUSB_DEVICE_QUALIFIER_DESCRIPTOR)commonDesc);
                        break;

                    case USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE:
                        ////@@DisplayConfigDesc - Other Speed Configuration Descriptor
                        //if (commonDesc->bLength != sizeof(USB_CONFIGURATION_DESCRIPTOR))
                        //{
                        //    //@@TestCase A2.2
                        //    //@@ERROR
                        //    //@@Descriptor Field - bLength
                        //    //@@The declared length in the device descriptor is not equal to the
                        //    //@@  required length in the USB Device Specification
                        //    AppendTextBuffer("*!*ERROR:  bLength of %d for Other Speed Configuration "\

                        //        "incorrect, should be %d\r\n",
                        //        commonDesc->bLength,
                        //        (UCHAR)sizeof(USB_CONFIGURATION_DESCRIPTOR));
                        //    OOPS();
                        //    displayUnknown = TRUE;
                        //}
                        //DisplayConfigurationDescriptor(
                        //    (PUSBDEVICEINFO)info,
                        //    (PUSB_CONFIGURATION_DESCRIPTOR)commonDesc,
                        //    StringDescs);
                        break;

                    case USB_CONFIGURATION_DESCRIPTOR_TYPE:
                        //@@DisplayConfigDesc - Configuration Descriptor
                        if (commonDesc.bLength != Marshal.SizeOf<USB_CONFIGURATION_DESCRIPTOR>())
                        {
                            ////@@TestCase A2.3
                            ////@@ERROR
                            ////@@Descriptor Field - bLength
                            ////@@The declared length in the device descriptor is not equal to the
                            ////@@required length in the USB Device Specification
                            //AppendTextBuffer("*!*ERROR:  bLength of %d for Configuration incorrect, "\

                            //    "should be %d\r\n",
                            //    commonDesc->bLength,
                            //    (UCHAR)sizeof(USB_CONFIGURATION_DESCRIPTOR));
                            //OOPS();
                            //displayUnknown = TRUE;
                            break;
                        }
                        //DisplayConfigurationDescriptor((PUSBDEVICEINFO)info,
                        //    (PUSB_CONFIGURATION_DESCRIPTOR)commonDesc,
                        //    StringDescs);

                        DisplayConfigurationDescriptor(ConfigDesc_buf);
                        break;

                    case USB_INTERFACE_DESCRIPTOR_TYPE:
                        //@@DisplayConfigDesc - Interface Descriptor
                        if ((commonDesc.bLength != Marshal.SizeOf<USB_INTERFACE_DESCRIPTOR>()) &&
                            (commonDesc.bLength != Marshal.SizeOf<USB_INTERFACE_DESCRIPTOR2>()))
                        {
                            ////@@TestCase A2.4
                            ////@@ERROR
                            ////@@Descriptor Field - bLength
                            ////@@The declared length in the device descriptor is not equal to the
                            ////@@required length in the USB Device Specification
                            //AppendTextBuffer("*!*ERROR:  bLength of %d for Interface incorrect, "\

                            //    "should be %d or %d\r\n",
                            //    commonDesc->bLength,
                            //    (UCHAR)sizeof(USB_INTERFACE_DESCRIPTOR),
                            //    (UCHAR)sizeof(USB_INTERFACE_DESCRIPTOR2));
                            //OOPS();
                            //displayUnknown = TRUE;
                            break;
                        }
                        var cc = MemoryMarshal.Read<USB_INTERFACE_DESCRIPTOR>(commonDesc_buf);
                        bInterfaceClass = cc.bInterfaceClass;
                        bInterfaceSubClass = cc.bInterfaceSubClass;
                        bInterfaceProtocol = cc.bInterfaceProtocol;

                        //DisplayInterfaceDescriptor(
                        //        (PUSB_INTERFACE_DESCRIPTOR)commonDesc,
                        //        StringDescs,
                        //        info->DeviceInfoNode != NULL ? info->DeviceInfoNode->LatestDevicePowerState : PowerDeviceUnspecified);

                        break;

                    case USB_ENDPOINT_DESCRIPTOR_TYPE:
                        {
                            //PUSB_SUPERSPEED_ENDPOINT_COMPANION_DESCRIPTOR epCompDesc = NULL;
                            //PUSB_SUPERSPEEDPLUS_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR
                            //                                              sspIsochCompDesc = NULL;


                            ////@@DisplayConfigDesc - Endpoint Descriptor
                            //if ((commonDesc->bLength != sizeof(USB_ENDPOINT_DESCRIPTOR)) &&
                            //    (commonDesc->bLength != sizeof(USB_ENDPOINT_DESCRIPTOR2)))
                            //{
                            //    //@@TestCase A2.5
                            //    //@@ERROR
                            //    //@@Descriptor Field - bLength
                            //    //@@The declared length in the device descriptor is not equal to
                            //    //@@  the required length in the USB Device Specification
                            //    AppendTextBuffer("*!*ERROR:  bLength of %d for Endpoint incorrect, "\

                            //        "should be %d or %d\r\n",
                            //        commonDesc->bLength,
                            //        (UCHAR)sizeof(USB_ENDPOINT_DESCRIPTOR),
                            //        (UCHAR)sizeof(USB_ENDPOINT_DESCRIPTOR2));
                            //    OOPS();
                            //    displayUnknown = TRUE;
                            //    break;
                            //}

                            //if (isSS)
                            //{
                            //    epCompDesc = (PUSB_SUPERSPEED_ENDPOINT_COMPANION_DESCRIPTOR)
                            //       GetNextDescriptor((PUSB_COMMON_DESCRIPTOR)ConfigDesc, ConfigDesc->wTotalLength, commonDesc, -1);
                            //}

                            //if (epCompDesc != NULL &&
                            //    epCompDesc->bmAttributes.Isochronous.SspCompanion == 1)
                            //{
                            //    sspIsochCompDesc = (PUSB_SUPERSPEEDPLUS_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR)
                            //        GetNextDescriptor((PUSB_COMMON_DESCRIPTOR)ConfigDesc,
                            //            ConfigDesc->wTotalLength,
                            //            (PUSB_COMMON_DESCRIPTOR)epCompDesc,
                            //            -1);
                            //}

                            //DisplayEndpointDescriptor((PUSB_ENDPOINT_DESCRIPTOR)commonDesc,
                            //    epCompDesc,
                            //    sspIsochCompDesc,
                            //    bInterfaceClass,
                            //    TRUE);

                            //if (sspIsochCompDesc != NULL)
                            //{
                            //    commonDesc = (PUSB_COMMON_DESCRIPTOR)sspIsochCompDesc;
                            //}
                            //else if (epCompDesc != NULL)
                            //{
                            //    commonDesc = (PUSB_COMMON_DESCRIPTOR)epCompDesc;
                            //}
                        }

                        break;

                    case USB_HID_DESCRIPTOR_TYPE:
                        //if (commonDesc->bLength < sizeof(USB_HID_DESCRIPTOR))
                        //{
                        //    OOPS();
                        //    displayUnknown = TRUE;
                        //    break;
                        //}
                        //DisplayHidDescriptor((PUSB_HID_DESCRIPTOR)commonDesc);
                        break;

                    case USB_OTG_DESCRIPTOR_TYPE:
                        //if (commonDesc->bLength < sizeof(USB_OTG_DESCRIPTOR))
                        //{
                        //    OOPS();
                        //    displayUnknown = TRUE;
                        //    break;
                        //}
                        //DisplayOTGDescriptor((PUSB_OTG_DESCRIPTOR)commonDesc);
                        break;

                    case USB_IAD_DESCRIPTOR_TYPE:
                        //if (commonDesc->bLength < sizeof(USB_IAD_DESCRIPTOR))
                        //{
                        //    OOPS();
                        //    displayUnknown = TRUE;
                        //    break;
                        //}
                        //USB_IAD_DESCRIPTOR
                        var idadespc = MemoryMarshal.Read<USB_IAD_DESCRIPTOR>(commonDesc_buf);
                        
                        //DisplayIADDescriptor((PUSB_IAD_DESCRIPTOR)commonDesc, StringDescs,
                        //        ConfigDesc->bNumInterfaces,
                        //        info->DeviceInfoNode != NULL ? info->DeviceInfoNode->LatestDevicePowerState : PowerDeviceUnspecified);
                        break;

                    default:
                        //@@DisplayConfigDesc - Interface Class Device
                        // TODO: BUG: bInterfaceClass is initialized before this code

                        switch (bInterfaceClass)
                        {
                            case USB_DEVICE_CLASS_AUDIO:
                                //displayUnknown = !DisplayAudioDescriptor(
                                //    (PUSB_AUDIO_COMMON_DESCRIPTOR)commonDesc,
                                //    bInterfaceSubClass);
                                break;

                            case USB_DEVICE_CLASS_VIDEO:
                                var ccc = MemoryMarshal.Read<VIDEO_SPECIFIC>(commonDesc_buf);
                                DisplayVideoDescriptor(commonDesc_buf, bInterfaceSubClass, new StringBuilder(), DEVICE_POWER_STATE.PowerDeviceD0);
                                //displayUnknown = !DisplayVideoDescriptor(
                                //    (PVIDEO_SPECIFIC)commonDesc,
                                //    bInterfaceSubClass,
                                //    StringDescs,
                                //    info->DeviceInfoNode != NULL ? info->DeviceInfoNode->LatestDevicePowerState : PowerDeviceUnspecified);
                                break;

                            //case USB_DEVICE_CLASS_RESERVED:
                            //    //@@TestCase A2.6
                            //    //@@ERROR
                            //    //@@Descriptor Field - bInterfaceClass
                            //    //@@An unknown interface class has been defined
                            //    AppendTextBuffer("*!*ERROR:  %d is a Reserved USB Device Interface Class\r\n",
                            //        USB_DEVICE_CLASS_RESERVED);
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_COMMUNICATIONS:
                            //    AppendTextBuffer("  -> This is a Communications (CDC Control) USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_HUMAN_INTERFACE:
                            //    AppendTextBuffer("  -> This is a HID USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_MONITOR:
                            //    AppendTextBuffer("  -> This is a Monitor USB Device Interface Class (This may be obsolete)\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_PHYSICAL_INTERFACE:
                            //    AppendTextBuffer("  -> This is a Physical Interface USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_POWER:
                            //    if (bInterfaceSubClass == 1 && bInterfaceProtocol == 1)
                            //    {
                            //        AppendTextBuffer("  -> This is an Image USB Device Interface Class\r\n");
                            //    }
                            //    else
                            //    {
                            //        AppendTextBuffer("  -> This is a Power USB Device Interface Class (This may be obsolete)\r\n");
                            //    }
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_PRINTER:
                            //    AppendTextBuffer("  -> This is a Printer USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_STORAGE:
                            //    AppendTextBuffer("  -> This is a Mass Storage USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DEVICE_CLASS_HUB:
                            //    AppendTextBuffer("  -> This is a HUB USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_CDC_DATA_INTERFACE:
                            //    AppendTextBuffer("  -> This is a CDC Data USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_CHIP_SMART_CARD_INTERFACE:
                            //    AppendTextBuffer("  -> This is a Chip/Smart Card USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_CONTENT_SECURITY_INTERFACE:
                            //    AppendTextBuffer("  -> This is a Content Security USB Device Interface Class\r\n");
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_DIAGNOSTIC_DEVICE_INTERFACE:
                            //    if (bInterfaceSubClass == 1 && bInterfaceProtocol == 1)
                            //    {
                            //        AppendTextBuffer("  -> This is a Reprogrammable USB2 Compliance Diagnostic Device USB Device\r\n");
                            //    }
                            //    else
                            //    {
                            //        //@@TestCase A2.7
                            //        //@@CAUTION
                            //        //@@Descriptor Field - bInterfaceClass
                            //        //@@An unknown diagnostic interface class device has been defined
                            //        AppendTextBuffer("*!*CAUTION:    This appears to be an invalid Interface Class\r\n");
                            //        OOPS();
                            //    }
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_WIRELESS_CONTROLLER_INTERFACE:
                            //    if (bInterfaceSubClass == 1 && bInterfaceProtocol == 1)
                            //    {
                            //        AppendTextBuffer("  -> This is a Wireless RF Controller USB Device Interface Class with Bluetooth Programming Interface\r\n");
                            //    }
                            //    else
                            //    {
                            //        //@@TestCase A2.8
                            //        //@@CAUTION
                            //        //@@Descriptor Field - bInterfaceClass
                            //        //@@An unknown wireless controller interface class device has been defined
                            //        AppendTextBuffer("*!*CAUTION:    This appears to be an invalid Interface Class\r\n");
                            //        OOPS();
                            //    }
                            //    displayUnknown = TRUE;
                            //    break;

                            //case USB_APPLICATION_SPECIFIC_INTERFACE:
                            //    AppendTextBuffer("  -> This is an Application Specific USB Device Interface Class\r\n");

                            //    switch (bInterfaceSubClass)
                            //    {
                            //        case 1:
                            //            AppendTextBuffer("  -> This is a Device Firmware Application Specific USB Device Interface Class\r\n");
                            //            break;
                            //        case 2:
                            //            AppendTextBuffer("  -> This is an IrDA Bridge Application Specific USB Device Interface Class\r\n");
                            //            break;
                            //        case 3:
                            //            AppendTextBuffer("  -> This is a Test & Measurement Class (USBTMC) Application Specific USB Device Interface Class\r\n");
                            //            break;
                            //        default:
                            //            //@@TestCase A2.9
                            //            //@@CAUTION
                            //            //@@Descriptor Field - bInterfaceClass
                            //            //@@A possibly invalid interface class has been defined
                            //            AppendTextBuffer("*!*CAUTION:    This appears to be an invalid Interface Class\r\n");
                            //            OOPS();
                            //    }
                            //    displayUnknown = TRUE;
                            //    break;

                            //default:
                            //    if (bInterfaceClass == USB_DEVICE_CLASS_VENDOR_SPECIFIC)
                            //    {
                            //        AppendTextBuffer("  -> This is a Vendor Specific USB Device Interface Class\r\n");
                            //    }
                            //    else
                            //    {
                            //        //@@TestCase A2.10
                            //        //@@CAUTION
                            //        //@@Descriptor Field - bInterfaceClass
                            //        //@@An unknown interface class has been defined
                            //        AppendTextBuffer("*!*CAUTION:    This appears to be an invalid Interface Class\r\n");
                            //        OOPS();
                            //    }
                            //    displayUnknown = TRUE;
                            //    break;
                        }
                        break;
                }

                if (displayUnknown)
                {
                    //DisplayUnknownDescriptor(commonDesc);
                }
                commonDesc_buf = commonDesc_buf[commonDesc.bLength ..];
                commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(commonDesc_buf);
            } while (true);
            //while ((commonDesc = GetNextDescriptor((PUSB_COMMON_DESCRIPTOR)ConfigDesc,
            //                                         ConfigDesc->wTotalLength,
            //                                         commonDesc,
            //                                         -1)) != NULL);

//# ifdef H264_SUPPORT
//            DoAdditionalErrorChecks();
//#endif
        }


        static void DisplayConfigurationDescriptor(Span<byte> ConfigDesc_buf)
        {
            uint uCount = 0;
            bool isSS;
            var ConfigDesc = MemoryMarshal.Read<USB_CONFIGURATION_DESCRIPTOR>(ConfigDesc_buf);
            
            //isSS = info->ConnectionInfoV2
            //       && (info->ConnectionInfoV2->Flags.DeviceIsOperatingAtSuperSpeedOrHigher ||
            //           info->ConnectionInfoV2->Flags.DeviceIsOperatingAtSuperSpeedPlusOrHigher)
            //       ? true
            //       : false;
            //isSS = info->ConnectionInfoV2
            //       && (info->ConnectionInfoV2->Flags.DeviceIsOperatingAtSuperSpeedOrHigher ||
            //           info->ConnectionInfoV2->Flags.DeviceIsOperatingAtSuperSpeedPlusOrHigher)
            //       ? true
            //       : false;

            //AppendTextBuffer("\r\n          ===>Configuration Descriptor<===\r\n");
            //@@DisplayConfigurationDescriptor - Configuration Descriptor

            //length checked in DisplayConfigDesc()
            StringBuilder strb = new();
            strb.AppendLine($"bLength:0x{ConfigDesc.bLength:XX}");
            //AppendTextBuffer("bLength:                           0x%02X\r\n", ConfigDesc.bLength);


            strb.AppendLine($"bDescriptorType:0x{ConfigDesc.bDescriptorType:XX}");
            //AppendTextBuffer("bDescriptorType:                   0x%02X\r\n",
            //    ConfigDesc.bDescriptorType);

            //@@TestCase A4.1
            //@@Priority 1
            //@@Descriptor Field - wTotalLength
            //@@Verify Configuration length is valid
            //AppendTextBuffer("wTotalLength:                    0x%04X", ConfigDesc.wTotalLength);
            strb.AppendLine($"wTotalLength:0x{ConfigDesc.bDescriptorType:XXXX}");

            //uCount = GetConfigurationSize(ConfigDesc_buf);
            if (uCount != ConfigDesc.wTotalLength)
            {
                //AppendTextBuffer("\r\n*!*ERROR: Invalid total configuration size 0x%02X, should be 0x%02X\r\n",
                //    ConfigDesc.wTotalLength, uCount);
            }
            else
            {
                //AppendTextBuffer("  -> Validated\r\n");
            }

            //@@TestCase A4.2
            //@@Priority 1
            //@@Descriptor Field - bNumInterfaces
            //@@Verify the number of interfaces is valid
            //AppendTextBuffer("bNumInterfaces:                    0x%02X\r\n",
            //    ConfigDesc.bNumInterfaces);

            /* Need to check spec vs composite devices
                uCount = GetInterfaceCount(info);
                if (uCount != ConfigDesc->bNumInterfaces) {
                    AppendTextBuffer("\r\n*!*ERROR: Invalid total Interfaces %d, should be %d\r\n",
                        ConfigDesc->bNumInterfaces, uCount);
                } else {
                    AppendTextBuffer("  -> Validated\r\n");
                }
            */

            //AppendTextBuffer("bConfigurationValue:               0x%02X\r\n",
            //    ConfigDesc.bConfigurationValue);

            if (ConfigDesc.bConfigurationValue != 1)
            {
                //@@TestCase A4.3
                //@@CAUTION
                //@@Descriptor Field - bConfigurationValue
                //@@Most host controllers do not handle more than one configuration
                //AppendTextBuffer("*!*CAUTION:    Most host controllers will only work with one configuration per speed\r\n");
                //OOPS();
            }

            //AppendTextBuffer("iConfiguration:                    0x%02X\r\n",
            //    ConfigDesc.iConfiguration);

            strb.AppendLine($"iConfiguration:0x{ConfigDesc.iConfiguration:XX}");

            //if (ConfigDesc.iConfiguration && gDoAnnotation)
            //{
            //    DisplayStringDescriptor(ConfigDesc.iConfiguration,
            //        StringDescs,
            //        info->DeviceInfoNode != NULL ? info->DeviceInfoNode->LatestDevicePowerState : PowerDeviceUnspecified);
            //}

            //AppendTextBuffer("bmAttributes:                      0x%02X",
            //    ConfigDesc.bmAttributes);
            strb.AppendLine($"bmAttributes:0x{ConfigDesc.bmAttributes:XX}");
            //if (info->ConnectionInfo->DeviceDescriptor.bcdUSB == 0x0100)
            //{
            //    if (ConfigDesc.bmAttributes & USB_CONFIG_SELF_POWERED)
            //    {
            //        if (gDoAnnotation)
            //        {
            //            AppendTextBuffer("  -> Self Powered\r\n");
            //        }
            //    }
            //    if (ConfigDesc.bmAttributes & USB_CONFIG_BUS_POWERED)
            //    {
            //        if (gDoAnnotation)
            //        {
            //            AppendTextBuffer("  -> Bus Powered\r\n");
            //        }
            //    }
            //}
            //else
            //{
            //    if (ConfigDesc.bmAttributes & USB_CONFIG_SELF_POWERED)
            //    {
            //        if (gDoAnnotation)
            //        {
            //            AppendTextBuffer("  -> Self Powered\r\n");
            //        }
            //    }
            //    else
            //    {
            //        if (gDoAnnotation)
            //        {
            //            AppendTextBuffer("  -> Bus Powered\r\n");
            //        }
            //    }
            //    if ((ConfigDesc.bmAttributes & USB_CONFIG_BUS_POWERED) == 0)
            //    {
            //        AppendTextBuffer("\r\n*!*ERROR:    Bit 7 is reserved and must be set\r\n");
            //        OOPS();
            //    }
            //}

            //if (ConfigDesc.bmAttributes & USB_CONFIG_REMOTE_WAKEUP)
            //{
            //    if (gDoAnnotation)
            //    {
            //        AppendTextBuffer("  -> Remote Wakeup\r\n");
            //    }
            //}

            //if (ConfigDesc.bmAttributes & USB_CONFIG_RESERVED)
            //{
            //    //@@TestCase A4.4
            //    //@@WARNING
            //    //@@Descriptor Field - bmAttributes
            //    //@@A bit has been set in reserved space
            //    AppendTextBuffer("\r\n*!*ERROR:    Bits 4...0 are reserved\r\n");
            //    OOPS();
            //}

            //AppendTextBuffer("MaxPower:                          0x%02X",
            //    ConfigDesc.MaxPower);
            strb.AppendLine($"MaxPower:0x{ConfigDesc.MaxPower:XX}");
            strb.AppendLine($"MaxPower:0x{ConfigDesc.MaxPower*2}mA");
            //if (gDoAnnotation)
            //{
            //    AppendTextBuffer(" = %3d mA\r\n",
            //        isSS ? ConfigDesc.MaxPower * 8 : ConfigDesc.MaxPower * 2);
            //}
            //else { AppendTextBuffer("\r\n"); }

        }

        static uint GetConfigurationSize(Span<byte> ConfigDesc_buf)
        {
            var ConfigDesc = MemoryMarshal.Read<USB_CONFIGURATION_DESCRIPTOR>(ConfigDesc_buf);
            //USB_COMMON_DESCRIPTOR
            //    commonDesc = (PUSB_COMMON_DESCRIPTOR)ConfigDesc;
            //PUCHAR descEnd = (PUCHAR)ConfigDesc + ConfigDesc->wTotalLength;
            uint uCount = 0;
            var span = ConfigDesc_buf[Marshal.SizeOf<USB_CONFIGURATION_DESCRIPTOR>()..];
            //USB_COMMON_DESCRIPTOR
            //    commonDesc = (PUSB_COMMON_DESCRIPTOR)ConfigDesc;


            USB_COMMON_DESCRIPTOR commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(span);
            while (span.Length > 0)
            {
                commonDesc = MemoryMarshal.Read<USB_COMMON_DESCRIPTOR>(span);
                
                uCount += commonDesc.bLength;
                var offset = Marshal.SizeOf<USB_COMMON_DESCRIPTOR>() + commonDesc.bLength;
                span = span[offset..];
            }

            //// return this device configuration's total sum of descriptor lengths
            //while ((PUCHAR)commonDesc + sizeof(USB_COMMON_DESCRIPTOR) < descEnd &&
            //    (PUCHAR)commonDesc + commonDesc.bLength <= descEnd)
            //{
            //    uCount += commonDesc.bLength;
            //    commonDesc = (USB_COMMON_DESCRIPTOR)((PUCHAR)commonDesc + commonDesc.bLength);
            //}
            return (uCount);
        }

        // Video sub-classes
        const byte SUBCLASS_UNDEFINED = 0x00;
        const byte VIDEO_SUBCLASS_CONTROL = 0x01;
        const byte VIDEO_SUBCLASS_STREAMING = 0x02;

        // Video Class-Specific Descriptor Types
        const byte CS_UNDEFINED = 0x20;
        const byte CS_DEVICE = 0x21;
        const byte CS_CONFIGURATION = 0x22;
        const byte CS_STRING = 0x23;
        const byte CS_INTERFACE = 0x24;
        const byte CS_ENDPOINT = 0x25;

        // Video Class-Specific VC Interface Descriptor Subtypes
        const byte VC_HEADER = 0x01;
        const byte INPUT_TERMINAL = 0x02;
        const byte OUTPUT_TERMINAL = 0x03;
        const byte SELECTOR_UNIT = 0x04;
        const byte PROCESSING_UNIT = 0x05;
        const byte EXTENSION_UNIT = 0x06;
        const byte MAX_TYPE_UNIT = 0x07;

        

        // Video Specific Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VIDEO_SPECIFIC
        {
            byte bLength;              // Size of this descriptor in bytes
            public byte bDescriptorType;      // CS_INTERFACE descriptor type
            public byte bDescriptorSubtype;   // descriptor subtype
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_INTERFACE_DESCRIPTOR2
        {
            byte bLength;             // offset 0, size 1
            byte bDescriptorType;     // offset 1, size 1
            byte bInterfaceNumber;    // offset 2, size 1
            byte bAlternateSetting;   // offset 3, size 1
            byte bNumEndpoints;       // offset 4, size 1
            byte bInterfaceClass;     // offset 5, size 1
            byte bInterfaceSubClass;  // offset 6, size 1
            byte bInterfaceProtocol;  // offset 7, size 1
            byte iInterface;          // offset 8, size 1
            ushort wNumClasses;         // offset 9, size 2
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_INTERFACE_DESCRIPTOR
        {
            byte bLength;
            byte bDescriptorType;
            byte bInterfaceNumber;
            byte bAlternateSetting;
            byte bNumEndpoints;
            public byte bInterfaceClass;
            public byte bInterfaceSubClass;
            public byte bInterfaceProtocol;
            byte iInterface;
        };


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_DEVICE_QUALIFIER_DESCRIPTOR
        {
            byte bLength;
            byte bDescriptorType;
            ushort bcdUSB;
            byte bDeviceClass;
            byte bDeviceSubClass;
            byte bDeviceProtocol;
            byte bMaxPacketSize0;
            byte bNumConfigurations;
            byte bReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_COMMON_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
        };

        struct structstr
        {
            public int Length;
            public char name;
        }
        static string GetString(this SafeFileHandle src, uint ioctl)
        {
            
            int aa = 0;
            structstr ss = new structstr();
            var buf =MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref ss, 1));
            var success = DeviceIoControl(src, ioctl, Span<byte>.Empty, 0, buf, (uint)buf.Length, out var sz, IntPtr.Zero);
            buf = stackalloc byte[ss.Length];
            success = DeviceIoControl(src, ioctl, Span<byte>.Empty, 0, buf, (uint)buf.Length, out  sz, IntPtr.Zero);
            var str1 = MemoryMarshal.Cast<byte, char>(buf[4..^2]);
            var str = new string(str1);
            return str;
        }


        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ReadOnlySpan<byte> lpInBuffer, uint nInBufferSize, Span<byte> lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        /// <summary>
        /// 對應 C 語言中的 USB_CONFIGURATION_DESCRIPTOR
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_CONFIGURATION_DESCRIPTOR
        {
            /// <summary>
            /// 描述符的大小，單位為 byte (固定為 9)
            /// </summary>
            public byte bLength;

            /// <summary>
            /// 描述符類型 (USB_CONFIGURATION_DESCRIPTOR_TYPE，值為 0x02)
            /// </summary>
            public byte bDescriptorType;

            /// <summary>
            /// 此配置回傳的總長度 (包含所有接口、端點描述符)
            /// </summary>
            public ushort wTotalLength;

            /// <summary>
            /// 此配置支援的接口數量
            /// </summary>
            public byte bNumInterfaces;

            /// <summary>
            /// SetConfiguration() 請求所使用的配置值
            /// </summary>
            public byte bConfigurationValue;

            /// <summary>
            /// 描述此配置的字串描述符索引 (iConfiguration)
            /// </summary>
            public byte iConfiguration;

            /// <summary>
            /// 設備屬性 (例如：匯流排供電、遠端喚醒)
            /// </summary>
            public byte bmAttributes;

            /// <summary>
            /// 設備在此配置下從匯流排獲取最大電流，單位為 2mA
            /// </summary>
            public byte MaxPower;
        }
        const byte USB_DEVICE_DESCRIPTOR_TYPE = 0x01;
        const byte USB_CONFIGURATION_DESCRIPTOR_TYPE = 0x02;
        const byte USB_STRING_DESCRIPTOR_TYPE = 0x03;
        const byte USB_INTERFACE_DESCRIPTOR_TYPE = 0x04;
        const byte USB_ENDPOINT_DESCRIPTOR_TYPE = 0x05;

        const byte USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE = 0x07;
        const byte USB_INTERFACE_POWER_DESCRIPTOR_TYPE  =          0x08;
        const byte USB_OTG_DESCRIPTOR_TYPE = 0x09;
        const byte USB_DEBUG_DESCRIPTOR_TYPE = 0x0A;
        const byte USB_IAD_DESCRIPTOR_TYPE = 0x0B;

        const byte USB_CDC_DATA_INTERFACE = 0x0A;
        const byte USB_CHIP_SMART_CARD_INTERFACE = 0x0B;
        const byte USB_CONTENT_SECURITY_INTERFACE = 0x0D;
        const byte USB_DIAGNOSTIC_DEVICE_INTERFACE = 0xDC;
        const byte USB_WIRELESS_CONTROLLER_INTERFACE = 0xE0;
        const byte USB_APPLICATION_SPECIFIC_INTERFACE = 0xFE;

        const byte USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE = 0x06;
        const byte EUSB2_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR_TYPE = 0x12;
        const byte USB_HID_DESCRIPTOR_TYPE = 0x21;


        const byte USB_DEVICE_CLASS_RESERVED = 0x00;
        const byte USB_DEVICE_CLASS_AUDIO = 0x01;
        const byte USB_DEVICE_CLASS_COMMUNICATIONS = 0x02;
        const byte USB_DEVICE_CLASS_HUMAN_INTERFACE = 0x03;
        const byte USB_DEVICE_CLASS_MONITOR = 0x04;
        const byte USB_DEVICE_CLASS_PHYSICAL_INTERFACE = 0x05;
        const byte USB_DEVICE_CLASS_POWER = 0x06;
        const byte USB_DEVICE_CLASS_IMAGE = 0x06;
        const byte USB_DEVICE_CLASS_PRINTER = 0x07;
        const byte USB_DEVICE_CLASS_STORAGE = 0x08;
        const byte USB_DEVICE_CLASS_HUB = 0x09;
        const byte USB_DEVICE_CLASS_CDC_DATA = 0x0A;
        const byte USB_DEVICE_CLASS_SMART_CARD = 0x0B;
        const byte USB_DEVICE_CLASS_CONTENT_SECURITY = 0x0D;
        const byte USB_DEVICE_CLASS_VIDEO = 0x0E;
        const byte USB_DEVICE_CLASS_PERSONAL_HEALTHCARE = 0x0F;
        const byte USB_DEVICE_CLASS_AUDIO_VIDEO = 0x10;
        const byte USB_DEVICE_CLASS_BILLBOARD = 0x11;
        const byte USB_DEVICE_CLASS_DIAGNOSTIC_DEVICE = 0xDC;
        const byte USB_DEVICE_CLASS_WIRELESS_CONTROLLER = 0xE0;
        const byte USB_DEVICE_CLASS_MISCELLANEOUS = 0xEF;
        const byte USB_DEVICE_CLASS_APPLICATION_SPECIFIC = 0xFE;
        const byte USB_DEVICE_CLASS_VENDOR_SPECIFIC = 0xFF;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SetupPacket
        {
            byte bmRequest;
            byte bRequest;
            public ushort wValue;
            ushort wIndex;
            public ushort wLength;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct USB_DESCRIPTOR_REQUEST
        {
            public uint ConnectionIndex;
            public SetupPacket SetupPacket;
            //byte Data;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UsbSetupPacket
        {
            public byte bmRequest;
            public byte bRequest;
            public ushort wValue;
            public ushort wIndex;
            public ushort wLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UsbDescriptorRequest
        {
            public uint ConnectionIndex;
            public UsbSetupPacket SetupPacket;
            public char Data;
        }
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

        enum DEVICE_POWER_STATE
        {
            PowerDeviceUnspecified = 0,
            PowerDeviceD0,
            PowerDeviceD1,
            PowerDeviceD2,
            PowerDeviceD3,
            PowerDeviceMaximum
        };

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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public enum USB_HUB_NODE
        {
            UsbHub,
            UsbMIParent
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_HUB_INFORMATION
        {

            public USB_HUB_DESCRIPTOR HubDescriptor;
            public bool HubIsBusPowered;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_MI_PARENT_INFORMATION
        {
            uint NumberOfInterfaces;
        };
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct USB_NODE_INFORMATION
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
