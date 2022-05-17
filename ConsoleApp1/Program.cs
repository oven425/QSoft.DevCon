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

            new DevMgr().AllDevice().Where(x => x.HardwareID != null).Enable();
        }
    }

    //public class DevMgr : IDisposable
    //{
    //    SafeHandle m_Handle;
    //    void EnumData()
    //    {

    //    }
    //    public IEnumerable<Device> AllDevice()
    //    {
    //        return new List<Device>();
    //    }
    //    public void Dispose()
    //    {
    //    }
    //}

    
}
