using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_NET8
{
    public static partial class DevCon_SMBIOS
    {
        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        private static partial uint GetSystemFirmwareTable(
            uint FirmwareTableProviderSignature,
            uint FirmwareTableID,
            Span<byte> pFirmwareTableBuffer);

        public static void RSMB()
        {
            try
            {
                const uint RSMB_SIGNATURE = 0x52534d42; // "RSMB"
                const uint RSMB_TABLE_ID = 0;

                // 第一次呼叫取得所需的緩衝區大小
                uint requiredSize = GetSystemFirmwareTable(RSMB_SIGNATURE, RSMB_TABLE_ID, Span<byte>.Empty);

                if (requiredSize == 0)
                {
                    Console.WriteLine("無法取得 SMBIOS 資料");
                    return;
                }

                Span<byte> smbiosSpan = stackalloc byte[(int)requiredSize];
                uint bytesRead = GetSystemFirmwareTable(RSMB_SIGNATURE, RSMB_TABLE_ID, smbiosSpan);

                if (bytesRead > 0)
                {
                    Console.WriteLine($"成功讀取 SMBIOS 資料，大小: {bytesRead} 位元組");
                    // 使用 smbiosSpan 進行後續處理
                }
                else
                {
                    Console.WriteLine("無法讀取 SMBIOS 資料");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }
    }
}
