// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using QSoft.DevCon;

namespace Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithId("net80"))
    .AddDiagnoser(MemoryDiagnoser.Default, new NativeMemoryProfiler())
    .AddJob(Job.Default.WithRuntime(ClrRuntime.Net472).WithId("net472"));

            var results = BenchmarkRunner.Run<DevConT>(config);


            //var results = BenchmarkRunner.Run<DevConT>();
            Console.WriteLine("Hello, World!");
        }
    }
}


//[MemoryDiagnoser]
//[SimpleJob(RuntimeMoniker.Net472)]
//[SimpleJob(RuntimeMoniker.Net80)]
public class DevConT
{
    [Benchmark]
    
    public void AA()
    {
        foreach(var oo in  "Camera".Devices())
        {
            oo.GetFriendName();
            oo.Service();
        }
    }

}
