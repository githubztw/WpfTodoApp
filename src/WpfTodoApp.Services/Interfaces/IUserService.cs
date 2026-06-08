using WpfTodoApp.Core.Dtos;

namespace WpfTodoApp.Services.Interfaces;

public interface IUserService
{
    List<UserDto> GetAllUsers(int requestingUserId);
    CreateUserResult CreateUser(int requestingUserId, string username, string password);
    UpdatePasswordResult UpdateUserPassword(int requestingUserId, int userId, string newPassword);
    DeleteUserResult DeleteUser(int requestingUserId, int userId);
}
