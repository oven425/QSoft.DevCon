// ConsoleApplication1.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <string>
using namespace std;
#include <Windows.h>
#include <SetupAPI.h>
#pragma comment(lib, "setupapi.lib")
#include "hidsdi.h"
#pragma comment( lib, "hid.lib")
//REG_DWORD
BOOL ListDeviceInstancePath()
{
	GUID guid1;
	HidD_GetHidGuid(&guid1);
	
	//spDevInfoData.ClassGuid = {745A17A0-74D3-11D0-B6FE-00A0C90F57DA}
	HDEVINFO hdev;
	DWORD idx;
	GUID guid = GUID_DEVINTERFACE_DISK;
	TCHAR csDevicePath[2048] = { 0 };
	BOOL bRet = TRUE;
	BOOL nStatus;
	DWORD dwSize = 0;

	hdev = SetupDiGetClassDevs(&guid, NULL, NULL, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
	if (hdev == INVALID_HANDLE_VALUE)
	{
		printf("ERROR : Unable to enumerate device.\n");
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

int main()
{
	//ListDeviceInstancePath();
	//從 EnumWDMDriver 開始找
	HDEVINFO hDevInfo = SetupDiGetClassDevs(0L, 0L, NULL, DIGCF_PRESENT | DIGCF_ALLCLASSES | DIGCF_PROFILE| DIGCF_DEVICEINTERFACE);

	int index = 0;
	SP_DEVINFO_DATA spDevInfoData = { 0 };
	spDevInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
	while (true)
	{
		TCHAR device_desc[MAXCHAR] = { 0 };
		if (SetupDiEnumDeviceInfo(hDevInfo, index, &spDevInfoData) == TRUE)
		{
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_CLASS, 0L, (PBYTE)device_desc, 2048, 0))
			{
				index++;
				continue;
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
			if (SetupDiGetDeviceRegistryProperty(hDevInfo, &spDevInfoData, SPDRP_LOCATION_INFORMATION, 0L, (PBYTE)locationinfo, 4096, 0) == FALSE)
			{
				DWORD err = ::GetLastError();
				::wsprintf(locationinfo, L"fail(%d)", err);
			}

			wstring str = classdesc;
			str = str + L"("+ device_desc + L")\r\n";
			str = str + L"frendly name:" + friendlyname +L"\r\n";
			str = str + L"device desc:" + devicedesc + L"\r\n";
			str = str + L"instanseid:"+instanseid + L"\r\n";
			str = str + L"hardwardid:" + hardwardids + L"\r\n";
			str = str + L"manufacturer:" + mfg + L"\r\n";
			str = str + L"bus number:" + bus_number_str + L"\r\n";
			str = str + L"driver:" + driver + L"\r\n";
			str = str + L"locationinfo:" + locationinfo + L"\r\n";
			
			SP_DEVICE_INTERFACE_DATA  DeviceInterfaceData;
			DeviceInterfaceData.cbSize = sizeof(DeviceInterfaceData);
			
			for (int idx = 0; SetupDiEnumDeviceInterfaces(hDevInfo, &spDevInfoData, &spDevInfoData.ClassGuid, idx, &DeviceInterfaceData); idx++)
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
			int ase = ::GetLastError();
			ase = 1;
			::OutputDebugStringA("\r\n");
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
