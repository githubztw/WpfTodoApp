using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Models;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Modules.TodoList.Managers;

/// <summary>
/// 任务管理业务编排器。在调用 ITodoService 前进行输入验证。
/// </summary>
public class TodoManager : ManagerBase
{
    private readonly ITodoService _todoService;

    public TodoManager(ITodoService todoService)
    {
        _todoService = todoService;
    }

    public List<TodoItemDto> GetTodos(int userId, TodoFilter filter)
    {
        return _todoService.GetTodos(userId, filter);
    }

    public TodoItemDto CreateTodo(
        int userId,
        string title,
        string? description,
        DateTime? dueDate,
        TodoPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("任务标题不能为空");

        return _todoService.CreateTodo(
            userId, title, description, dueDate, priority);
    }

    public TodoItemDto UpdateTodo(
        int todoId,
        string title,
        string? description,
        DateTime? dueDate,
        TodoPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("任务标题不能为空");

        return _todoService.UpdateTodo(
            todoId, title, description, dueDate, priority);
    }

    public TodoItemDto ToggleComplete(int todoId)
    {
        return _todoService.ToggleComplete(todoId);
    }

    public bool DeleteTodo(int todoId)
    {
        return _todoService.DeleteTodo(todoId);
    }
}
