using WpfTodoApp.Modules.Logging;

namespace WpfTodoApp.Core;

/// <summary>
/// 所有模块 Manager 的统一基类。
/// Manager 是 ViewModel 和 Service 之间的中间层，负责：
/// 1. 封装业务逻辑编排
/// 2. 提供服务调用结果的标准处理
/// 3. 避免 ViewModel 直接依赖 Service
/// </summary>
public class ManagerBase
{
    protected ILogger Logger { get; }
    public ManagerBase(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// 统一错误处理：将异常转换为用户友好的错误消息。
    /// </summary>
    protected string HandleError(Exception ex)
    {
        return ex switch
        {
            UnauthorizedAccessException => "权限不足，无法执行此操作",
            InvalidOperationException ioe => ioe.Message,
            ArgumentException ae => ae.Message,
            _ => $"操作失败：{ex.Message}"
        };
    }
}
