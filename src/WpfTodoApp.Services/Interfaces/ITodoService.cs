using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Services.Interfaces;

/// <summary>
/// 办事项服务接口。提供任务的 CRUD 操作和筛选功能。
/// </summary>
public interface ITodoService
{
    /// <summary>获取指定用户的任务列表，支持按完成状态筛选。</summary>
    List<TodoItemDto> GetTodos(int userId, TodoFilter filter);

    /// <summary>创建新任务。</summary>
    TodoItemDto CreateTodo(int userId, string title, string? description,
        DateTime? dueDate, TodoPriority priority);

    /// <summary>更新已有任务。</summary>
    TodoItemDto UpdateTodo(int todoId, string title, string? description,
        DateTime? dueDate, TodoPriority priority);

    /// <summary>切换任务的完成状态。</summary>
    TodoItemDto ToggleComplete(int todoId);

    /// <summary>删除任务。返回 false 表示任务不存在。</summary>
    bool DeleteTodo(int todoId);
}
