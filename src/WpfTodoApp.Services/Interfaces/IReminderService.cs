using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

/// <summary>
/// 提醒服务接口。后台轮询检查到期任务并发送提醒。
/// </summary>
public interface IReminderService : IDisposable
{
    /// <summary>启动后台监控，每分钟检查一次到期任务。</summary>
    void StartMonitoring();

    /// <summary>停止后台监控并释放资源。</summary>
    void StopMonitoring();

    /// <summary>检查已过期未提醒的任务，补发提醒。</summary>
    List<TodoItemDto> CheckMissedReminders();
}
