using Microsoft.Management.Infrastructure;
using QSoft.DevCon;
using System.Text;
using System.Xml;

namespace TestDevCon
{
    public class CameraTest
    {
        [Fact]
        public void Test()
        {
            using CimSession session = CimSession.Create(null);
            string query = "SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera'";
            var instances = session.QueryInstances(@"root\cimv2", "WQL", query);
            foreach(var oo in instances)
            {
                System.Diagnostics.Trace.WriteLine(oo);
            }
            var aa = instances.Select(x => new
            {
                id = x.CimInstanceProperties["DeviceID"].Value as string,
                name = x.CimInstanceProperties["Caption"].Value as string,
                hardwareIDs = x.CimInstanceProperties["HardwareID"].Value as string[],
                service = x.CimInstanceProperties["Service"].Value as string,
                compatibleIDs = x.CimInstanceProperties["CompatibleID"].Value as string[],
            });


            var bb = "Camera".Devices().Select(x => new
            {
                id = x.DeviceInstanceId(),
                name = x.GetFriendName(),
                hardwareIDs = x.HardwareIDs(),
                service = x.Service(),
                compatibleIDs = x.CompatibleIDs(),
            });


            Newtonsoft.Json.JsonConvert.SerializeObject(aa);
            string json_aa = Newtonsoft.Json.JsonConvert.SerializeObject(aa);
            string json_bb = Newtonsoft.Json.JsonConvert.SerializeObject(bb);
            Assert.Equal(json_aa, json_bb);

        }

    }


}