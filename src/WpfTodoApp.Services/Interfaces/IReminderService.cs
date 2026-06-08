using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

public interface IReminderService : IDisposable
{
    void StartMonitoring();
    void StopMonitoring();
    List<TodoItemDto> CheckMissedReminders();
}
