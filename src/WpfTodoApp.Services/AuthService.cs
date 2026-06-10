using Microsoft.EntityFrameworkCore;
using Prism.Events;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Core.Interfaces;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

/// <summary>
/// 认证服务实现。使用 BCrypt 验证密码哈希，登录成功后发布 UserLoggedInEvent。
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger _logger;

    public AuthService(
        AppDbContext db,
        IEventAggregator eventAggregator,
        ILogger logger)
    {
        _db = db;
        _eventAggregator = eventAggregator;
        _logger = logger;
    }

    public AuthResult Authenticate(string username, string password)
    {
        _logger.Info($"用户登录尝试: {username}");

        var user = _db.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Username == username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.Warning($"登录失败: {username}");
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "用户名或密码错误"
            };
        }

        _logger.Info($"登录成功: {username}");

        return new AuthResult
        {
            Success = true,
            IsFirstLogin = user.IsFirstLogin,
            IsAdmin = user.IsAdmin,
            UserId = user.Id,
            Username = user.Username
        };
    }

    public ChangePasswordResult ChangePassword(
        string username, string oldPassword, string newPassword)
    {
        if (newPassword.Length < 6)
        {
            return new ChangePasswordResult
            {
                Success = false,
                ErrorMessage = "密码长度不能少于6位"
            };
        }

        if (!newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
        {
            return new ChangePasswordResult
            {
                Success = false,
                ErrorMessage = "密码必须包含字母和数字"
            };
        }

        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user is null)
        {
            return new ChangePasswordResult
            {
                Success = false,
                ErrorMessage = "用户不存在"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return new ChangePasswordResult
            {
                Success = false,
                ErrorMessage = "旧密码不正确"
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.IsFirstLogin = false;
        _db.SaveChanges();

        _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(
            new UserLoggedInEvent.Payload(user.Id, user.Username, user.IsAdmin));

        _logger.Info($"密码修改成功: {username}");

        return new ChangePasswordResult { Success = true };
    }
}
