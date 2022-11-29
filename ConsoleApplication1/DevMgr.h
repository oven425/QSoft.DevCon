#pragma once
#include <vector>
#include <memory>
#include <functional>
using namespace std;

#include <atlstr.h>

namespace QSoft
{
namespace DevCon
{
class DeviceInfo
{
public:
	CString FriendlyName;
};
class DevMgr
{
public:
	int Enable(function<bool(DeviceInfo)> func)
	{
		return 0;
	}
	vector<unique_ptr<DeviceInfo>> AllDevice()
	{
		vector<unique_ptr<DeviceInfo>> devices;

		return devices;
	}
private:
	HANDLE m_hDevInfo;
};


}
}


