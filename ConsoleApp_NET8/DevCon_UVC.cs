using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    static public partial class DevConExtensiona
    {
        bool DisplayVCHeader(Span<byte> VCInterfaceDesc)
        {
            //@@DisplayVCHeader -Video Control Interface Header
            uint i = 0;
            uint uSize = 0;
            //PUCHAR pData = NULL;

            //AppendTextBuffer("\r\n          ===>Class-Specific Video Control Interface Header "\

            //    "Descriptor<===\r\n");
            //AppendTextBuffer("bLength:                           0x%02X\r\n", VCInterfaceDesc->bLength);
            //AppendTextBuffer("bDescriptorType:                   0x%02X\r\n", VCInterfaceDesc->bDescriptorType);
            //AppendTextBuffer("bDescriptorSubtype:                0x%02X\r\n", VCInterfaceDesc->bDescriptorSubtype);
            //if (UVC10 == g_chUVCversion)
            //{
            //    AppendTextBuffer("bcdVDC:                          0x%04X\r\n", VCInterfaceDesc->bcdVideoSpec);
            //}
            //else
            //{
            //    AppendTextBuffer("bcdUVC:                          0x%04X\r\n", VCInterfaceDesc->bcdVideoSpec);
            //}
            //AppendTextBuffer("wTotalLength:                    0x%04X", VCInterfaceDesc->wTotalLength);

            // Verify the total interface size (size of this header and all descriptors
            //   following until and not including the first endpoint)
            //uSize = GetVCInterfaceSize(VCInterfaceDesc);
            //if (uSize != VCInterfaceDesc->wTotalLength)
            //{
            //    AppendTextBuffer("\r\n*!*ERROR: Invalid total interface size 0x%02X, should be 0x%02X\r\n",
            //        VCInterfaceDesc->wTotalLength, uSize);
            //}
            //else
            //{
            //    //AppendTextBuffer("  -> Validated\r\n");
            //}
            //AppendTextBuffer("dwClockFreq:                 0x%08X",
            //    VCInterfaceDesc->dwClockFreq);
            //if (gDoAnnotation)
            //{
            //    AppendTextBuffer(" = (%d) Hz", VCInterfaceDesc->dwClockFreq);
            //}
            //AppendTextBuffer("\r\nbInCollection:                     0x%02X\r\n",
            //    VCInterfaceDesc->bInCollection);

            // baInterfaceNr is a variable length field
            // Size is in bInCollection
            //for (i = 1, pData = (PUCHAR) & VCInterfaceDesc->bInCollection;
            //    i <= VCInterfaceDesc->bInCollection; i++, pData++)
            //{
            //    AppendTextBuffer("baInterfaceNr[%d]:                  0x%02X\r\n",
            //        i, *pData);
            //}

            //uSize = (sizeof(VIDEO_CONTROL_HEADER_UNIT) + VCInterfaceDesc->bInCollection);
            //if (VCInterfaceDesc->bLength != uSize)
            {
                //@@TestCase B2.1 (also in Descript.c)
                //@@ERROR
                //@@Descriptor Field - bLength
                //@@The declared length in the device descriptor is less than required length in
                //@@  the USB Video Device Specification
                //AppendTextBuffer("*!*ERROR:  bLength of %d incorrect, should be %d\r\n",
                //    VCInterfaceDesc->bLength, uSize);
                //OOPS();
            }

            //@@TestCase B2.2 (also in Descript.c)
            //@@WARNING
            //@@Descriptor Field - bcdVDC
            //@@The bcdVDC version of the device is not the same as the version of used by USBView
            //if (VCInterfaceDesc->bcdVideoSpec < BCDVDC)
            {
                //AppendTextBuffer("*!*WARNING: This device is set to the old USB Video "\

                //    "Class spec version 0x%04X\r\n", VCInterfaceDesc->bcdVideoSpec);
                //OOPS();
            }

            //if (VCInterfaceDesc->dwClockFreq < 1)
            {
                //@@TestCase B2.3 (Descript.c Line 70)
                //@@WARNING
                //@@dwClockFrequency should be greater than 0
                //@@Question should we check that any non-zero value is accurate
                //AppendTextBuffer("*!*ERROR:  dwClockFreq must be non-zero\r\n");
                //OOPS();
            }

            //@@TestCase B2.4
            //@@Not yet implemented - Priority 1
            //@@Descriptor Field - baInterfaceNr
            //@@We should test to verify each interface number is valid?
            //    for (i=0; i<VCInterfaceDesc->bInCollection; i++)
            //      {AppendTextBuffer("baInterfaceNr[%d]:                  0x%02X\r\n", i+1,
            //        VCInterfaceDesc->baInterfaceNr[i]);}


//            if (gDoAnnotation)
//            {
//                switch (g_chUVCversion)
//                {
//                    case UVC10:
//                        //AppendTextBuffer("USB Video Class device: spec version 1.0\r\n");
//                        break;
//                    case UVC11:
//                        //AppendTextBuffer("USB Video Class device: spec version 1.1\r\n");
//                        break;
//# if H264_SUPPORT
//                    case UVC15:
//                        AppendTextBuffer("USB Video Class device: spec version 1.5\r\n");
//                        break;
//#endif

//                    default:
//                        break;
//                }
//            }
            return true;
        }


        static bool DisplayVideoDescriptor(Span<byte> VidCommonDesc_buf, byte bInterfaceSubClass, StringBuilder StringDescs, DEVICE_POWER_STATE LatestDevicePowerState)
        {
            var VidCommonDesc = MemoryMarshal.Read<VIDEO_SPECIFIC>(VidCommonDesc_buf);
            //@@DisplayVideoDescriptor -Class-Specific Video Descriptor
            switch (VidCommonDesc.bDescriptorType)
            {
                case CS_INTERFACE:
                    //@@DisplayVideoDescriptor -Class-Specific Video Interface Descriptor
                    switch (bInterfaceSubClass)
                    {
                        case VIDEO_SUBCLASS_CONTROL:
                            //@@DisplayVideoDescriptor -Class-Specific Video Control Interface Descriptor
                            switch (VidCommonDesc.bDescriptorSubtype)
                            {
                                case VC_HEADER:
                                    //return DisplayVCHeader(
                                    //    (PVIDEO_CONTROL_HEADER_UNIT)VidCommonDesc);

                                case INPUT_TERMINAL:
                                //return DisplayVCInputTerminal(
                                //    (PVIDEO_INPUT_TERMINAL)VidCommonDesc,
                                //    StringDescs,
                                //    LatestDevicePowerState);

                                case OUTPUT_TERMINAL:
                                //return DisplayVCOutputTerminal(
                                //    (PVIDEO_OUTPUT_TERMINAL)VidCommonDesc,
                                //    StringDescs,
                                //    LatestDevicePowerState);

                                case SELECTOR_UNIT:
                                //return DisplayVCSelectorUnit(
                                //    (PVIDEO_SELECTOR_UNIT)VidCommonDesc,
                                //    StringDescs,
                                //    LatestDevicePowerState);

                                case PROCESSING_UNIT:
                                //return DisplayVCProcessingUnit(
                                //    (PVIDEO_PROCESSING_UNIT)VidCommonDesc,
                                //    StringDescs,
                                //    LatestDevicePowerState);

                                case EXTENSION_UNIT:
                                //return DisplayVCExtensionUnit(
                                //    (PVIDEO_EXTENSION_UNIT)VidCommonDesc,
                                //    StringDescs,
                                //    LatestDevicePowerState);

#if H264_SUPPORT
                                case H264_ENCODING_UNIT:
                                    return DisplayVCH264EncodingUnit(
                                        (PVIDEO_ENCODING_UNIT)VidCommonDesc
                                        );

#endif

#if H264_SUPPORT
                                case MAX_TYPE_UNIT + 1:
                                // for H.264, the bDescriptorSubtype = 7, which is equal to MAX_TYPE_UNIT
                                // so now MAX_TYPE_UNIT needs to be set to 8
                                //(TODO: need to change nt\sdpublic\internal\drivers\inc\uvcdesc.h's define
                                // of MAX_TYPE_UNIT from7 to 8, and ad the type for H.264 = 8)
#else
                                case MAX_TYPE_UNIT:
#endif
                                    //@@TestCase B1.1
                                    //@@CAUTION
                                    //@@Descriptor Field - bDescriptorSubtype
                                    //@@An undefined descriptor subtype has been defined
                                    //AppendTextBuffer("*!*CAUTION:  This is an undefined class specific "\

                                    //    "Video Control bDescriptorSubtype\r\n");
                                    break;

                                default:
                                    //@@TestCase B1.2
                                    //@@ERROR
                                    //@@Descriptor Field - bDescriptorSubtype
                                    //@@An unknown descriptor subtype has been defined
                                    //AppendTextBuffer("*!*ERROR:  unknown bDescriptorSubtype\r\n");
                                    //OOPS();
                                    break;
                            }
                            break;

                        case VIDEO_SUBCLASS_STREAMING:
                            //@@DisplayVideoDescriptor -Class-Specific Video Streaming Interface Descriptor
                            switch (VidCommonDesc.bDescriptorSubtype)
                            {
                                case VS_INPUT_HEADER:
                                //return DisplayVidInHeader(
                                //    (PVIDEO_STREAMING_INPUT_HEADER)VidCommonDesc);

                                case VS_OUTPUT_HEADER:
                                //return DisplayVidOutHeader(
                                //    (PVIDEO_STREAMING_OUTPUT_HEADER)VidCommonDesc);

                                case VS_STILL_IMAGE_FRAME:
                                //return DisplayStillImageFrame(
                                //    (PVIDEO_STILL_IMAGE_FRAME)VidCommonDesc);

                                case VS_FORMAT_UNCOMPRESSED:
#if H264_SUPPORT
                                    {
                                        BOOL retCode = DisplayUncompressedFormat((PVIDEO_FORMAT_UNCOMPRESSED)VidCommonDesc);
                                        g_expectedNumberOfUncompressedFrameFrameDescriptors += ((PVIDEO_FORMAT_UNCOMPRESSED)VidCommonDesc)->bNumFrameDescriptors;
                                        return retCode;
                                    }
#else
                                //return DisplayUncompressedFormat(
                                //    (PVIDEO_FORMAT_UNCOMPRESSED)VidCommonDesc);
#endif

                                case VS_FRAME_UNCOMPRESSED:
#if H264_SUPPORT
                                    {
                                        BOOL retCode = DisplayUncompressedFrameType((PVIDEO_FRAME_UNCOMPRESSED)VidCommonDesc);
                                        g_numberOfUncompressedFrameFrameDescriptors++;
                                        return retCode;
                                    }
#else
                                //return DisplayUncompressedFrameType(
                                //    (PVIDEO_FRAME_UNCOMPRESSED)VidCommonDesc);
#endif

#if H264_SUPPORT
                                case VS_FORMAT_H264:
                                    {
                                        BOOL retCode = DisplayVCH264Format((PVIDEO_FORMAT_H264)VidCommonDesc);
                                        g_expectedNumberOfH264FrameDescriptors += ((PVIDEO_FORMAT_H264)VidCommonDesc)->bNumFrameDescriptors;
                                        return retCode;
                                    }

                                case VS_FRAME_H264:
                                    {
                                        BOOL retCode = DisplayVCH264FrameType((PVIDEO_FRAME_H264)VidCommonDesc);
                                        g_numberOfH264FrameDescriptors++;
                                        return retCode;
                                    }
#endif

                                case VS_FORMAT_MJPEG:
#if H264_SUPPORT // additional checks
                                    {
                                        BOOL retCode = DisplayMJPEGFormat((PVIDEO_FORMAT_MJPEG)VidCommonDesc);
                                        g_expectedNumberOfMJPEGFrameDescriptors += ((PVIDEO_FORMAT_MJPEG)VidCommonDesc)->bNumFrameDescriptors;
                                        return retCode;
                                    }
#else
                                //return DisplayMJPEGFormat(
                                //    (PVIDEO_FORMAT_MJPEG)VidCommonDesc);
#endif

                                case VS_FRAME_MJPEG:
#if H264_SUPPORT
                                    {
                                        BOOL retCode = DisplayMJPEGFrameType((PVIDEO_FRAME_MJPEG)VidCommonDesc);
                                        g_numberOfMJPEGFrameDescriptors++;
                                        return retCode;
                                    }

#else
                                //return DisplayMJPEGFrameType(
                                //    (PVIDEO_FRAME_MJPEG)VidCommonDesc);
#endif



                                case VS_FORMAT_MPEG1:
                                //{
                                //if (UVC10 == g_chUVCversion)
                                //{
                                //    return DisplayMPEG1SSFormat(
                                //        (PVIDEO_FORMAT_MPEG1SS)VidCommonDesc);
                                //}
                                //else // this format is obsoleted in UVC version >= 1.1
                                //{
                                //    AppendTextBuffer("*!*ERROR:  obsoleted bDescriptorSubtype\r\n");
                                //    OOPS();
                                //    break;
                                //}
                                //}

                                case VS_FORMAT_MPEG2PS:
                                //{
                                //if (UVC10 == g_chUVCversion)
                                //{
                                //    return DisplayMPEG2PSFormat(
                                //        (PVIDEO_FORMAT_MPEG2PS)VidCommonDesc);
                                //}
                                //else // this format is obsoleted in UVC version >= 1.1
                                //{
                                //    AppendTextBuffer("*!*ERROR:  obsoleted bDescriptorSubtype\r\n");
                                //    OOPS();
                                //    break;
                                //}
                                //}

                                case VS_FORMAT_MPEG2TS:
                                //return DisplayMPEG2TSFormat(
                                //    (PVIDEO_FORMAT_MPEG2TS)VidCommonDesc);

                                case VS_FORMAT_MPEG4SL:
                                //{
                                //if (UVC10 == g_chUVCversion)
                                //{
                                //    return DisplayMPEG4SLFormat(
                                //        (PVIDEO_FORMAT_MPEG4SL)VidCommonDesc);
                                //}
                                //else // this format is obsoleted in UVC version >= 1.1
                                //{
                                //    AppendTextBuffer("*!*ERROR:  obsoleted bDescriptorSubtype\r\n");
                                //    OOPS();
                                //    break;
                                //}
                                //}

                                case VS_FORMAT_DV:
                                //return DisplayDVFormat(
                                //    (PVIDEO_FORMAT_DV)VidCommonDesc);

                                case VS_COLORFORMAT:
                                //return DisplayColorMatching(
                                //    (PVIDEO_COLORFORMAT)VidCommonDesc);

                                case VS_FORMAT_VENDOR:
                                //{
                                //if (UVC10 == g_chUVCversion)
                                //{
                                //    return DisplayVendorVidFormat(
                                //       (PVIDEO_FORMAT_VENDOR)VidCommonDesc);
                                //}
                                //else // this format is obsoleted in UVC version >= 1.1
                                //{
                                //    AppendTextBuffer("*!*ERROR:  obsoleted bDescriptorSubtype\r\n");
                                //    OOPS();
                                //    break;
                                //}
                                //}

                                case VS_FRAME_VENDOR:
                                //{
                                //if (UVC10 == g_chUVCversion)
                                //{
                                //    return DisplayVendorVidFrameType(
                                //        (PVIDEO_FRAME_VENDOR)VidCommonDesc);
                                //}
                                //else // this format is obsoleted in UVC version >= 1.1
                                //{
                                //    AppendTextBuffer("*!*ERROR:  obsoleted bDescriptorSubtype\r\n");
                                //    OOPS();
                                //    break;
                                //}
                                //}

                                case VS_FORMAT_FRAME_BASED:
                                //{
                                //    if (UVC10 != g_chUVCversion)
                                //    {
                                //        return DisplayFramePayloadFormat(
                                //            (PVIDEO_FORMAT_FRAME)VidCommonDesc);
                                //    }
                                //    else // this format did not exist in UVC 1.0
                                //    {
                                //        AppendTextBuffer("*!*ERROR: bDescriptorSubtype did not exist in UVC 1.0\r\n");
                                //        OOPS();
                                //        break;
                                //    }
                                //}

                                case VS_FRAME_FRAME_BASED:
                                //{
                                //    if (UVC10 != g_chUVCversion)
                                //    {
                                //        return DisplayFramePayloadFrame(
                                //            (PVIDEO_FRAME_FRAME)VidCommonDesc);
                                //    }
                                //    else // this format did not exist in UVC 1.0
                                //    {
                                //        AppendTextBuffer("*!*ERROR: bDescriptorSubtype did not exist in UVC 1.0\r\n");
                                //        OOPS();
                                //        break;
                                //    }
                                //}

                                case VS_FORMAT_STREAM_BASED:
                                //{
                                //    if (UVC10 != g_chUVCversion)
                                //    {
                                //        return DisplayStreamPayload(
                                //            (PVIDEO_FORMAT_STREAM)VidCommonDesc);
                                //    }
                                //    else // this format did not exist in UVC 1.0
                                //    {
                                //        AppendTextBuffer("*!*ERROR: bDescriptorSubtype did not exist in UVC 1.0\r\n");
                                //        OOPS();
                                //        break;
                                //    }
                                //}

                                case VS_DESCRIPTOR_UNDEFINED:
                                    //@@TestCase B1.3
                                    //@@CAUTION
                                    //@@Descriptor Field - bDescriptorSubtype
                                    //@@An undefined descriptor subtype has been defined
                                    //AppendTextBuffer("*!*CAUTION:  This is an undefined class specific Video "\

                                    //    "Streaming bDescriptorSubtype\r\n");
                                    break;

                                default:
                                    //@@TestCase B1.4
                                    //@@ERROR
                                    //@@Descriptor Field - bDescriptorSubtype
                                    //@@An unknown descriptor subtype has been defined
                                    //AppendTextBuffer("*!*ERROR:  unknown bDescriptorSubtype\r\n");
                                    //OOPS();
                                    break;
                            }
                            break;

                        default:
                            //@@TestCase B1.6
                            //@@ERROR
                            //@@Descriptor Field - bInterfaceSubClass
                            //@@An unknown interface sub-class has been defined
                            //AppendTextBuffer("*!*ERROR:  unknown bInterfaceSubClass\r\n");
                            //OOPS();
                            break;
                    }
                    break;

                case CS_ENDPOINT:
                    //@@DisplayVideoDescriptor -Class-Specific Video Endpoint Descriptor
                    switch (VidCommonDesc.bDescriptorSubtype)
                    {
                        //@@TestCase B1.7
                        //@@CAUTION
                        //@@Descriptor Field - bInterfaceSubtype
                        //@@An undefined descriptor subtype has been defined
                        case EP_UNDEFINED:
                            //AppendTextBuffer("*!*CAUTION:  This is an undefined bDescriptorSubtype\r\n");
                            break;
                        //@@TestCase B1.8
                        //@@Not yet implemented - Priority 3
                        //@@Descriptor Field - bDescriptorSubtype
                        //@@Question:  How valid are VIDEO_EP_GENERAL and VIDEO_EP_ENDPOINT?  Should we test?
                        case EP_GENERAL:
                            break;
                        case EP_ENDPOINT:
                            break;
                        case EP_INTERRUPT:
                            //return DisplayVSEndpoint(
                            //    (PVIDEO_CS_INTERRUPT)VidCommonDesc);
                            break;
                        default:
                            //@@TestCase B1.9
                            //@@ERROR
                            //@@Descriptor Field - bDescriptorSubtype
                            //@@An unknown descriptor subtype has been defined
                            //AppendTextBuffer("*!*CAUTION:  Unknown bDescriptorSubtype");
                            break;
                    }
                    break;
                //@@DisplayVideoDescriptor -Class-Specific Video Device Descriptor
                //@@DisplayVideoDescriptor -Class-Specific Video Configuration Descriptor
                //@@DisplayVideoDescriptor -Class-Specific Video String Descriptor
                //@@DisplayVideoDescriptor -Class-Specific Video Undefined Descriptor
                //@@TestCase B1.10
                //@@Not yet implemented - Priority 3
                //@@Descriptor -Class-Specific Device, Configuration, String, Undefined
                //@@Descriptor Field - bDescriptorType
                //@@Question:  How valid are these Descriptor Types?  Should we test?

                /*        case USB_VIDEO_CS_DEVICE:
                AppendTextBuffer("USB_VIDEO_CS_DEVICE bDescriptorType\r\n");
                break;

                case USB_VIDEO_CS_CONFIGURATION:
                AppendTextBuffer("USB_VIDEO_CS_CONFIGURATION bDescriptorType\r\n");
                break;

                case USB_VIDEO_CS_STRING:
                AppendTextBuffer("USB_VIDEO_CS_STRING bDescriptorType\r\n");
                break;

                case USB_VIDEO_CS_UNDEFINED:
                AppendTextBuffer("USB_VIDEO_CS_UNDEFINED bDescriptorType\r\n");
                break;
                */
                default:
                    ////@@TestCase B1.11
                    ////@@ERROR
                    ////@@Descriptor Field - bDescriptorType
                    ////@@An unknown descriptor type has been defined
                    //AppendTextBuffer("*!*CAUTION:  Unknown bDescriptorSubtype");
                    //OOPS();
                    break;
            }

            return false;
        }

        // global version for USB Video Class spec version (pre-release)
        const ushort BCDVDC = 0x0083;

        // USB Video Class spec version
        const ushort NOT_UVC = 0x0;
        const ushort UVC10 = 0x100;
        const ushort UVC11 = 0x110;

#if H264_SUPPORT
const ushort UVC15 =  0x150;
#endif

        const byte VS_FORMAT_FRAME_BASED = 0x10;
        const byte VS_FRAME_FRAME_BASED = 0x11;
        const byte VS_FORMAT_STREAM_BASED = 0x12;


        // Video Class-Specific VS Interface Descriptor Subtypes
        const byte VS_DESCRIPTOR_UNDEFINED = 0x00;
        const byte VS_INPUT_HEADER = 0x01;
        const byte VS_OUTPUT_HEADER = 0x02;
        const byte VS_STILL_IMAGE_FRAME = 0x03;
        const byte VS_FORMAT_UNCOMPRESSED = 0x04;
        const byte VS_FRAME_UNCOMPRESSED = 0x05;
        const byte VS_FORMAT_MJPEG = 0x06;
        const byte VS_FRAME_MJPEG = 0x07;
        const byte VS_FORMAT_MPEG1 = 0x08;
        const byte VS_FORMAT_MPEG2PS = 0x09;
        const byte VS_FORMAT_MPEG2TS = 0x0A;
        const byte VS_FORMAT_MPEG4SL = 0x0B;
        const byte VS_FORMAT_DV = 0x0C;
        const byte VS_COLORFORMAT = 0x0D;
        const byte VS_FORMAT_VENDOR = 0x0E;
        const byte VS_FRAME_VENDOR = 0x0F;

        // Video Class-Specific Endpoint Descriptor Subtypes
        const byte EP_UNDEFINED = 0x00;
        const byte EP_GENERAL = 0x01;
        const byte EP_ENDPOINT = 0x02;
        const byte EP_INTERRUPT = 0x03;
    }
}
