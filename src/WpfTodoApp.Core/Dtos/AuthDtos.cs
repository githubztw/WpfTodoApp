namespace WpfTodoApp.Core.Dtos;

/// <summary>认证操作返回结果。</summary>
public class AuthResult
{
    /// <summary>认证是否成功。</summary>
    public bool Success { get; set; }

    /// <summary>是否需要首次登录密码修改。</summary>
    public bool IsFirstLogin { get; set; }

    /// <summary>用户是否为管理员。</summary>
    public bool IsAdmin { get; set; }

    /// <summary>认证成功后的用户ID。</summary>
    public int UserId { get; set; }

    /// <summary>认证成功后的用户名。</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>失败时的错误消息。</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>密码修改操作返回结果。</summary>
public class ChangePasswordResult
{
    /// <summary>修改是否成功。</summary>
    public bool Success { get; set; }

    /// <summary>失败时的错误消息。</summary>
    public string? ErrorMessage { get; set; }
}
