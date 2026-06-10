using Prism.Commands;
using Prism.Regions;
using WpfTodoApp.Core;
using WpfTodoApp.Modules.Logging;
using WpfTodoApp.Modules.Login.Managers;

namespace WpfTodoApp.Modules.Login.ViewModels;

/// <summary>
/// 密码修改界面 ViewModel。通过 INavigationAware 接收登录传递的用户名。
/// </summary>
public class ChangePasswordViewModel : ViewModelBase, INavigationAware
{
    private readonly LoginManager _loginManager;
    private readonly IRegionManager _regionManager;
    private string _username = string.Empty;

    private string _oldPassword = string.Empty;
    public string OldPassword
    {
        get => _oldPassword;
        set => SetProperty(ref _oldPassword, value);
    }

    private string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set
        {
            SetProperty(ref _newPassword, value);
            RaisePropertyChanged(nameof(ConfirmPassword));
        }
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand ChangePasswordCommand { get; }

    public ChangePasswordViewModel(
        LoginManager loginManager,
        IRegionManager regionManager,
        ILogger logger)
        : base(logger)
    {
        _loginManager = loginManager;
        _regionManager = regionManager;

        ChangePasswordCommand = new DelegateCommand(ExecuteChangePassword);
    }

    private void ExecuteChangePassword()
    {
        ErrorMessage = null;

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "两次输入的密码不一致";
            return;
        }

        var result = _loginManager.ChangePassword(
            _username, _oldPassword, NewPassword);

        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        Logger.Info($"密码修改成功: {_username}");
        _regionManager.RequestNavigate("ContentRegion", "LoginView");
    }

    public void OnNavigatedTo(NavigationContext ctx)
    {
        _username = ctx.Parameters.GetValue<string>("username");
    }

    public bool IsNavigationTarget(NavigationContext ctx)
    {
        return true;
    }

    public void OnNavigatedFrom(NavigationContext ctx)
    {
    }
}
