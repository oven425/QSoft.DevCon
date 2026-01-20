// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using QSoft.DevCon;
//https://studyhost.blogspot.com/2025/06/net-mcp-server.html
var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly().WithPrompts;

await builder.Build().RunAsync();
[McpServerToolType]
public static class TestFunction
{
    [McpServerTool, Description("現在時間")]
    public static string GetCurrentDateTime()
    {
        var time = DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss.fff");
        return $"[RAW_TIME_DATA: {time}]";
        //return DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss.fff");
    }
    [McpServerTool, Description("相機資訊")]
    public static IEnumerable<string?> GetCameras()
    {
        return "Camera".Devices().Select(x => x.GetFriendName());
    }
}