using Microsoft.Management.Infrastructure;
using QSoft.DevCon;

namespace TestDevCon
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            using CimSession session = CimSession.Create(null);
            string query = "SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera'";
            var instances = session.QueryInstances(@"root\cimv2", "WQL", query);
            var aa = instances.Select(x => new {
                id = x.CimInstanceProperties["DeviceID"].Value as string,
                name = x.CimInstanceProperties["Caption"].Value as string,
                hardwareIDs = x.CimInstanceProperties["HardwareID"].Value as string[],
            }).ToList();


            var bb = "Camera".Devices().Select(x => new
            {
                id = x.DeviceInstanceId(),
                name = x.GetFriendName(),
                hardwareIDs = x.HardwareIDs()
            }).ToList();

            var ass = aa.Join(bb, x => x.id, y => y.id, (x, y) =>new
            {
                x,
                y
            });
        }
    }
}