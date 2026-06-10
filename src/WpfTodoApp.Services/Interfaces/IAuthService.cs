using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

/// <summary>
/// 认证服务接口。提供用户登录验证和密码修改功能。
/// </summary>
public interface IAuthService
{
    /// <summary>验证用户凭证。成功返回用户信息，失败返回错误消息。</summary>
    AuthResult Authenticate(string username, string password);

    /// <summary>修改用户密码。验证旧密码后更新为新密码。</summary>
    ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword);
}
