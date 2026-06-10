using Microsoft.EntityFrameworkCore;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Modules.Logging;
using WpfTodoApp.Core.Models;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

/// <summary>
/// 办事项服务实现。提供任务的增删改查和状态切换。
/// </summary>
public class TodoService : ITodoService
{
    private readonly AppDbContext _db;
    private readonly ILogger _logger;

    public TodoService(AppDbContext db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public List<TodoItemDto> GetTodos(int userId, TodoFilter filter)
    {
        var query = _db.TodoItems
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        query = filter switch
        {
            TodoFilter.Completed => query.Where(t => t.IsCompleted),
            TodoFilter.Incomplete => query.Where(t => !t.IsCompleted),
            _ => query
        };

        return query
            .OrderByDescending(t => t.CreatedAt)
            .Select(Map)
            .ToList();
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

        var todo = new TodoItem
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate,
            Priority = priority,
            UserId = userId
        };

        _db.TodoItems.Add(todo);
        _db.SaveChanges();

        _logger.Info($"创建任务: [{todo.Id}] {todo.Title}");

        return Map(todo);
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

        var todo = _db.TodoItems.Find(todoId)
            ?? throw new InvalidOperationException("任务不存在");

        todo.Title = title.Trim();
        todo.Description = description?.Trim();
        todo.DueDate = dueDate;
        todo.Priority = priority;

        _db.SaveChanges();

        _logger.Info($"更新任务: [{todo.Id}] {todo.Title}");

        return Map(todo);
    }

    public TodoItemDto ToggleComplete(int todoId)
    {
        var todo = _db.TodoItems.Find(todoId)
            ?? throw new InvalidOperationException("任务不存在");

        todo.IsCompleted = !todo.IsCompleted;
        _db.SaveChanges();

        var status = todo.IsCompleted ? "完成" : "未完成";
        _logger.Info($"任务状态变更: [{todo.Id}] {status}");

        return Map(todo);
    }

    public bool DeleteTodo(int todoId)
    {
        var todo = _db.TodoItems.Find(todoId);
        if (todo is null)
            return false;

        _db.TodoItems.Remove(todo);
        _db.SaveChanges();

        _logger.Info($"删除任务: [{todo.Id}] {todo.Title}");

        return true;
    }

    private static TodoItemDto Map(TodoItem t)
    {
        return new TodoItemDto(
            t.Id, t.Title, t.Description, t.CreatedAt,
            t.DueDate, t.Priority, t.IsCompleted, t.UserId);
    }
}
