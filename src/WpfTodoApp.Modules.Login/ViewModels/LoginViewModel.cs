using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Modules.Login.Managers;

namespace WpfTodoApp.Modules.Login.ViewModels;

public class LoginViewModel : BindableBase
{
    private readonly LoginManager _loginManager;
    private readonly IRegionManager _regionManager;
    private readonly IEventAggregator _eventAggregator;

    private string _username = string.Empty;
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    private string _password = string.Empty;
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _isLoading;
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand LoginCommand { get; }

    public LoginViewModel(LoginManager loginManager, IRegionManager regionManager,
        IEventAggregator eventAggregator)
    {
        _loginManager = loginManager;
        _regionManager = regionManager;
        _eventAggregator = eventAggregator;
        LoginCommand = new DelegateCommand(ExecuteLogin, CanExecuteLogin)
            .ObservesProperty(() => Username).ObservesProperty(() => Password);
    }

    private bool CanExecuteLogin() =>
        !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsLoading;

    private void ExecuteLogin()
    {
        IsLoading = true; ErrorMessage = null;
        var result = _loginManager.Authenticate(Username, Password);
        if (!result.Success) { ErrorMessage = result.ErrorMessage; IsLoading = false; return; }

        if (result.IsFirstLogin)
        {
            var p = new NavigationParameters { { "username", result.Username }, { "isAdmin", result.IsAdmin } };
            _regionManager.RequestNavigate("ContentRegion", "ChangePasswordView", p);
        }
        else
        {
            _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(
                new UserLoggedInEvent.Payload(result.UserId, result.Username, result.IsAdmin));
        }
        IsLoading = false;
    }
}
