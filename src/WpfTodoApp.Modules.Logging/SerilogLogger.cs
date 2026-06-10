namespace WpfTodoApp.Modules.Logging;

/// <summary>
/// 基于 Serilog 的日志实现。包装 Serilog.Log 静态类。
/// 构造函数无需参数，日志输出目标由 Serilog 配置文件控制。
/// </summary>
public class SerilogLogger : ILogger
{
    public void Debug(string message) => Serilog.Log.Debug(message);
    public void Info(string message) => Serilog.Log.Information(message);
    public void Warning(string message) => Serilog.Log.Warning(message);

    public void Error(string message, Exception? ex = null)
    {
        if (ex is null)
            Serilog.Log.Error(message);
        else
            Serilog.Log.Error(ex, message);
    }
}
