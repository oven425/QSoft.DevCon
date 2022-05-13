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
            List<Device> dds = new List<Device>();
            using (var mgr = new DevMgr())
            {
                mgr.AllDevice().Where(x => x.HardwareID != "").Enable();
            }
        }
    }

    public class DevMgr : IDisposable
    {
        SafeHandle m_Handle;
        void EnumData()
        {

        }
        public IEnumerable<Device> AllDevice()
        {
            return new List<Device>();
        }
        public void Dispose()
        {
        }
    }

    public class Device
    {
        public string HardwareID {internal set; get; }
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
