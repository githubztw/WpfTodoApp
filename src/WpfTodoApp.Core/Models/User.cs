namespace WpfTodoApp.Core.Models;

/// <summary>
/// 系统用户实体。
/// </summary>
public class User
{
    /// <summary>用户唯一标识。</summary>
    public int Id { get; set; }

    /// <summary>登录用户名，系统中唯一。</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt 加密后的密码哈希，明文从不落盘。</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>是否为管理员。管理员可访问用户管理模块。</summary>
    public bool IsAdmin { get; set; }

    /// <summary>是否首次登录。首次登录时强制修改密码。</summary>
    public bool IsFirstLogin { get; set; }
}
