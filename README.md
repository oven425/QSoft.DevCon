# Simple example
```c#
//get all device
var dds = new DevMgr().AllDevice();
foreach(var item in dds)
{
    System.Diagnostics.Trace.WriteLine(item.Description);
}

//group by device type
var groups = new DevMgr().AllDevice().GroupBy(x=>x.Class);
foreach (var group in groups)
{
    System.Diagnostics.Trace.WriteLine(group.Key);
    foreach(var item in group)
    {
        System.Diagnostics.Trace.WriteLine(item.Description);
    }
}
                
//disable device
var disable_count = new DevMgr().Disable(x => x.Description.Contains("Camera"));

//enable device 
var enable_count = new DevMgr().Enable(x => x.Description.Contains("Camera"));
```
PS: Thanks for [Simple Device Manager](https://www.codeproject.com/Articles/14469/Simple-Device-Manager).
