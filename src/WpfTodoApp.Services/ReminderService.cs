using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Modules.Logging;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

/// <summary>
/// 提醒服务实现。使用 System.Timers.Timer 每分钟轮询一次，
/// 检查未完成且有截止日期的任务，在到期前10分钟触发 TodoDueReminderEvent。
/// </summary>
public class ReminderService : IReminderService
{
    private readonly IContainerProvider _container;
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger _logger;
    private System.Timers.Timer? _timer;
    private bool _disposed;

    public ReminderService(
        IContainerProvider container,
        IEventAggregator eventAggregator,
        ILogger logger)
    {
        _container = container;
        _eventAggregator = eventAggregator;
        _logger = logger;
    }

    public void StartMonitoring()
    {
        _logger.Info("提醒服务启动");

        CheckMissedReminders();

        _timer = new System.Timers.Timer(
            TimeSpan.FromMinutes(1).TotalMilliseconds);
        _timer.Elapsed += (_, _) => CheckMissedReminders();
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void StopMonitoring()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        _logger.Info("提醒服务停止");
    }

    public List<TodoItemDto> CheckMissedReminders()
    {
        var db = _container.Resolve<AppDbContext>();

        try
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddMinutes(10);

            var dueTodos = db.Set<Core.Models.TodoItem>()
                .AsNoTracking()
                .Where(t => !t.IsCompleted
                    && t.DueDate.HasValue
                    && t.DueDate.Value <= threshold
                    && !t.ReminderSent)
                .ToList();

            var todoEvent = _eventAggregator
                .GetEvent<TodoDueReminderEvent>();

            foreach (var todo in dueTodos)
            {
                _logger.Info($"到期提醒: [{todo.Id}] {todo.Title}");

                todoEvent.Publish(new TodoDueReminderEvent.Payload(
                    todo.Id, todo.Title, todo.DueDate!.Value));

                var entity = db.Set<Core.Models.TodoItem>().Find(todo.Id);
                if (entity is not null)
                    entity.ReminderSent = true;
            }

            db.SaveChanges();

            return dueTodos.Select(t => new TodoItemDto(
                t.Id, t.Title, t.Description, t.CreatedAt,
                t.DueDate, t.Priority, t.IsCompleted, t.UserId)).ToList();
        }
        finally
        {
            db.Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        StopMonitoring();
    }
}
