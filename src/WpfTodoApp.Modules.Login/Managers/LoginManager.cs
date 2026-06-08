using Prism.Events;
using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Services.Interfaces;

namespace WpfTodoApp.Modules.Login.Managers;

public class LoginManager : ManagerBase
{
    private readonly IAuthService _authService;
    private readonly IEventAggregator _eventAggregator;

    public LoginManager(IAuthService authService, IEventAggregator eventAggregator)
    {
        _authService = authService;
        _eventAggregator = eventAggregator;
    }

    public AuthResult Authenticate(string username, string password)
    {
        try
        {
            var result = _authService.Authenticate(username, password);
            if (result.Success && !result.IsFirstLogin)
            {
                _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(
                    new UserLoggedInEvent.Payload(result.UserId, result.Username, result.IsAdmin));
            }
            return result;
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, ErrorMessage = HandleError(ex) };
        }
    }

    public ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword)
    {
        try
        {
            return _authService.ChangePassword(username, oldPassword, newPassword);
        }
        catch (Exception ex)
        {
            return new ChangePasswordResult { Success = false, ErrorMessage = HandleError(ex) };
        }
    }

    public void PublishLoginEvent(int userId, string username, bool isAdmin)
    {
        _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(
            new UserLoggedInEvent.Payload(userId, username, isAdmin));
    }
}
