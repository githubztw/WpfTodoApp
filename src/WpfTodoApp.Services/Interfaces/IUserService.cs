using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

/// <summary>
/// 用户管理服务接口。仅管理员可调用。
/// </summary>
public interface IUserService
{
    /// <summary>获取所有用户列表（管理员权限）。</summary>
    List<UserDto> GetAllUsers(int requestingUserId);

    /// <summary>创建新用户（管理员权限）。</summary>
    CreateUserResult CreateUser(int requestingUserId, string username, string password);

    /// <summary>重置用户密码（管理员权限）。</summary>
    UpdatePasswordResult UpdateUserPassword(int requestingUserId, int userId, string newPassword);

    /// <summary>删除用户（管理员权限，不可删除自己和admin）。</summary>
    DeleteUserResult DeleteUser(int requestingUserId, int userId);
}
