using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Services.Interfaces;

public interface ITodoService
{
    List<TodoItemDto> GetTodos(int userId, TodoFilter filter);
    TodoItemDto CreateTodo(int userId, string title, string? description,
        DateTime? dueDate, TodoPriority priority);
    TodoItemDto UpdateTodo(int todoId, string title, string? description,
        DateTime? dueDate, TodoPriority priority);
    TodoItemDto ToggleComplete(int todoId);
    bool DeleteTodo(int todoId);
}
