namespace WpfTodoApp.Core.Config;

/// <summary>
/// Core 模块配置实现。由启动程序从 appsettings.json 加载后赋值。
/// </summary>
public class CoreConfig
{
    /// <summary>系统默认管理员用户名。</summary>
    public string SystemName { get; set; } = "admin";

    /// <summary>系统默认管理员初始密码。</summary>
    public string SystemPassword { get; set; } = string.Empty;
}
