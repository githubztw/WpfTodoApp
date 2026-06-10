using Prism.Events;

namespace WpfTodoApp.Core.Events;

/// <summary>
/// 任务到期提醒事件。ReminderService 在任务到期前10分钟发布，
/// MainWindow 订阅并弹出桌面通知。
/// </summary>
public class TodoDueReminderEvent : PubSubEvent<TodoDueReminderEvent.Payload>
{
    /// <summary>事件载荷：到期任务信息。</summary>
    public record Payload(int TodoId, string Title, DateTime DueDate);
}
