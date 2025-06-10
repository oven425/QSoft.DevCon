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
            char[] charArray = { 'H', 'e', 'l', 'l', 'o', '\0', 'W', 'o', 'r', 'l', 'd', '\0', 'C', 'S', 'h', 'a', 'r', 'p', '\0' };
            ReadOnlySpan<char> mySpan = new ReadOnlySpan<char>(charArray);

            while(true)
            {
                int nullTerminatorIndex = mySpan.IndexOf('\0');
                if (nullTerminatorIndex <= 0) break;
                var mm = mySpan[..nullTerminatorIndex];
                mySpan = mySpan[(nullTerminatorIndex + 1)..];
            }
            


            var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithId("net80"))
    //.AddDiagnoser(MemoryDiagnoser.Default, new NativeMemoryProfiler())
    .AddJob(Job.Default.WithRuntime(ClrRuntime.Net472).WithId("net472"));

            var results = BenchmarkRunner.Run<DevConT>(config);

            //var results = BenchmarkRunner.Run<DevConT>();
            Console.ReadLine();
            
        }
    }
}



[MemoryDiagnoser]
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
            oo.PowerData();
            oo.Manufacturer();
            oo.BiosDeviceName();
            oo.DeviceInstanceId();
        }
    }
}


