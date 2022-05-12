using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Device> dds = new List<Device>();
            dds.Enable();
        }
    }

    public class Device
    {
        public string HardwareID { set; get; }
        public void Enable()
        {

        }
    }

    public static class IDeviceEnumable
    {
        public static int Enable(this IEnumerable<Device> src)
        {
            foreach(var oo in src)
            {
                oo.Enable();
            }
            return 0;
        }
    }
}
