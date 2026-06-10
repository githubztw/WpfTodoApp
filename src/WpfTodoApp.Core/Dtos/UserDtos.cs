namespace WpfTodoApp.Core.Dtos;

/// <summary>用户数据传输对象（不含密码哈希）。</summary>
public record UserDto(int Id, string Username, bool IsAdmin, bool IsFirstLogin);

/// <summary>创建用户操作返回结果。</summary>
public class CreateUserResult
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>更新密码操作返回结果。</summary>
public class UpdatePasswordResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>删除用户操作返回结果。</summary>
public class DeleteUserResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
