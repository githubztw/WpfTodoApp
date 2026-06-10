using WpfTodoApp.Core.Interfaces;

namespace WpfTodoApp.Modules.Logging;

/// <summary>
/// 日志模块 — 仅负责注册 ILogger 实现到 DI 容器。
/// Serilog 初始化在启动模块 (App.xaml.cs) 中完成。
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
