using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Modules.UserManagement.Managers;

namespace WpfTodoApp.Modules.UserManagement.ViewModels;

public class UserManagementViewModel : BindableBase, IDisposable
{
    private readonly UserManager _userManager;
    private int _requestingUserId;

    public ObservableCollection<UserDto> Users { get; } = new();

    private string _newUsername = string.Empty;
    public string NewUsername { get => _newUsername; set => SetProperty(ref _newUsername, value); }

    private string _newPassword = string.Empty;
    public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private string? _successMessage;
    public string? SuccessMessage { get => _successMessage; set => SetProperty(ref _successMessage, value); }

    public DelegateCommand LoadUsersCommand { get; }
    public DelegateCommand AddUserCommand { get; }
    public DelegateCommand<UserDto> EditPasswordCommand { get; }
    public DelegateCommand<UserDto> DeleteUserCommand { get; }

    public UserManagementViewModel(UserManager userManager)
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
        if (isAdmin) ExecuteLoadUsers();
    }

    private void ExecuteLoadUsers()
    {
        ErrorMessage = null;
        try { Users.Clear(); foreach (var u in _userManager.GetAllUsers(_requestingUserId)) Users.Add(u); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    private void ExecuteAddUser()
    {
        ErrorMessage = null; SuccessMessage = null;
        var r = _userManager.CreateUser(_requestingUserId, NewUsername, NewPassword);
        if (!r.Success) { ErrorMessage = r.ErrorMessage; return; }
        SuccessMessage = $"用户 '{NewUsername}' 创建成功";
        NewUsername = string.Empty; NewPassword = string.Empty;
        ExecuteLoadUsers();
    }

    private void ExecuteEditPassword(UserDto user)
    {
        ErrorMessage = null; SuccessMessage = null;
        var r = _userManager.UpdateUserPassword(_requestingUserId, user.Id, "Pass123");
        if (!r.Success) { ErrorMessage = r.ErrorMessage; return; }
        SuccessMessage = $"用户 '{user.Username}' 密码已重置";
    }

    private void ExecuteDeleteUser(UserDto user)
    {
        ErrorMessage = null; SuccessMessage = null;
        var r = _userManager.DeleteUser(_requestingUserId, user.Id);
        if (!r.Success) { ErrorMessage = r.ErrorMessage; return; }
        Users.Remove(user);
        SuccessMessage = $"用户 '{user.Username}' 已删除";
    }

    public void Dispose() { }
}
