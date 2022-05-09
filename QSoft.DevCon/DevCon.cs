using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.DevCon
{
    public class DeviceControl
    {
        public void Where(Expression<Func<DeviceInfo, bool>> func)
        {

        }
    }

    public class DeviceInfo
    {
        public string HardwareID { set; get; }
    }
}
