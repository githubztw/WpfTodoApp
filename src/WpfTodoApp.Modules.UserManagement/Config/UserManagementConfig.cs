namespace WpfTodoApp.Modules.UserManagement.Config;

/// <summary>
/// 用户管理模块配置。由启动程序从 appsettings.json 的 UserManagementConfig 节加载。
/// </summary>
public class UserManagementConfig
{
    /// <summary>用户名最小长度。</summary>
    public int UsernameMinLength { get; set; } = 3;

    /// <summary>用户名最大长度。</summary>
    public int UsernameMaxLength { get; set; } = 50;

    /// <summary>密码重置时的默认密码。</summary>
    public string ResetPassword { get; set; } = "Pass123";
}
