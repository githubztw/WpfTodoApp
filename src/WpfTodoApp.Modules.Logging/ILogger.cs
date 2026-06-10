namespace WpfTodoApp.Modules.Logging;

/// <summary>
/// 统一日志接口。所有模块通过此接口记录日志。
/// 接口定义在 Logging 模块中，避免模块间不必要的依赖。
/// </summary>
public interface ILogger
{
    /// <summary>记录调试级别日志。</summary>
    void Debug(string message);

    /// <summary>记录信息级别日志。</summary>
    void Info(string message);

    /// <summary>记录警告级别日志。</summary>
    void Warning(string message);

    /// <summary>记录错误级别日志，可附带异常对象。</summary>
    void Error(string message, Exception? ex = null);
}
