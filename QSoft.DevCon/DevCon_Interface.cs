using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QSoft.DevCon
{
    static public partial class DevConExtension
    {
        public static IEnumerable<(IntPtr dev, SP_DEVICE_INTERFACE_DATA devdata)> Interfaces(this Guid guid, bool showhiddendevice = false)
        {
            uint flags = DIGCF_PRESENT;
            if (showhiddendevice)
            {
                flags = DIGCF_PROFILE;
            }
            flags |= DIGCF_DEVICEINTERFACE;
            //if (guid == Guid.Empty)
            //{
            //    flags |= DIGCF_ALLCLASSES;
            //}


            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, flags);
            try
            {
                while (true)
                {
                    SP_DEVINFO_DATA devinfo = new();
                    devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);

                    SP_DEVICE_INTERFACE_DATA interfaceinfo = new();
                    interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                    //IntPtrMem<SP_DEVICE_INTERFACE_DATA> interfaceinfo = new();
                    //interfaceinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                    if (!SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, guid, index, out interfaceinfo))
                    {
                        var err = Marshal.GetLastWin32Error();
                        yield break;
                    }
                    else
                    {
#if !NET8_0_OR_GREATER
                        
                        var bb = SetupDiGetDeviceInterfaceDetail(hDevInfo, interfaceinfo,  out _, 0, out var reqsize, out _);
                        var err = Marshal.GetLastWin32Error();

#endif
                        yield return (hDevInfo, interfaceinfo);
                    }
                    index++;
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }

    }

    

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public UIntPtr Reserved;
    };

    struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        uint cbSize;
        //char DevicePath[ANYSIZE_ARRAY];
    };
}

//namespace QSoft.DevCon
//{
//    internal class DevCon_Interface
//    {
//        BOOL Enum(USHORT m_Pid, USHORT m_Vid, USHORT m_Pvn, char* str, int* len)

//    DWORD DeviceNum = 0;
//        GUID hidGuid;
//	//获取HID设备的GUID
//	::HidD_GetHidGuid((LPGUID)&hidGuid);

//        HDEVINFO hDevInfoList = SetupDiGetClassDevs(&hidGuid, NULL, NULL, (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));
//	if (hDevInfoList != NULL)
//	{
//		SP_DEVICE_INTERFACE_DATA deviceInfoData;

//        // Clear data structure
//        ZeroMemory(&deviceInfoData, sizeof(deviceInfoData));
//        deviceInfoData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
//		SetLastError(NO_ERROR);
//		while (1)
//		{
//			int ret = GetLastError();
//			if (ret == ERROR_NO_MORE_ITEMS)
//			{
//				break;
//			}
//			ZeroMemory(&deviceInfoData, sizeof(deviceInfoData));
//        deviceInfoData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
//			// retrieves a context structure for a device interface of a device information set.
//			if (SetupDiEnumDeviceInterfaces(hDevInfoList, 0, &hidGuid, DeviceNum, &deviceInfoData))
//			{
//				// Must get the detailed information in two steps
//				// First get the length of the detailed information and allocate the buffer
//				// retrieves detailed information about a specified device interface.
//				PSP_DEVICE_INTERFACE_DETAIL_DATA functionClassDeviceData = NULL;
//        ULONG predictedLength, requiredLength;

//        predictedLength = requiredLength = 0;
//				SetupDiGetDeviceInterfaceDetail(hDevInfoList,
//					&deviceInfoData,
//                    NULL,			// Not yet allocated
//					0,				// Set output buffer length to zero 
//					&requiredLength,// Find out memory requirement
//                    NULL);

//        predictedLength = requiredLength;
//				functionClassDeviceData = (PSP_DEVICE_INTERFACE_DETAIL_DATA) malloc(predictedLength);
//        functionClassDeviceData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

//				SP_DEVINFO_DATA did = { sizeof(SP_DEVINFO_DATA) };

//				// Second, get the detailed information
//				if (SetupDiGetDeviceInterfaceDetail(hDevInfoList,
//					&deviceInfoData,
//					functionClassDeviceData,
//					predictedLength,
//					&requiredLength,
//					&did))
//				{
//					TCHAR fname[256];

//					// Try by friendly name first.
//					if (!SetupDiGetDeviceRegistryProperty(hDevInfoList, &did, SPDRP_FRIENDLYNAME, NULL, (PBYTE) fname, sizeof(fname), NULL))
//					{	// Try by device description if friendly name fails.
//						if (!SetupDiGetDeviceRegistryProperty(hDevInfoList, &did, SPDRP_DEVICEDESC, NULL, (PBYTE) fname, sizeof(fname), NULL))
//						{	// Use the raw path information for linkname and friendlyname
//							strncpy_s(fname, 256, (char*) functionClassDeviceData->DevicePath, 256);
//    }
//}


//HANDLE UdiskDevice = CreateFile(functionClassDeviceData->DevicePath,
//    GENERIC_READ | GENERIC_WRITE,
//    FILE_SHARE_READ | FILE_SHARE_WRITE,
//    NULL,
//    OPEN_EXISTING,
//    0,
//    NULL);



////=============== Get Attribute ===============
//HIDD_ATTRIBUTES Attributes;
//ZeroMemory(&Attributes, sizeof(Attributes));
//Attributes.Size = sizeof(HIDD_ATTRIBUTES);
//if (!HidD_GetAttributes(UdiskDevice, &Attributes))
//{
//    CloseHandle(UdiskDevice);
//    DeviceNum++;
//    continue;
//}
//if (Attributes.ProductID == m_Pid && Attributes.VendorID == m_Vid
//    && Attributes.VersionNumber == m_Pvn)
//{
//    //Save Device Path
//    CString m_linkname = functionClassDeviceData->DevicePath;
//    memcpy(str, m_linkname.GetBuffer(), m_linkname.GetLength());
//    *len = m_linkname.GetLength();
//    free(functionClassDeviceData);
//    SetupDiDestroyDeviceInfoList(hDevInfoList);
//    return TRUE;
//}

//free(functionClassDeviceData);
//CloseHandle(UdiskDevice);
//UdiskDevice = INVALID_HANDLE_VALUE;
//				}
//				DeviceNum++;
//			}
//		}
//	}

//	// SetupDiDestroyDeviceInfoList() destroys a device information set
//	// and frees all associated memory.
//	SetupDiDestroyDeviceInfoList(hDevInfoList);
//return FALSE;
//}
//————————————————

//                            版权声明：本文为博主原创文章，遵循 CC 4.0 BY-SA 版权协议，转载请附上原文出处链接和本声明。

//原文链接：https://blog.csdn.net/newworldis/article/details/127755767
//    }
//}
