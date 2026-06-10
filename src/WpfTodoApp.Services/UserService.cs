using Microsoft.EntityFrameworkCore;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Interfaces;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

/// <summary>
/// 用户管理服务实现。包含权限验证和输入校验。
/// admin 账户受保护，不可删除或降级。
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly ILogger _logger;

    public UserService(AppDbContext db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public List<UserDto> GetAllUsers(int requestingUserId)
    {
        var requestor = _db.Users.Find(requestingUserId);
        if (requestor is null || !requestor.IsAdmin)
            throw new UnauthorizedAccessException("仅管理员可访问用户管理");

        return _db.Users
            .AsNoTracking()
            .Select(u => new UserDto(
                u.Id, u.Username, u.IsAdmin, u.IsFirstLogin))
            .ToList();
    }

    public CreateUserResult CreateUser(
        int requestingUserId, string username, string password)
    {
        var requestor = _db.Users.Find(requestingUserId);
        if (requestor is null || !requestor.IsAdmin)
        {
            return new CreateUserResult
            {
                Success = false,
                ErrorMessage = "权限不足"
            };
        }

        if (string.IsNullOrWhiteSpace(username)
            || username.Length < 3
            || username.Length > 50
            || !username.All(c => char.IsLetterOrDigit(c) || c == '_'))
        {
            return new CreateUserResult
            {
                Success = false,
                ErrorMessage = "用户名须为3-50位字母、数字或下划线"
            };
        }

        if (_db.Users.Any(u => u.Username == username))
        {
            return new CreateUserResult
            {
                Success = false,
                ErrorMessage = "用户名已存在"
            };
        }

        if (password.Length < 6
            || !password.Any(char.IsLetter)
            || !password.Any(char.IsDigit))
        {
            return new CreateUserResult
            {
                Success = false,
                ErrorMessage = "密码必须至少6位且包含字母和数字"
            };
        }

        var user = new Core.Models.User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        _logger.Info($"创建用户: {username}");

        return new CreateUserResult { Success = true, UserId = user.Id };
    }

    public UpdatePasswordResult UpdateUserPassword(
        int requestingUserId, int userId, string newPassword)
    {
        var requestor = _db.Users.Find(requestingUserId);
        if (requestor is null || !requestor.IsAdmin)
        {
            return new UpdatePasswordResult
            {
                Success = false,
                ErrorMessage = "权限不足"
            };
        }

        if (newPassword.Length < 6
            || !newPassword.Any(char.IsLetter)
            || !newPassword.Any(char.IsDigit))
        {
            return new UpdatePasswordResult
            {
                Success = false,
                ErrorMessage = "密码必须至少6位且包含字母和数字"
            };
        }

        var user = _db.Users.Find(userId);
        if (user is null)
        {
            return new UpdatePasswordResult
            {
                Success = false,
                ErrorMessage = "用户不存在"
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _db.SaveChanges();

        _logger.Info($"重置密码: {user.Username}");

        return new UpdatePasswordResult { Success = true };
    }

    public DeleteUserResult DeleteUser(int requestingUserId, int userId)
    {
        var requestor = _db.Users.Find(requestingUserId);
        if (requestor is null || !requestor.IsAdmin)
        {
            return new DeleteUserResult
            {
                Success = false,
                ErrorMessage = "权限不足"
            };
        }

        if (requestingUserId == userId)
        {
            return new DeleteUserResult
            {
                Success = false,
                ErrorMessage = "无法删除自己的账户"
            };
        }

        var user = _db.Users.Find(userId);
        if (user is null)
        {
            return new DeleteUserResult
            {
                Success = false,
                ErrorMessage = "用户不存在"
            };
        }

        if (user.Username == "admin")
        {
            return new DeleteUserResult
            {
                Success = false,
                ErrorMessage = "无法删除内置管理员账户"
            };
        }

        _db.Users.Remove(user);
        _db.SaveChanges();

        _logger.Info($"删除用户: {user.Username}");

        return new DeleteUserResult { Success = true };
    }
}
