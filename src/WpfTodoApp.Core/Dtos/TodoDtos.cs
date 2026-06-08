using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Core.Dtos;

public enum TodoFilter { All, Completed, Incomplete }

public record TodoItemDto(
    int Id, string Title, string? Description, DateTime CreatedAt,
    DateTime? DueDate, TodoPriority Priority, bool IsCompleted, int UserId);
