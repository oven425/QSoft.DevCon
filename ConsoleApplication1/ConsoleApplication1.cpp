// ConsoleApplication1.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <string>
#include <sstream>
#include <iomanip>
using namespace std;
#include <Windows.h>
#include <SetupAPI.h>
#pragma comment(lib, "setupapi.lib")
#include <hidsdi.h>
#pragma comment( lib, "hid.lib")
#include <initguid.h>
#include <Hidclass.h>
#include <Ntddmou.h>
#include <cfgmgr32.h>
#include <Usbiodef.h>
#include <atlstr.h>

int aa = CM_DEVCAP_HARDWAREDISABLED;
BOOL ListDeviceInstancePath1()
{
	GUID guid1;
	HidD_GetHidGuid(&guid1);
	
	//spDevInfoData.ClassGuid = {745A17A0-74D3-11D0-B6FE-00A0C90F57DA}
	HDEVINFO hdev;
	DWORD idx;
	GUID guid = GUID_DEVINTERFACE_USB_DEVICE;
	::memcpy(&guid, &GUID_DEVINTERFACE_USB_DEVICE, sizeof(GUID));
	TCHAR csDevicePath[2048] = { 0 };
	BOOL bRet = TRUE;
	BOOL nStatus;
	DWORD dwSize = 0;
	//::memset(&guid, 0, sizeof(guid));
	//hdev = SetupDiGetClassDevs(0L, NULL, NULL, DIGCF_PRESENT | DIGCF_ALLCLASSES | DIGCF_PROFILE);
	hdev = SetupDiGetClassDevs(NULL, NULL, NULL, DIGCF_PRESENT| DIGCF_ALLCLASSES | DIGCF_DEVICEINTERFACE);
	if (hdev == INVALID_HANDLE_VALUE)
	{
		printf("ERROR : Unable to enumerate device.\n");
		auto err = ::GetLastError();
		return FALSE;
	}

	SP_DEVICE_INTERFACE_DATA  DeviceInterfaceData;
	DeviceInterfaceData.cbSize = sizeof(DeviceInterfaceData);

	for (idx = 0; SetupDiEnumDeviceInterfaces(hdev, NULL, &guid1, idx, &DeviceInterfaceData); idx++)
	{
		nStatus = SetupDiGetDeviceInterfaceDetail(hdev, &DeviceInterfaceData, NULL, 0, &dwSize, NULL);
		if (!dwSize)
		{
			bRet = FALSE;
			printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
			break;
		}

		PSP_DEVICE_INTERFACE_DETAIL_DATA pBuffer = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(dwSize);
		ZeroMemory(pBuffer, sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
		pBuffer->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
		SP_DEVINFO_DATA DeviceInfoData = { sizeof(SP_DEVINFO_DATA) };
		nStatus = SetupDiGetDeviceInterfaceDetail(hdev, &DeviceInterfaceData, pBuffer, dwSize, &dwSize, &DeviceInfoData);
		if (!nStatus)
		{
			bRet = FALSE;
			printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
			break;
		}
		//csDevicePath = pBuffer->DevicePath;
		::memset(csDevicePath, 0, sizeof(csDevicePath));
		::wcscpy(csDevicePath, pBuffer->DevicePath);
		//csDevicePath.MakeUpper();
		::OutputDebugString(csDevicePath);
		::OutputDebugStringA("\r\n");
	}
	int err = ::GetLastError();
	SetupDiDestroyDeviceInfoList(hdev);

	return bRet;
}

BOOL ListDeviceInstancePath()
{
	HDEVINFO hdev;
	DWORD idx;
	GUID guid = GUID_DEVINTERFACE_DISK;
	auto hr = CLSIDFromString(_T("{6BDD1FC6-810F-11D0-BEC7-08002BE2092F}"), &guid);
	//guid = GUID_DEVINTERFACE_IMAGE;
	CString csDevicePath;
	BOOL bRet = TRUE;
	BOOL nStatus;
	DWORD dwSize = 0;

	hdev = SetupDiGetClassDevs(&guid,
		NULL,
		NULL,
		DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
	if (hdev == INVALID_HANDLE_VALUE)
	{
		printf("ERROR : Unable to enumerate device.\n");
		return FALSE;
	}

	SP_DEVICE_INTERFACE_DATA  DeviceInterfaceData;
	DeviceInterfaceData.cbSize = sizeof(DeviceInterfaceData);

	for (idx = 0; SetupDiEnumDeviceInterfaces(hdev, NULL, &guid, idx, &DeviceInterfaceData); idx++)
	{
		nStatus = SetupDiGetDeviceInterfaceDetail(hdev, &DeviceInterfaceData, NULL, 0, &dwSize, NULL);
		if (!dwSize)
		{
			bRet = FALSE;
			printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
			break;
		}

		PSP_DEVICE_INTERFACE_DETAIL_DATA pBuffer = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(dwSize);
		ZeroMemory(pBuffer, sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
		pBuffer->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

		SP_DEVINFO_DATA DeviceInfoData = { sizeof(SP_DEVINFO_DATA) };
		nStatus = SetupDiGetDeviceInterfaceDetail(hdev, &DeviceInterfaceData, pBuffer, dwSize, &dwSize, &DeviceInfoData);
		if (!nStatus)
		{
			bRet = FALSE;
			printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
			break;
		}
		csDevicePath = pBuffer->DevicePath;
		csDevicePath.MakeUpper();
		printf("%s\n", csDevicePath);
	}
	SetupDiDestroyDeviceInfoList(hdev);

	return bRet;
}

//void GetDeviceInterfaceInfo(HDEVINFO hDevInfo, SP_DEVINFO_DATA spDevInfoData, TCHAR* szPath)
//{
//	SP_DEVICE_INTERFACE_DATA spDevInterfaceData = { 0 };
//	//
//	spDevInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
//	//if (!SetupDiCreateDeviceInterface(hDevInfo, &spDevInfoData, &spDevInfoData.ClassGuid, 0L, 0L, &spDevInterfaceData))
//	//{
//	//	//ShowErrorMsg(_hDlg, GetLastError(), "SetupDiBuildDriverInfoList");
//	//}
//	if (!SetupDiOpenDeviceInterface(hDevInfo, &spDevInfoData, &spDevInfoData.ClassGuid, 0L, 0L, &spDevInterfaceData))
//	{
//		//ShowErrorMsg(_hDlg, GetLastError(), "SetupDiBuildDriverInfoList");
//	}
//	else
//	{
//		SP_DEVICE_INTERFACE_DETAIL_DATA* pspDevInterfaceDetail = 0L;
//		DWORD                           dwRequire = 0L;
//		//
//		if (!SetupDiGetDeviceInterfaceDetail(hDevInfo, &spDevInterfaceData, 0L, 0, &dwRequire, 0L))
//		{
//			DWORD dwError = GetLastError();
//			//
//			if (dwError != ERROR_INSUFFICIENT_BUFFER)
//			{
//				//ShowErrorMsg(_hDlg, dwError, "SetupDiBuildDriverInfoList");
//				return;
//			};
//		};
//		//
//		pspDevInterfaceDetail = (SP_DEVICE_INTERFACE_DETAIL_DATA*)LocalAlloc(LPTR, sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA) * dwRequire);
//		pspDevInterfaceDetail->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
//		if (!SetupDiGetDeviceInterfaceDetail(hDevInfo, &spDevInterfaceData, pspDevInterfaceDetail, dwRequire, &dwRequire, 0L))
//		{
//			DWORD dwError = GetLastError();
//			//
//			//if (dwError != ERROR_INSUFFICIENT_BUFFER)
//				//ShowErrorMsg(_hDlg, dwError, "SetupDiBuildDriverInfoList");
//		}
//		else
//		{
//			CString str = pspDevInterfaceDetail->DevicePath;
//			str = _T("");
//			//memcpy(szPath, pspDevInterfaceDetail->DevicePath, strlen(pspDevInterfaceDetail->DevicePath));
//			//            switch(spDevInterfaceData.                    
//		};
//		//
//		if (pspDevInterfaceDetail)
//			LocalFree(pspDevInterfaceDetail);
//	};
//};

#include "DevMgr.h"
#include <Devpropdef.h>
#include <devguid.h>
#include <Devpkey.h>
int main()
{
	DWORD reqszie = 0;
	auto uu = SetupDiClassGuidsFromNameW(_T("Camera"), nullptr, 0, &reqszie);
	auto guids = new GUID[reqszie];
	uu = SetupDiClassGuidsFromNameW(_T("Camera"), guids, reqszie, &reqszie);

	//DEVPKEY_Device_DriverVersion
	auto dd = DEVPROP_TYPE_BINARY;
	//auto aa = SPDRP_PHYSICAL_DEVICE_LOCATION;
	//ListDeviceInstancePath();
	//QSoft::DevCon::DevMgr devmgr;
	//devmgr.Enable([](auto dev) {return true; });
	//SPDIT_COMPATDRIVER
	SP_DRVINFO_DATA sp = { 0 };
	auto cbSize = sizeof(sp);
	auto ii = GUID_DEVINTERFACE_MOUSE;
	//ListDeviceInstancePath();
	//從 EnumWDMDriver 開始找
	GUID guid1;
	HidD_GetHidGuid(&guid1);
	//GUID_DEVCLASS_PROCESSOR
	//GUID_DEVCLASS_DISKDRIVE
	//GUID_DEVCLASS_CAMERA
	GUID_DEVCLASS_VOLUME;
	//GUID_DEVINTERFACE_USB_HUB
	HDEVINFO hDevInfo = SetupDiGetClassDevs(&GUID_DEVCLASS_DISKDRIVE, 0L, NULL, DIGCF_PRESENT  | DIGCF_PROFILE);
	GUID_DEVINTERFACE_USB_DEVICE;
	int index = 0;
	SP_DEVINFO_DATA spDevInfoData = { 0 };
	spDevInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
	while (true)
	{
		TCHAR tarhet[200] = { 0 };
		QueryDosDevice(_T("D:"), tarhet, 100);
		TCHAR device_desc[MAXCHAR] = { 0 };
		if (SetupDiEnumDeviceInfo(hDevInfo, index, &spDevInfoData) == TRUE)
		{
			//GetDeviceInterfaceInfo(hDevInfo, spDevInfoData, NULL);
			//{4340a6c5-93fa-4706-972c-7b648008a5a7} 8
			DEVPROPKEY parent;
			parent.pid = 8;
			
			IIDFromString(L"{4340a6c5-93fa-4706-972c-7b648008a5a7}", &parent.fmtid);
			DWORD reqsz = 0;
			TCHAR parentstr[300] = { 0 };
			auto bb11 = SetupDiGetDeviceProperty(hDevInfo, &spDevInfoData, &parent, &parent.pid, (PBYTE)parentstr, 300, &reqsz, 0);
			if (bb11 == FALSE)
			{
				auto err = ::GetLastError();
				err = 0;
			}
			DWORD reqcount = 0;
			DEVPROPKEY keys[100] = { 0 };
			auto gethr = SetupDiGetDevicePropertyKeys(hDevInfo, &spDevInfoData, keys, 100, &reqcount, 0);
			ULONG status = 0;
			ULONG problem = 0;
			CM_Get_DevNode_Status(&status, &problem, spDevInfoData.DevInst, 0);
			DN_STARTED;
			DWORD compatibleids;
			//TCHAR compatibleids[1024] = { 0 };
			unsigned char bbbs[1024] = { 0 };
			auto pptype = REG_DWORD;
			DWORD sss = 0;
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_CAPABILITIES, &pptype, bbbs, sizeof(bbbs), &sss) == FALSE)
			{
				DWORD err = ::GetLastError();
				err = 0;
			}
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_CLASS, 0L, (PBYTE)device_desc, 2048, 0))
			{
				index++;
				continue;
			}
			TCHAR classguid[256] = { 0 };
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_CLASSGUID, 0L, (PBYTE)classguid, 2048, 0))
			{
				DWORD err = ::GetLastError();
				::wsprintf(classguid, L"fail(%d)", err);
			}
			TCHAR instanseid[LINE_LEN] = { 0 };
			if (SetupDiGetDeviceInstanceId(hDevInfo, &spDevInfoData, instanseid, LINE_LEN, 0) == false)
			{
				DWORD err = ::GetLastError();
				::wsprintf(instanseid, L"fail(%d)", err);
			}
			TCHAR classdesc[LINE_LEN] = { 0 };
			if (SetupDiGetClassDescription(&spDevInfoData.ClassGuid, classdesc, LINE_LEN, NULL) == false)
			{
				DWORD err = ::GetLastError();
				::wsprintf(classdesc, L"fail(%d)", err);
			};
			DWORD RequiredSize = 0;
			DWORD property_type = 0;
			auto bb = SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_CLASSGUID, &property_type, NULL, NULL, &RequiredSize);
			auto err1 = ::GetLastError();
			TCHAR friendlyname[LINE_LEN] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_FRIENDLYNAME, 0L, (PBYTE)friendlyname, LINE_LEN, 0)==FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(friendlyname, L"fail(%d)", err);
			}
			TCHAR devicedesc[LINE_LEN] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_DEVICEDESC, 0L, (PBYTE)devicedesc, LINE_LEN, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(devicedesc, L"fail(%d)", err);
			}

			TCHAR hardwardid[2048] = { 0 };
			wstring hardwardids;
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_HARDWAREID, 0L, (PBYTE)hardwardid, 2048, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(hardwardid, L"fail(%d)", err);
				hardwardids = hardwardid;
			}
			else
			{
				TCHAR* p = hardwardid;
				while (*p)
				{
					if (hardwardids.empty() == false)
					{
						hardwardids = hardwardids + L"\r\n           ";
					}
					hardwardids = hardwardids + p;
					p = ::wcschr(p, 0);
					*p++;
				}
			}
			TCHAR mfg[LINE_LEN] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_MFG, 0L, (PBYTE)mfg, LINE_LEN, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(mfg, L"fail(%d)", err);
			}
			int bus_number = 0;
			TCHAR bus_number_str[128] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_BUSNUMBER, 0L, (PBYTE)&bus_number, 63, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(bus_number_str, L"fail(%d)", err);
			}
			else
			{
				::wsprintf(bus_number_str, L"%d", bus_number);
			}
			TCHAR driver[1024] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_DRIVER, 0L, (PBYTE)driver, 1024, 0)==FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(driver, L"fail(%d)", err);
			}
			TCHAR locationinfo[4096] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_LOCATION_INFORMATION, NULL, (PBYTE)locationinfo, 4096, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(locationinfo, L"fail(%d)", err);
			}
			TCHAR localpaths[4096] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_LOCATION_PATHS, NULL, (PBYTE)localpaths, 4096, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(localpaths, L"fail(%d)", err);
			}

			TCHAR enumerator[1024] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_ENUMERATOR_NAME, 0L, (PBYTE)enumerator, 63, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(enumerator, L"fail(%d)", err);
			}
			TCHAR lowerfilter[1024] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_LOWERFILTERS, 0L, (PBYTE)lowerfilter, 63, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(lowerfilter, L"fail(%d)", err);
			}
			TCHAR upperfilter[1024] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_UPPERFILTERS, 0L, (PBYTE)upperfilter, 63, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(upperfilter, L"fail(%d)", err);
			}
			TCHAR objectname[1024] = { 0 };
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, 0L, (PBYTE)objectname, 63, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(objectname, L"fail(%d)", err);
			}
			if (::wcscmp(classguid, L"{4d36e978-e325-11ce-bfc1-08002be10318}") == 0)
			{
				auto key = SetupDiOpenDevRegKey(hDevInfo, &spDevInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_QUERY_VALUE);
				if (key == INVALID_HANDLE_VALUE)
				{

				}
				int a = 0;
				a = a + 1;
			}
			SP_DEVICE_INTERFACE_DATA spDevInterfaceData = { 0 };
			//
			spDevInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
			if (!SetupDiCreateDeviceInterface(hDevInfo, &spDevInfoData, &spDevInfoData.ClassGuid, 0L, 0L, &spDevInterfaceData))
			{
				auto err = ::GetLastError();
				err = 0;
				//ShowErrorMsg(_hDlg, GetLastError(), "SetupDiBuildDriverInfoList");
			}
			//SetupDiOpenDeviceInterface()
			std::wstringstream driverinfo;
			if (SetupDiBuildDriverInfoList(hDevInfo, &spDevInfoData, SPDIT_COMPATDRIVER) == TRUE)
			{

				int driverindex = 0;
				while (true)
				{
					
					SP_DRVINFO_DATA        spDrvInfoData = { 0 };
					SP_DRVINFO_DETAIL_DATA spDrvInfoDetail = { 0 };
					spDrvInfoData.cbSize = sizeof(SP_DRVINFO_DATA);
					if (SetupDiEnumDriverInfo(hDevInfo, &spDevInfoData, SPDIT_COMPATDRIVER, driverindex, &spDrvInfoData) == FALSE)
					{
						DWORD err = ::GetLastError();
						if (err != ERROR_NO_MORE_ITEMS)
						{
							driverinfo << L"fail(" << err << L")";
						}
						break;
					}
					else
					{
						//memcpy(&spDrvInfoDetail, szBuf, sizeof(SP_DRVINFO_DETAIL_DATA));
						spDrvInfoDetail.cbSize = sizeof(SP_DRVINFO_DETAIL_DATA);
						DWORD dwRequireSize = 0;
						if (SetupDiGetDriverInfoDetail(hDevInfo, &spDevInfoData, &spDrvInfoData, &spDrvInfoDetail, 2048, &dwRequireSize))
						{
							SYSTEMTIME date = { 0 };
							FileTimeToSystemTime(&spDrvInfoData.DriverDate, &date);
							
							driverinfo << L"driver ("<<driverindex<<L"):"<<endl;
							driverinfo << L"          " << L"MfgName:" << spDrvInfoData.MfgName << endl;
							driverinfo << L"          " << L"ProviderName:" << spDrvInfoData.ProviderName << endl;
							driverinfo << L"          " << L"DriverType:" << spDrvInfoData.DriverType << endl;
							driverinfo << L"          " << L"SectionName:" << spDrvInfoDetail.SectionName << endl;
							driverinfo << L"          " << L"HardwareID:" << spDrvInfoDetail.HardwareID << endl;
							driverinfo << L"          " << L"Inf filename:"<< spDrvInfoDetail.InfFileName<<endl;
							driverinfo << L"          " << L"Inf Date:" << date.wYear<<L"/"<< date.wMonth<<L"/"<< date.wDay << endl;
						}
					}
					driverindex = driverindex + 1;
				}


				
				SetupDiDestroyDriverInfoList(hDevInfo, &spDevInfoData, SPDIT_COMPATDRIVER);
			}

			wstring str = classdesc;
			str = str + L"(" + device_desc + L")\r\n";
			str = str + L"Class Guid:" + classguid + L"\r\n";
			str = str + L"frendly name:" + friendlyname + L"\r\n";
			str = str + L"device desc:" + devicedesc + L"\r\n";
			str = str + L"instanseid:" + instanseid + L"\r\n";
			str = str + L"hardwardid:" + hardwardids + L"\r\n";
			str = str + L"manufacturer:" + mfg + L"\r\n";
			str = str + L"bus number:" + bus_number_str + L"\r\n";
			str = str + L"driver:" + driver + L"\r\n";
			str = str + L"Local Information:" + locationinfo + L"\r\n";
			str = str + L"enumerator:" + enumerator + L"\r\n";

			str = str + L"objectname:" + objectname + L"\r\n";
			str = str + L"upperfilter:" + upperfilter + L"\r\n";
			str = str + L"lowerfilter:" + lowerfilter + L"\r\n";
			str = str + driverinfo.str() + L"\r\n";
			SP_DEVICE_INTERFACE_DATA  DeviceInterfaceData;
			DeviceInterfaceData.cbSize = sizeof(DeviceInterfaceData);

			if (wcscmp(device_desc, L"Ports") == 0)
			{
				auto err = ::GetLastError();
				::OutputDebugStringA("");
			}

			if (wcscmp(device_desc, L"HIDClass") == 0)
			{
				BOOL bb = SetupDiEnumDeviceInterfaces(hDevInfo, &spDevInfoData, &guid1, 0, &DeviceInterfaceData);
				if (bb == FALSE)
				{
					auto err = ::GetLastError();
					::OutputDebugStringA("");
				}
				else
				{
					DWORD dwSize = 0;
					auto nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, NULL, 0, &dwSize, NULL);
					if (!dwSize)
					{
						//bRet = FALSE;
						printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
						break;
					}

					PSP_DEVICE_INTERFACE_DETAIL_DATA pBuffer = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(dwSize);
					ZeroMemory(pBuffer, sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
					pBuffer->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

					SP_DEVINFO_DATA DeviceInfoData = { sizeof(SP_DEVINFO_DATA) };
					nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, pBuffer, dwSize, &dwSize, &DeviceInfoData);
					if (!nStatus)
					{
						//bRet = FALSE;
						printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
						break;
					}
					//::OutputDebugString(pBuffer->DevicePath);
					str = str + L"devicepath:" + pBuffer->DevicePath + L"\r\n";
				}
			}
			
			
			for (int idx = 0; SetupDiEnumDeviceInterfaces(hDevInfo, &spDevInfoData, &spDevInfoData.ClassGuid, idx, &DeviceInterfaceData)==TRUE; idx++)
			{
				DWORD dwSize = 0;
				auto nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, NULL, 0, &dwSize, NULL);
				if (!dwSize)
				{
					//bRet = FALSE;
					printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
					break;
				}

				PSP_DEVICE_INTERFACE_DETAIL_DATA pBuffer = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(dwSize);
				ZeroMemory(pBuffer, sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
				pBuffer->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

				SP_DEVINFO_DATA DeviceInfoData = { sizeof(SP_DEVINFO_DATA) };
				nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, &DeviceInterfaceData, pBuffer, dwSize, &dwSize, &DeviceInfoData);
				if (!nStatus)
				{
					//bRet = FALSE;
					printf("ERROR : SetupDiGetDeviceInterfaceDetail fial.\n");
					break;
				}
				//::OutputDebugString(pBuffer->DevicePath);
				str = str + L"devicepath:" + pBuffer->DevicePath + L"\r\n";
			}
			index++;

			::OutputDebugString(str.c_str());

		}
		else
		{
			break;
		}
	}




	SetupDiDestroyDeviceInfoList(hDevInfo);
    std::cout << "Hello World!\n";
}

// 執行程式: Ctrl + F5 或 [偵錯] > [啟動但不偵錯] 功能表
// 偵錯程式: F5 或 [偵錯] > [啟動偵錯] 功能表

// 開始使用的提示: 
//   1. 使用 [方案總管] 視窗，新增/管理檔案
//   2. 使用 [Team Explorer] 視窗，連線到原始檔控制
//   3. 使用 [輸出] 視窗，參閱組建輸出與其他訊息
//   4. 使用 [錯誤清單] 視窗，檢視錯誤
//   5. 前往 [專案] > [新增項目]，建立新的程式碼檔案，或是前往 [專案] > [新增現有項目]，將現有程式碼檔案新增至專案
//   6. 之後要再次開啟此專案時，請前往 [檔案] > [開啟] > [專案]，然後選取 .sln 檔案
