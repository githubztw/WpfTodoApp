using System.Collections.ObjectModel;
using Prism.Commands;
using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Modules.Logging;
using WpfTodoApp.Modules.UserManagement.Managers;

namespace WpfTodoApp.Modules.UserManagement.ViewModels;

/// <summary>
/// 用户管理界面 ViewModel。管理员可查看、创建、重置密码和删除用户。
/// </summary>
public class UserManagementViewModel : ViewModelBase, IDisposable
{
    private readonly UserManager _userManager;
    private int _requestingUserId;

    public ObservableCollection<UserDto> Users { get; } = new();

    private string _newUsername = string.Empty;
    public string NewUsername
    {
        get => _newUsername;
        set => SetProperty(ref _newUsername, value);
    }

    private string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string? _successMessage;
    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public DelegateCommand LoadUsersCommand { get; }
    public DelegateCommand AddUserCommand { get; }
    public DelegateCommand<UserDto> EditPasswordCommand { get; }
    public DelegateCommand<UserDto> DeleteUserCommand { get; }

    public UserManagementViewModel(UserManager userManager, ILogger logger)
        : base(logger)
    {
        _userManager = userManager;

        LoadUsersCommand = new DelegateCommand(ExecuteLoadUsers);
        AddUserCommand = new DelegateCommand(ExecuteAddUser);
        EditPasswordCommand = new DelegateCommand<UserDto>(ExecuteEditPassword);
        DeleteUserCommand = new DelegateCommand<UserDto>(ExecuteDeleteUser);
    }

    public void SetUser(int userId, bool isAdmin)
    {
        _requestingUserId = userId;
        if (isAdmin)
            ExecuteLoadUsers();
    }

    private void ExecuteLoadUsers()
    {
        ErrorMessage = null;

        try
        {
            var users = _userManager.GetAllUsers(_requestingUserId);
            Users.Clear();
            foreach (var user in users)
                Users.Add(user);
        }
        catch (Exception ex)
        {
            Logger.Error("加载用户失败", ex);
            ErrorMessage = ex.Message;
        }
    }

    private void ExecuteAddUser()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var result = _userManager.CreateUser(
            _requestingUserId, NewUsername, NewPassword);

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        SuccessMessage = $"用户 '{NewUsername}' 创建成功";
        NewUsername = string.Empty;
        NewPassword = string.Empty;
        ExecuteLoadUsers();
    }

    private void ExecuteEditPassword(UserDto user)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var result = _userManager.UpdateUserPassword(
            _requestingUserId, user.Id, "Pass123");

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        SuccessMessage = $"用户 '{user.Username}' 密码已重置";
    }

    private void ExecuteDeleteUser(UserDto user)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var result = _userManager.DeleteUser(_requestingUserId, user.Id);

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        Users.Remove(user);
        SuccessMessage = $"用户 '{user.Username}' 已删除";
    }

    public void Dispose()
    {
    }
}
