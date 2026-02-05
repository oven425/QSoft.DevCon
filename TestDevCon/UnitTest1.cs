using Microsoft.Management.Infrastructure;
using QSoft.DevCon;

namespace TestDevCon
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //using CimSession session = CimSession.Create(null);
            //// 查詢 Windows PnP 實體，篩選與相機相關的服務或類別
            //string query = "SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera' OR Service = 'usbvideo'";
            //var instances = session.QueryInstances(@"root\cimv2", "WQL", query);

            //foreach (var instance in instances)
            //{
            //    Console.WriteLine($"[MI 來源]");
            //    Console.WriteLine($"名稱: {instance.CimInstanceProperties["Name"].Value}");
            //    Console.WriteLine($"DeviceID: {instance.CimInstanceProperties["PNPDeviceID"].Value}");
            //    System.Diagnostics.Trace.WriteLine($"{instance.CimInstanceProperties["InstallDate"].Value}");
            //    // HardwareID 通常是字串陣列
            //    var hwIds = (string[])instance.CimInstanceProperties["HardwareID"].Value;
            //    Console.WriteLine($"HardwareID: {string.Join(", ", hwIds)}");
            //    Console.WriteLine();
            //}

            Console.WriteLine("Test1--");
            var dds = Guid.Empty.Devices().Select(x => new
            {
                name = x.GetFriendName()??x.DeviceDesc()
            });
            Console.WriteLine($"dds count:{dds.Count()}");
            foreach (var d in dds)
            {
                Console.WriteLine($"name:{d.name}");
            }
            Console.WriteLine("Test1----");
            Assert.True(true);
        }
    }
}