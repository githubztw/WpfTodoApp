namespace WpfTodoApp.Services.Config;

/// <summary>
/// 提醒服务配置。由启动程序从 appsettings.json 的 ReminderConfig 节加载。
/// </summary>
public class ReminderConfig
{
    /// <summary>后台轮询检查间隔（分钟）。</summary>
    public int CheckIntervalMinutes { get; set; } = 1;

    /// <summary>任务到期前提前提醒的时间（分钟）。</summary>
    public int RemindBeforeMinutes { get; set; } = 10;
}
