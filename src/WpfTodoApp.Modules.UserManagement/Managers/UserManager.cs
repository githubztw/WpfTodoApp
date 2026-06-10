using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Modules.UserManagement.Managers;

/// <summary>
/// 用户管理业务编排器。封装 IUserService 调用，统一管理用户操作。
/// </summary>
public class UserManager : ManagerBase
{
    private readonly IUserService _userService;

    public UserManager(IUserService userService)
    {
        _userService = userService;
    }

    public List<UserDto> GetAllUsers(int requestingUserId)
    {
        return _userService.GetAllUsers(requestingUserId);
    }

    public CreateUserResult CreateUser(
        int requestingUserId, string username, string password)
    {
        return _userService.CreateUser(requestingUserId, username, password);
    }

    public UpdatePasswordResult UpdateUserPassword(
        int requestingUserId, int userId, string newPassword)
    {
        return _userService.UpdateUserPassword(
            requestingUserId, userId, newPassword);
    }

    public DeleteUserResult DeleteUser(int requestingUserId, int userId)
    {
        return _userService.DeleteUser(requestingUserId, userId);
    }
}
