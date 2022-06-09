using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                //var count = new DevMgr().Enable(x => x.InstanceId == "aaaaa");
                var dds = new DevMgr().AllDevice();
                foreach (var item in dds)
                {
                    System.Diagnostics.Trace.WriteLine(item.Description);
                }

                var groups = new DevMgr().AllDevice().GroupBy(x=>x.Class);
                foreach (var group in groups)
                {
                    System.Diagnostics.Trace.WriteLine(group.Key);
                    foreach(var item in group)
                    {
                        System.Diagnostics.Trace.WriteLine(item.Description);
                    }
                }
                
                var disable_count = new DevMgr().Disable(x => x.Description.Contains("Camera"));

                var enable_count = new DevMgr().Enable(x => x.Description.Contains("Camera"));
                //var dds = new DevMgr().AllDevice().Where(x => x.Description.Contains("Camera"));
                //var dds = new DevMgr().AllPath();
                //var lookup = dds.ToLookup(x => x.DeviceInfo);
            }
            catch(Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
            }
            Console.ReadLine();
        }
    }

    
}
