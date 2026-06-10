using Prism.Events;

namespace WpfTodoApp.Core.Events;

/// <summary>
/// 用户登录成功事件。通过 Prism IEventAggregator 发布，
/// MainWindow 和 TodoList 模块订阅此事件以初始化界面。
/// </summary>
public class UserLoggedInEvent : PubSubEvent<UserLoggedInEvent.Payload>
{
    /// <summary>事件载荷：登录用户的基本信息。</summary>
    public record Payload(int UserId, string Username, bool IsAdmin);
}
