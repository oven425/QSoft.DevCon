// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using QSoft.DevCon;
using System.Runtime.InteropServices;
using System.Text;

//https://learn.microsoft.com/zh-tw/visualstudio/profiling/profiling-with-benchmark-dotnet?view=vs-2022

namespace Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            //        var config = DefaultConfig.Instance
            //.AddJob(Job.Default.WithId("net80"))
            //.AddDiagnoser(MemoryDiagnoser.Default, new NativeMemoryProfiler())
            //.AddJob(Job.Default.WithRuntime(ClrRuntime.Net472).WithId("net472"));
            UserNameBenchmark vv = new();
            var str = vv.StackAllocGetUserName();
            str = "";
            //        var results = BenchmarkRunner.Run<DevConT>(config);
            //var summary = BenchmarkRunner.Run<UserNameBenchmark>();

            //var results = BenchmarkRunner.Run<DevConT>();
            Console.WriteLine("Hello, World!");
        }
    }
}

[MemoryDiagnoser]
public partial class UserNameBenchmark
{
    // 定義 GetUserNameW Win32 API 的 P/Invoke 簽章
    // CharSet.Unicode 表示使用寬字元 (UTF-16)，對應 Win32 API 的 W 版本
    // SetLastError = true 允許我們在 API 呼叫失敗時獲取錯誤碼
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetUserNameW(
        IntPtr lpBuffer, // 用於接收使用者名稱的緩衝區指標 (LPWSTR)
        ref uint lpnSize // 緩衝區的大小（以字元為單位），同時也是返回時實際寫入的字元數 (LPDWORD)
    );

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetUserNameW(
        Span<char> lpBuffer, // 用於接收使用者名稱的緩衝區指標 (LPWSTR)
        ref uint lpnSize // 緩衝區的大小（以字元為單位），同時也是返回時實際寫入的字元數 (LPDWORD)
    );

    [LibraryImport("advapi32.dll", EntryPoint = "GetUserNameW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetUserName(Span<byte> lpBuffer, ref uint lpnSize);

    [LibraryImport("advapi32.dll", EntryPoint = "GetUserNameW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetUserName1(Span<char> lpBuffer, ref uint lpnSize);


    // 定義一個常數，表示用於接收使用者名稱的緩衝區大小（以字元為單位）
    // 256 個字元對於大多數使用者名稱來說應該足夠了
    private const int BufferCharSize = 256;

    /// <summary>
    /// 使用 stackalloc 呼叫 GetUserNameW 的基準測試方法。
    /// stackalloc 在棧上分配記憶體，速度非常快且自動管理。
    /// </summary>
    [Benchmark]
    public string StackAllocGetUserName()
    {
        uint size = (uint)BufferCharSize;
        size = 0;
        GetUserName1([], ref size);

        size = (uint)BufferCharSize;
        Span<char> buffer = stackalloc char[(int)BufferCharSize];
        GetUserName1(buffer, ref size);
        var sss1 = new string(buffer[..^1]);
        // 聲明一個 uint 變數來儲存緩衝區的初始大小和 GetUserNameW 返回的實際字元數。


        // 呼叫 GetUserNameW API。
        // (IntPtr)buffer 將 char* 轉換為 IntPtr，以符合 API 簽章。
        size = 0;
        GetUserName([], ref size);
        Span<byte> aa = stackalloc byte[(int)size*2];
        if (GetUserName(aa, ref size))
        {
            //buffer.Overlaps
            // 如果呼叫成功，將 char* 指向的記憶體轉換為 C# 字符串。
            // size - 1 是因為 GetUserNameW 返回的字元數包含 null 終止符，而 new string() 不需要它。
            var aa2 = aa.Slice(0, (int)size - 2);
            var sss = Encoding.Unicode.GetString(aa[..^2]);

            return aa.Slice(0, (int)size - 2).ToString();
        }
        else
        {
            // 如果 API 呼叫失敗，拋出異常。
            // 在實際應用中，您會在這裡添加更詳細的錯誤處理。
            throw new InvalidOperationException($"GetUserNameW 失敗: {Marshal.GetLastWin32Error()}");
        }
    }

    /// <summary>
    /// 使用 Marshal.AllocHGlobal 呼叫 GetUserNameW 的基準測試方法。
    /// Marshal.AllocHGlobal 在非託管堆上分配記憶體，需要手動釋放。
    /// </summary>
    [Benchmark]
    public string MarshalAllocHGlobalGetUserName()
    {
        IntPtr bufferPtr = IntPtr.Zero; // 用於儲存非託管緩衝區的指標
        string userName = null;
        // 聲明一個 uint 變數來儲存緩衝區的初始大小和 GetUserNameW 返回的實際字元數。
        uint size = (uint)BufferCharSize;

        try
        {
            // 在非託管堆上分配記憶體。
            // BufferCharSize * sizeof(char) 計算所需的位元組數 (256 字元 * 2 位元組/字元 = 512 位元組)。
            bufferPtr = Marshal.AllocHGlobal(BufferCharSize * sizeof(char));

            // 呼叫 GetUserNameW API。
            // 注意：lpnSize 參數是 ref uint，CLR 的 P/Invoke 機制會自動處理這個託管 uint 和原生 DWORD 之間的讀寫。
            if (GetUserNameW(bufferPtr, ref size))
            {
                // 如果呼叫成功，將 IntPtr 指向的非託管記憶體轉換為 C# 字符串。
                // Marshal.PtrToStringUni 專門用於將寬字元 (UTF-16) 指標轉換為字符串。
                // size - 1 是因為 GetUserNameW 返回的字元數包含 null 終止符。
                userName = Marshal.PtrToStringUni(bufferPtr, (int)size - 1);
            }
            else
            {
                // 如果 API 呼叫失敗，拋出異常。
                throw new InvalidOperationException($"GetUserNameW 失敗: {Marshal.GetLastWin32Error()}");
            }
        }
        finally
        {
            // 非常重要：在 finally 塊中釋放非託管記憶體，確保無論是否發生異常都能釋放。
            if (bufferPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }
        return userName;
    }


    //[MemoryDiagnoser]
    //[SimpleJob(RuntimeMoniker.Net472)]
    //[SimpleJob(RuntimeMoniker.Net80)]
    public class DevConT
    {
        [Benchmark]

        public void AA()
        {
            foreach (var oo in "Camera".Devices())
            {
                oo.GetFriendName();
                oo.Service();
            }
        }
    }

}
