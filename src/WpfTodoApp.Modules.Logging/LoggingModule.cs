namespace WpfTodoApp.Modules.Logging;

/// <summary>
/// 日志模块。暴露 SerilogLogger 实例供启动模块注册到 DI 容器。
/// Serilog 初始化由启动模块 App.xaml.cs 通过 serilog.json 配置完成。
/// </summary>
public class LoggingModule
{
    private readonly ILogger _logger;

    public LoggingModule()
    {
        _logger = new SerilogLogger();
    }

    public ILogger Logger => _logger;
}
