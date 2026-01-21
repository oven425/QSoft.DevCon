// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using QSoft.DevCon;
using static QSoft.DevCon.DevConExtension;
//https://studyhost.blogspot.com/2025/06/net-mcp-server.html
var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
[McpServerToolType]
public static class TestFunction
{
    [McpServerTool, Description("相機完整資訊")]
    public static IEnumerable<DeviceInfo> GetCameras()
    {
        return "Camera".Devices().Select(x => new DeviceInfo()
        {
            DeviceDesc = x.DeviceDesc(),
            FriendlyName = x.GetFriendName()??"",
            InstanceId = x.DeviceInstanceId(),
            IsPresent = x.IsPresent()
        });
    }

    [McpServerTool, Description("相機部分資訊")]
    public static IEnumerable<object> GetCameras1([Description("選填，指定需要的欄位以節省流量。例如：['FriendlyName', 'IsPresent']")] string[] fileds)
    {
        return "Camera".Devices().Select(x => GetPartial(x, fileds));
    }

    static Dictionary<string, object> GetPartial((IntPtr dev, SP_DEVINFO_DATA devdata) src, string[] fileds)
    {
        var dic = new Dictionary<string, object>();
        foreach (var oo in fileds)
        {
            var aa = oo switch
            {
                "DeviceDesc" => src.DeviceDesc(),
                "FriendlyName" =>src.GetFriendName()??"",
                "InstanceId"=>src.DeviceInstanceId(),
                //"IsPresent" =>src.IsPresent(),
                _ =>""
            };
            dic[oo] = aa;
        }
        return dic;
    }
}

public class DeviceInfo
{
    public string DeviceDesc { set; get; } = "";
    public string FriendlyName { set; get; } = "";
    public string InstanceId { set; get; } = "";
    public bool IsPresent { set; get; }
}