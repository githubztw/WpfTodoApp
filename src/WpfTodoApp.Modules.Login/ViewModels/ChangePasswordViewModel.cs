using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using WpfTodoApp.Modules.Login.Managers;

namespace WpfTodoApp.Modules.Login.ViewModels;

public class ChangePasswordViewModel : BindableBase, INavigationAware
{
    private readonly LoginManager _loginManager;
    private readonly IRegionManager _regionManager;
    private string _username = string.Empty;

    private string _oldPassword = string.Empty;
    public string OldPassword { get => _oldPassword; set => SetProperty(ref _oldPassword, value); }

    private string _newPassword = string.Empty;
    public string NewPassword { get => _newPassword; set { SetProperty(ref _newPassword, value); RaisePropertyChanged(nameof(ConfirmPassword)); } }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public DelegateCommand ChangePasswordCommand { get; }

    public ChangePasswordViewModel(LoginManager loginManager, IRegionManager regionManager)
    {
        _loginManager = loginManager;
        _regionManager = regionManager;
        ChangePasswordCommand = new DelegateCommand(ExecuteChangePassword);
    }

    private void ExecuteChangePassword()
    {
        ErrorMessage = null;
        if (NewPassword != ConfirmPassword) { ErrorMessage = "两次输入的密码不一致"; return; }
        var result = _loginManager.ChangePassword(_username, _oldPassword, NewPassword);
        if (!result.Success) { ErrorMessage = result.ErrorMessage; return; }
        _regionManager.RequestNavigate("ContentRegion", "LoginView");
    }

    public void OnNavigatedTo(NavigationContext ctx)
    {
        _username = ctx.Parameters.GetValue<string>("username");
    }

    public bool IsNavigationTarget(NavigationContext ctx) => true;
    public void OnNavigatedFrom(NavigationContext ctx) { }
}
