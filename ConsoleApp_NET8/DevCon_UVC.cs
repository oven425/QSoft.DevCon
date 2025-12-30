using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QSoft.DevCon
{
    static public partial class DevConExtensiona
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VIDEO_CONTROL_HEADER_UNIT
        {
            byte bLength;              // Size of this descriptor in bytes
            byte bDescriptorType;      // CS_INTERFACE descriptor type
            byte bDescriptorSubtype;   // VC_HEADER descriptor subtype
            ushort bcdVideoSpec;        // USB video class spec revision number
            ushort wTotalLength;        // Total length, including all units and terminals
            uint dwClockFreq;          // Device clock frequency in Hz
            byte bInCollection;        // number of video streaming interfaces
            //byte baInterfaceNr[];      // interface number array
        };
        static bool DisplayVCHeader(Span<byte> VCInterfaceDesc)
        {
            var unit = MemoryMarshal.Read<VIDEO_CONTROL_HEADER_UNIT>(VCInterfaceDesc);
            var baInterfaceNr = VCInterfaceDesc.Slice(12);
            return true;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VIDEO_EXTENSION_UNIT
        {
            public byte bLength;              // Size of this descriptor in bytes
            public byte bDescriptorType;      // CS_INTERFACE descriptor type
            public byte bDescriptorSubtype;   // EXTENSION_UNIT descriptor subtype
            public byte bUnitID;              // Constant uniquely identifying the Unit
            public Guid guidExtensionCode;     // Vendor-specific code identifying extension unit
            public byte bNumControls;         // Number of controls in Extension Unit
            public byte bNrInPins;            // Number of input pins
            //UCHAR baSourceID[];         // IDs of connected units/terminals
        };

        static bool DisplayVCExtensionUnit(Span<byte> VidExtensionDesc_buf)
        {
            //https://www.usbzh.com/article/detail-36.html
            var VidExtensionDesc = MemoryMarshal.Read<VIDEO_EXTENSION_UNIT>(VidExtensionDesc_buf);
            byte[] baSourceID = new byte[VidExtensionDesc.bNrInPins];
            var sz = Marshal.SizeOf<VIDEO_EXTENSION_UNIT>();
            if (VidExtensionDesc.bNrInPins > 0)
            {
                VidExtensionDesc_buf.Slice(sz, VidExtensionDesc.bNrInPins).CopyTo(baSourceID);
            }
            sz = sz + baSourceID.Length;
            var bControlSize = VidExtensionDesc_buf[sz];
            sz = sz + 1;
            var aaa = VidExtensionDesc_buf.Slice(sz, bControlSize);
            var bits = new BitArray(aaa.ToArray());

            return true;
        }


        // VideoControl Processing Unit Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VIDEO_PROCESSING_UNIT
        {
            public byte bLength;              // Size of this descriptor in bytes
            byte bDescriptorType;      // CS_INTERFACE descriptor type
            byte bDescriptorSubtype;   // PROCESSING_UNIT descriptor subtype
            public byte bUnitID;              // Constant uniquely identifying the Unit
            public byte bSourceID;            // ID of connected unit/terminal
            ushort wMaxMultiplier;      // Maximum digital magnification
            byte bControlSize;         // Size of bmControls field
            //byte bmControls[];         // Bitmap of controls supported
        };


        static bool DisplayVCProcessingUnit(this Span<byte> VidProcessingDescBuf)
        {
            var VidProcessingDesc = MemoryMarshal.Read<VIDEO_PROCESSING_UNIT>(VidProcessingDescBuf);
            ////@@DisplayVCProcessingUnit -Video Control Processor Unit
            //PUCHAR pData = NULL;
            //UCHAR bLength = 0;

            //AppendTextBuffer("\r\n          ===>Video Control Processing Unit Descriptor<===\r\n");
            //AppendTextBuffer("bLength:                           0x%02X\r\n", VidProcessingDesc->bLength);
            //AppendTextBuffer("bDescriptorType:                   0x%02X\r\n", VidProcessingDesc->bDescriptorType);
            //AppendTextBuffer("bDescriptorSubtype:                0x%02X\r\n", VidProcessingDesc->bDescriptorSubtype);
            //AppendTextBuffer("bUnitID:                           0x%02X\r\n", VidProcessingDesc->bUnitID);
            //AppendTextBuffer("bSourceID:                         0x%02X\r\n", VidProcessingDesc->bSourceID);
            //AppendTextBuffer("wMaxMultiplier:                  0x%04X\r\n", VidProcessingDesc->wMaxMultiplier);
            //AppendTextBuffer("bControlSize:                      0x%02X\r\n", VidProcessingDesc->bControlSize);

            //pData = &VidProcessingDesc->bControlSize;

            //// Are there any controls?
            //if (0 < *pData)
            //{
            //    UINT uBitIndex = 0;
            //    BYTE cCheckBit = 0;
            //    BYTE cMask = 1;

            //    AppendTextBuffer("bmControls : ");
            //    VDisplayBytes(pData + 1, *pData);

            //    // map the first control
            //    for (; uBitIndex < 8; uBitIndex++)
            //    {
            //        cCheckBit = cMask & *(pData + 1);

            //        AppendTextBuffer("     D%02d = %d  %s %s\r\n",
            //            uBitIndex,
            //            cCheckBit ? 1 : 0,
            //            cCheckBit ? "yes - " : " no - ",
            //            GetStringFromList(slProcessorControls1,
            //                sizeof(slProcessorControls1) / sizeof(STRINGLIST),
            //                cMask,
            //                "Invalid PU bmControl value"));

            //        cMask = cMask << 1;
            //    }

            //    // Is there a second control?
            //    if (1 < *pData)
            //    {
            //        // map the second control
            //        for (uBitIndex = 8, cMask = 1; uBitIndex < 16; uBitIndex++)
            //        {
            //            cCheckBit = cMask & *(pData + 2);

            //            AppendTextBuffer("     D%02d = %d  %s %s\r\n",
            //                uBitIndex,
            //                cCheckBit ? 1 : 0,
            //                cCheckBit ? "yes - " : " no - ",
            //                GetStringFromList(slProcessorControls2,
            //                    sizeof(slProcessorControls2) / sizeof(STRINGLIST),
            //                    cMask,
            //                    "Invalid PU bmControl value"));

            //            cMask = cMask << 1;
            //        }
            //    }

            //    // Is there a third control?
            //    if (2 < *pData)
            //    {
            //        // map the third control
            //        for (uBitIndex = 16, cMask = 1; uBitIndex < 24; uBitIndex++)
            //        {
            //            cCheckBit = cMask & *(pData + 3);

            //            AppendTextBuffer("     D%02d = %d  %s %s\r\n",
            //                uBitIndex,
            //                cCheckBit ? 1 : 0,
            //                cCheckBit ? "yes - " : " no - ",
            //                GetStringFromList(slProcessorControls3,
            //                    sizeof(slProcessorControls3) / sizeof(STRINGLIST),
            //                    cMask,
            //                    "Invalid PU bmControl value"));

            //            cMask = cMask << 1;
            //        }
            //    }
            //}

            //// get address of iProcessing
            //if (UVC10 != g_chUVCversion)
            //{
            //    // size of descriptor is struct size plus control size plus 2 if UVC11
            //    bLength = sizeof(VIDEO_PROCESSING_UNIT) + 2 + VidProcessingDesc->bControlSize;
            //    pData = (PUCHAR)VidProcessingDesc + (VidProcessingDesc->bLength - 2);
            //}
            //else // UVC 1.0
            //{
            //    // size of descriptor is struct size plus control size plus 1 if UVC10
            //    bLength = sizeof(VIDEO_PROCESSING_UNIT) + 1 + VidProcessingDesc->bControlSize;
            //    pData = (PUCHAR)VidProcessingDesc + (VidProcessingDesc->bLength - 1);
            //}
            //AppendTextBuffer("iProcessing :                      0x%02X\r\n", *pData);
            //if (gDoAnnotation)
            //{
            //    if (*pData)
            //    {
            //        // if executing this code, the configuration descriptor has been
            //        // obtained.  If a device is suspended, then its configuration
            //        // descriptor was not obtained and we do not want errors to be
            //        // displayed when string descriptors were not obtained.
            //        DisplayStringDescriptor(*pData, StringDescs, LatestDevicePowerState);
            //    }
            //}

            //// check for new UVC 1.1 bmVideoStandards fields
            //if (UVC10 != g_chUVCversion)
            //{
            //    UINT uBitIndex = 0;
            //    BYTE cCheckBit = 0;
            //    BYTE cMask = 1;

            //    pData = (PUCHAR)VidProcessingDesc + (VidProcessingDesc->bLength - 1);

            //    AppendTextBuffer("bmVideoStandards :                 ");
            //    VDisplayBytes(pData, 1);

            //    // map the first control
            //    for (; uBitIndex < 8; uBitIndex++)
            //    {
            //        cCheckBit = cMask & *(pData);

            //        AppendTextBuffer("     D%02d = %d  %s %s\r\n",
            //            uBitIndex,
            //            cCheckBit ? 1 : 0,
            //            cCheckBit ? "yes - " : " no - ",
            //            GetStringFromList(slProcessorVideoStandards,
            //                sizeof(slProcessorVideoStandards) / sizeof(STRINGLIST),
            //                cMask,
            //                "Invalid PU bmVideoStandards value"));

            //        cMask = cMask << 1;
            //    }
            //}

            //if (VidProcessingDesc.bLength != bLength)
            //{
            //    //@@TestCase B9.1 (also in Descript.c)
            //    //@@ERROR
            //    //@@Descriptor Field - bLength
            //    AppendTextBuffer("*!*ERROR:  bLength of 0x%02X incorrect, should be 0x%02X\r\n",
            //        VidProcessingDesc->bLength, bLength);
            //    OOPS();
            //}

            //if (VidProcessingDesc.bUnitID < 1)
            //{
            //    //@@TestCase B9.2 (Descript.c   Line 466)
            //    //@@ERROR
            //    //@@Descriptor Field - bUnitID
            //    //@@bUnitID must be greater than 0
            //    //@@Question: Should we test to verify unit number is unique?
            //    AppendTextBuffer("*!*ERROR:  bUnitID must be non-zero\r\n");
            //    OOPS();
            //}

            //if (VidProcessingDesc.bSourceID < 1)
            //{
            //    //@@TestCase B9.3 (Descript.c   Line 471)
            //    //@@ERROR
            //    //@@Descriptor Field - bSourceID
            //    //@@bSourceID must be non-zero
            //    //@@Question: Should we test to verify the bSourceID is valid?
            //    AppendTextBuffer("*!*ERROR:  bSourceID must be non-zero\r\n");
            //    OOPS();
            //}

            //@@TestCase B9.4
            //@@Not yet implemented - Priority 1
            //@@Descriptor Field - wMaxMultiplier
            //@@We should test to verify multiplier is valid
            //    AppendTextBuffer("wMaxMultiplier:                  0x%04X\r\n", VidProcessingDesc->wMaxMultiplier);

            return true;
        }

        public static void GetUVC(this Span<byte> src, byte interfaceclass, byte interfacesubclass)
        {
            var VidCommonDesc = MemoryMarshal.Read<VIDEO_SPECIFIC>(src);
            switch (VidCommonDesc.bDescriptorType)
            {
                case CS_INTERFACE:
                    switch(interfacesubclass)
                    {
                        case VIDEO_SUBCLASS_CONTROL:
                            switch(VidCommonDesc.bDescriptorSubtype)
                            {
                                case VC_HEADER:
                                    DisplayVCHeader(src);
                                    break;
                                case PROCESSING_UNIT:
                                    src.DisplayVCProcessingUnit();
                                    break;
                                case EXTENSION_UNIT:
                                    DisplayVCExtensionUnit(src);
                                    break;
                            }
                            break;
                        case VIDEO_SUBCLASS_STREAMING:
                            switch(VidCommonDesc.bDescriptorSubtype)
                            {

                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        static bool DisplayVideoDescriptor(Span<byte> VidCommonDesc_buf, byte bInterfaceSubClass, StringBuilder StringDescs, DEVICE_POWER_STATE LatestDevicePowerState)
        {
            var aa = MemoryMarshal.Cast<byte, VIDEO_SPECIFIC>(VidCommonDesc_buf)[0];
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
                                    //return DisplayVCHeader(VidCommonDesc_buf);
                                    break;

                                case INPUT_TERMINAL:
                                    //return DisplayVCInputTerminal(
                                    //    (PVIDEO_INPUT_TERMINAL)VidCommonDesc,
                                    //    StringDescs,
                                    //    LatestDevicePowerState);
                                    break;

                                case OUTPUT_TERMINAL:
                                    //return DisplayVCOutputTerminal(
                                    //    (PVIDEO_OUTPUT_TERMINAL)VidCommonDesc,
                                    //    StringDescs,
                                    //    LatestDevicePowerState);
                                    break;

                                case SELECTOR_UNIT:
                                    //return DisplayVCSelectorUnit(
                                    //    (PVIDEO_SELECTOR_UNIT)VidCommonDesc,
                                    //    StringDescs,
                                    //    LatestDevicePowerState);
                                    break;

                                case PROCESSING_UNIT:
                                    //return DisplayVCProcessingUnit(
                                    //    (PVIDEO_PROCESSING_UNIT)VidCommonDesc,
                                    //    StringDescs,
                                    //    LatestDevicePowerState);
                                    break;

                                case EXTENSION_UNIT:
                                    //var extunit = MemoryMarshal.Read<VIDEO_EXTENSION_UNIT>(VidCommonDesc_buf);
                                    //return DisplayVCExtensionUnit(VidCommonDesc_buf, StringDescs, LatestDevicePowerState);
                                    break;

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


    public struct USB_IAD_DESCRIPTOR
    {
        byte bLength;
        byte bDescriptorType;
        byte bFirstInterface;
        byte bInterfaceCount;
        byte bFunctionClass;
        byte bFunctionSubClass;
        byte bFunctionProtocol;
        byte iFunction;
    };

    
}
