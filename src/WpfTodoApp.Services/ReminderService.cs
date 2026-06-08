using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

public class ReminderService : IReminderService
{
    private readonly IContainerProvider _container;
    private readonly IEventAggregator _eventAggregator;
    private System.Timers.Timer? _timer;
    private bool _disposed;

    public ReminderService(IContainerProvider container, IEventAggregator eventAggregator)
    { _container = container; _eventAggregator = eventAggregator; }

    public void StartMonitoring()
    {
        CheckMissedReminders();
        _timer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
        _timer.Elapsed += (_, _) => CheckMissedReminders();
        _timer.AutoReset = true; _timer.Start();
    }

    public void StopMonitoring() { _timer?.Stop(); _timer?.Dispose(); _timer = null; }

    public List<TodoItemDto> CheckMissedReminders()
    {
        var db = _container.Resolve<AppDbContext>();
        try
        {
            var now = DateTime.UtcNow; var threshold = now.AddMinutes(10);
            var dueTodos = db.Set<Core.Models.TodoItem>().AsNoTracking()
                .Where(t => !t.IsCompleted && t.DueDate.HasValue
                    && t.DueDate.Value <= threshold && !t.ReminderSent).ToList();
            var todoEvent = _eventAggregator.GetEvent<TodoDueReminderEvent>();
            var result = new List<TodoItemDto>();
            foreach (var todo in dueTodos)
            {
                todoEvent.Publish(new TodoDueReminderEvent.Payload(todo.Id, todo.Title, todo.DueDate!.Value));
                var entity = db.Set<Core.Models.TodoItem>().Find(todo.Id);
                if (entity is not null) entity.ReminderSent = true;
                result.Add(new TodoItemDto(todo.Id, todo.Title, todo.Description,
                    todo.CreatedAt, todo.DueDate, todo.Priority, todo.IsCompleted, todo.UserId));
            }
            db.SaveChanges(); return result;
        }
        finally { db.Dispose(); }
    }

    public void Dispose()
    {
        if (!_disposed) { _disposed = true; StopMonitoring(); }
    }
}
