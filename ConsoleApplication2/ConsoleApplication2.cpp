// ConsoleApplication2.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <windows.h>
#include <setupapi.h>
#include <batclass.h>
#include <devguid.h>
#include <powrprof.h>

#pragma comment(lib, "powrprof.lib")
#pragma comment(lib, "setupapi.lib")

DWORD GetBatteryState()
{
#define GBS_HASBATTERY 0x1
#define GBS_ONBATTERY  0x2
    // Returned value includes GBS_HASBATTERY if the system has a 
    // non-UPS battery, and GBS_ONBATTERY if the system is running on 
    // a battery.
    //
    // dwResult & GBS_ONBATTERY means we have not yet found AC power.
    // dwResult & GBS_HASBATTERY means we have found a non-UPS battery.

    DWORD dwResult = GBS_ONBATTERY;

    // IOCTL_BATTERY_QUERY_INFORMATION,
    // enumerate the batteries and ask each one for information.

    HDEVINFO hdev =
        SetupDiGetClassDevs(&GUID_DEVCLASS_BATTERY,
            0,
            0,
            DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
    if (INVALID_HANDLE_VALUE != hdev)
    {
        // Limit search to 100 batteries max
        for (int idev = 0; idev < 100; idev++)
        {
            SP_DEVICE_INTERFACE_DATA did = { 0 };
            did.cbSize = sizeof(did);

            if (SetupDiEnumDeviceInterfaces(hdev,
                0,
                &GUID_DEVCLASS_BATTERY,
                idev,
                &did))
            {
                DWORD cbRequired = 0;

                SetupDiGetDeviceInterfaceDetail(hdev,
                    &did,
                    0,
                    0,
                    &cbRequired,
                    0);
                if (ERROR_INSUFFICIENT_BUFFER == GetLastError())
                {
                    PSP_DEVICE_INTERFACE_DETAIL_DATA pdidd =
                        (PSP_DEVICE_INTERFACE_DETAIL_DATA)LocalAlloc(LPTR,
                            cbRequired);
                    if (pdidd)
                    {
                        pdidd->cbSize = sizeof(*pdidd);
                        if (SetupDiGetDeviceInterfaceDetail(hdev,
                            &did,
                            pdidd,
                            cbRequired,
                            &cbRequired,
                            0))
                        {
                            // Enumerated a battery.  Ask it for information.
                            HANDLE hBattery =
                                CreateFile(pdidd->DevicePath,
                                    GENERIC_READ | GENERIC_WRITE,
                                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                                    NULL,
                                    OPEN_EXISTING,
                                    FILE_ATTRIBUTE_NORMAL,
                                    NULL);

                            if (INVALID_HANDLE_VALUE != hBattery)
                            {
                                // Ask the battery for its tag.
                                BATTERY_QUERY_INFORMATION bqi = { 0 };

                                DWORD dwWait = 0;
                                DWORD dwOut;
                                auto ttga = IOCTL_BATTERY_QUERY_INFORMATION;
                                if (DeviceIoControl(hBattery,
                                    IOCTL_BATTERY_QUERY_TAG,
                                    &dwWait,
                                    sizeof(dwWait),
                                    &bqi.BatteryTag,
                                    sizeof(bqi.BatteryTag),
                                    &dwOut,
                                    NULL)
                                    && bqi.BatteryTag)
                                {
                                    // With the tag, you can query the battery info.
                                    BATTERY_INFORMATION bi = { 0 };
                                    bqi.InformationLevel = BatteryInformation;

                                    if (DeviceIoControl(hBattery,
                                        IOCTL_BATTERY_QUERY_INFORMATION,
                                        &bqi,
                                        sizeof(bqi),
                                        &bi,
                                        sizeof(bi),
                                        &dwOut,
                                        NULL))
                                    {
                                        // Only non-UPS system batteries count
                                        if (bi.Capabilities & BATTERY_SYSTEM_BATTERY)
                                        {
                                            if (!(bi.Capabilities & BATTERY_IS_SHORT_TERM))
                                            {
                                                dwResult |= GBS_HASBATTERY;
                                            }

                                            // Query the battery status.
                                            BATTERY_WAIT_STATUS bws = { 0 };
                                            bws.BatteryTag = bqi.BatteryTag;

                                            BATTERY_STATUS bs;
                                            if (DeviceIoControl(hBattery,
                                                IOCTL_BATTERY_QUERY_STATUS,
                                                &bws,
                                                sizeof(bws),
                                                &bs,
                                                sizeof(bs),
                                                &dwOut,
                                                NULL))
                                            {
                                                if (bs.PowerState & BATTERY_POWER_ON_LINE)
                                                {
                                                    dwResult &= ~GBS_ONBATTERY;
                                                }
                                            }
                                        }
                                    }
                                }
                                CloseHandle(hBattery);
                            }
                        }
                        LocalFree(pdidd);
                    }
                }
            }
            else  if (ERROR_NO_MORE_ITEMS == GetLastError())
            {
                break;  // Enumeration failed - perhaps we're out of items
            }
        }
        SetupDiDestroyDeviceInfoList(hdev);
    }

    //  Final cleanup:  If we didn't find a battery, then presume that we
    //  are on AC power.

    if (!(dwResult & GBS_HASBATTERY))
        dwResult &= ~GBS_ONBATTERY;

    return dwResult;
}

int main()
{
    GetBatteryState();
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
