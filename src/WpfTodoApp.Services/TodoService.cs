using Microsoft.EntityFrameworkCore;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Models;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _db;
    public TodoService(AppDbContext db) => _db = db;

    public List<TodoItemDto> GetTodos(int userId, TodoFilter filter)
    {
        var query = _db.TodoItems.AsNoTracking().Where(t => t.UserId == userId);
        query = filter switch
        {
            TodoFilter.Completed => query.Where(t => t.IsCompleted),
            TodoFilter.Incomplete => query.Where(t => !t.IsCompleted),
            _ => query
        };
        return query.OrderByDescending(t => t.CreatedAt).Select(t => Map(t)).ToList();
    }

    public TodoItemDto CreateTodo(int userId, string title, string? description, DateTime? dueDate, TodoPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("任务标题不能为空");
        var todo = new TodoItem { Title = title.Trim(), Description = description?.Trim(), CreatedAt = DateTime.UtcNow, DueDate = dueDate, Priority = priority, UserId = userId };
        _db.TodoItems.Add(todo); _db.SaveChanges();
        return Map(todo);
    }

    public TodoItemDto UpdateTodo(int todoId, string title, string? description, DateTime? dueDate, TodoPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("任务标题不能为空");
        var todo = _db.TodoItems.Find(todoId) ?? throw new InvalidOperationException("任务不存在");
        todo.Title = title.Trim(); todo.Description = description?.Trim(); todo.DueDate = dueDate; todo.Priority = priority;
        _db.SaveChanges();
        return Map(todo);
    }

    public TodoItemDto ToggleComplete(int todoId)
    {
        var todo = _db.TodoItems.Find(todoId) ?? throw new InvalidOperationException("任务不存在");
        todo.IsCompleted = !todo.IsCompleted; _db.SaveChanges();
        return Map(todo);
    }

    public bool DeleteTodo(int todoId)
    {
        var todo = _db.TodoItems.Find(todoId);
        if (todo is null) return false;
        _db.TodoItems.Remove(todo); _db.SaveChanges();
        return true;
    }

    private static TodoItemDto Map(TodoItem t) => new(t.Id, t.Title, t.Description, t.CreatedAt, t.DueDate, t.Priority, t.IsCompleted, t.UserId);
}
