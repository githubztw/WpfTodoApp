namespace WpfTodoApp.Core.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public bool IsCompleted { get; set; }
    public bool ReminderSent { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}
