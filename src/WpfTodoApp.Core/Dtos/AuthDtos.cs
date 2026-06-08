namespace WpfTodoApp.Core.Dtos;

public class AuthResult
{
    public bool Success { get; set; }
    public bool IsFirstLogin { get; set; }
    public bool IsAdmin { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class ChangePasswordResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
