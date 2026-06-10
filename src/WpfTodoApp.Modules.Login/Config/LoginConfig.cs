namespace WpfTodoApp.Modules.Login.Config;

/// <summary>
/// 登录模块配置。由启动程序从 appsettings.json 的 LoginConfig 节加载。
/// </summary>
public class LoginConfig
{
    /// <summary>默认管理员用户名。</summary>
    public string DefaultAdminUsername { get; set; } = "admin";

    /// <summary>默认管理员初始密码（仅首次使用）。</summary>
    public string DefaultAdminPassword { get; set; } = string.Empty;

    /// <summary>密码最小长度要求。</summary>
    public int PasswordMinLength { get; set; } = 6;
}
