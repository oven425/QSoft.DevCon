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
            //List<Device> dds = new List<Device>();
            //using (var mgr = new DevMgr())
            //{
            //    mgr.AllDevice().Where(x => x.HardwareID != "").Enable();
            //}

            try
            {
                StringBuilder strb = new StringBuilder();
                var dd = new Dd();
                var dds = new DevMgr().AllDevice();
                foreach (var oo in dds)
                {
                    Console.WriteLine($"FriendlyName:{oo.FriendlyName}");
                    Console.WriteLine($"HardwareID:{oo.HardwareID}");
                    Console.WriteLine($"Description:{oo.Description}");
                    Console.WriteLine("");
                }
            }
            catch(Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
            }
            Console.ReadLine();
        }
    }

    public class Read:Exception
    {
        public override string ToString()
        {
            System.Diagnostics.Trace.WriteLine("ToString");
            return "ccccc";
        }
    }

    public class Dd
    {
        public string AA
        {
            get
            {
                return "";
            }
        }
    }

    
}
