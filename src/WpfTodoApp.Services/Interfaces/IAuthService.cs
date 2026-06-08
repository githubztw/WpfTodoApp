using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

public interface IAuthService
{
    AuthResult Authenticate(string username, string password);
    ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword);
}
