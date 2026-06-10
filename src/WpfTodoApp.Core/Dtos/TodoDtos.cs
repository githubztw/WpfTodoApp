using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Core.Dtos;

/// <summary>任务列表筛选条件。</summary>
public enum TodoFilter { All, Completed, Incomplete }

/// <summary>任务数据传输对象（Service 层返回给 ViewModel 的数据）。</summary>
public record TodoItemDto(
    int Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime? DueDate,
    TodoPriority Priority,
    bool IsCompleted,
    int UserId);
