using Microsoft.EntityFrameworkCore;
using Prism.Events;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IEventAggregator _eventAggregator;

    public AuthService(AppDbContext db, IEventAggregator eventAggregator)
    {
        _db = db; _eventAggregator = eventAggregator;
    }

    public AuthResult Authenticate(string username, string password)
    {
        var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new AuthResult { Success = false, ErrorMessage = "用户名或密码错误" };
        return new AuthResult
        {
            Success = true, IsFirstLogin = user.IsFirstLogin,
            IsAdmin = user.IsAdmin, UserId = user.Id, Username = user.Username
        };
    }

    public ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword)
    {
        if (newPassword.Length < 6)
            return new ChangePasswordResult { Success = false, ErrorMessage = "密码长度不能少于6位" };
        if (!newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
            return new ChangePasswordResult { Success = false, ErrorMessage = "密码必须包含字母和数字" };
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user is null)
            return new ChangePasswordResult { Success = false, ErrorMessage = "用户不存在" };
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            return new ChangePasswordResult { Success = false, ErrorMessage = "旧密码不正确" };
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.IsFirstLogin = false;
        _db.SaveChanges();
        _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(
            new UserLoggedInEvent.Payload(user.Id, user.Username, user.IsAdmin));
        return new ChangePasswordResult { Success = true };
    }
}
