// ConsoleApplication_NVME.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <windows.h>
#include <Nvme.h>
typedef VOID(*CQ_CALLBACK)(DWORD req_val, DWORD cdw0);
DWORD nvme_specific(HANDLE FileHandle, STORAGE_PROTOCOL_NVME_DATA_TYPE data_type, DWORD req_val, DWORD req_sub_val, PVOID pdata, DWORD xfer_len, CQ_CALLBACK callback)
{
	BOOL result;
	PVOID buffer = NULL;
	ULONG bufferLength = 0;
	ULONG returnedLength = 0;

	PSTORAGE_PROPERTY_QUERY query = NULL;
	PSTORAGE_PROTOCOL_SPECIFIC_DATA protocolData = NULL;
	PSTORAGE_PROTOCOL_DATA_DESCRIPTOR protocolDataDescr = NULL;

	//
	// Allocate buffer for use.
	//
	auto s1 = sizeof(STORAGE_PROPERTY_QUERY);
	auto s2 = FIELD_OFFSET(STORAGE_PROPERTY_QUERY, AdditionalParameters);
	bufferLength = FIELD_OFFSET(STORAGE_PROPERTY_QUERY, AdditionalParameters) + sizeof(STORAGE_PROTOCOL_SPECIFIC_DATA) + xfer_len;
	buffer = malloc(bufferLength);

	if (buffer == NULL)
	{
		printf("DeviceNVMeQueryProtocolDataTest: allocate buffer failed, exit.\n");
		return 1;
	}

	//
	// Initialize query data structure to get Identify Controller Data.
	//
	ZeroMemory(buffer, bufferLength);

	query = (PSTORAGE_PROPERTY_QUERY)buffer;
	protocolDataDescr = (PSTORAGE_PROTOCOL_DATA_DESCRIPTOR)buffer;
	protocolData = (PSTORAGE_PROTOCOL_SPECIFIC_DATA)query->AdditionalParameters;

	query->PropertyId = StorageAdapterProtocolSpecificProperty;
	query->QueryType = PropertyStandardQuery;

	protocolData->ProtocolType = ProtocolTypeNvme;
	protocolData->DataType = data_type;
	protocolData->ProtocolDataRequestValue = req_val;
	protocolData->ProtocolDataRequestSubValue = req_sub_val;
	if (xfer_len)
	{
		protocolData->ProtocolDataOffset = sizeof(STORAGE_PROTOCOL_SPECIFIC_DATA);
		protocolData->ProtocolDataLength = xfer_len;
	}
	unsigned char mem[8192] = { 0 };
	::memcpy(mem, buffer, bufferLength);
	auto ff = ::fopen("buffer.bin", "wb");
	::fwrite(mem, 1, bufferLength, ff);
	::fclose(ff);
	//
	// Send request down.
	//
	result = DeviceIoControl(FileHandle,
		IOCTL_STORAGE_QUERY_PROPERTY,
		buffer,
		bufferLength,
		buffer,
		bufferLength,
		&returnedLength,
		NULL);

	if (!result || (returnedLength == 0))
	{
		printf("FAIL, Error Code=%d\n", GetLastError());
		return GetLastError();
	}

	//
	// Validate the returned data.
	//
	if ((protocolDataDescr->Version != sizeof(STORAGE_PROTOCOL_DATA_DESCRIPTOR)) ||
		(protocolDataDescr->Size != sizeof(STORAGE_PROTOCOL_DATA_DESCRIPTOR)))
	{
		printf("Data descriptor header not valid\n");
		return 1;
	}

	protocolData = &protocolDataDescr->ProtocolSpecificData;
	memcpy_s(pdata, xfer_len, (PCHAR)protocolData + protocolData->ProtocolDataOffset, xfer_len);

	if (callback != NULL)
	{
		callback(req_val, protocolDataDescr->ProtocolSpecificData.FixedProtocolReturnData);
	}

	free(buffer);
	return 0;
}

DWORD nvme_get_log_page(NVME_LOG_PAGES lid)
{
	HANDLE FileHandle = CreateFileA(
		"\\\\?\\aa", GENERIC_WRITE | GENERIC_READ,
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		NULL, OPEN_EXISTING, 0, NULL
	);
	CHAR pdata[NVME_MAX_LOG_SIZE];
	if (nvme_specific(FileHandle, NVMeDataTypeLogPage, lid, 0, pdata, NVME_MAX_LOG_SIZE, NULL)) {
		return 1;
	}
	switch (lid)
	{
	case NVME_LOG_PAGE_HEALTH_INFO:
		PNVME_HEALTH_INFO_LOG smartInfo = (PNVME_HEALTH_INFO_LOG)pdata;
		printf("SMART/Health Info - Temperature %d.\n", ((ULONG)smartInfo->Temperature[1] << 8 | smartInfo->Temperature[0]) - 273);
	}
	return 0;
}

int main()
{
	nvme_get_log_page(NVME_LOG_PAGE_HEALTH_INFO);
	//\\?\scsi#disk&ven_nvme&prod_sk_hynix_pc711_h#5&6091cf4&0&000000#{53f56307-b6bf-11d0-94f2-00a0c91efb8b}
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
