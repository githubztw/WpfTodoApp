namespace WpfTodoApp.Core.Models;

/// <summary>
/// 办事项（任务）实体。每个任务属于一个用户。
/// </summary>
public class TodoItem
{
    /// <summary>任务唯一标识。</summary>
    public int Id { get; set; }

    /// <summary>任务标题（必填，最大200字符）。</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>任务详细描述（可选，最大1000字符）。</summary>
    public string? Description { get; set; }

    /// <summary>创建时间（UTC），用于默认排序。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>截止日期（可选）。设置后触发到期提醒。</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>优先级：Low=0, Medium=1, High=2。</summary>
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    /// <summary>是否已完成。</summary>
    public bool IsCompleted { get; set; }

    /// <summary>是否已发送到期提醒。防止重复提醒。</summary>
    public bool ReminderSent { get; set; }

    /// <summary>所属用户的外键。</summary>
    public int UserId { get; set; }

    /// <summary>所属用户导航属性。</summary>
    public User? User { get; set; }
}
