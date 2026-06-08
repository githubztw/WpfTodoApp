namespace WpfTodoApp.Core.Dtos;

public record UserDto(int Id, string Username, bool IsAdmin, bool IsFirstLogin);

public class CreateUserResult
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class UpdatePasswordResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DeleteUserResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
