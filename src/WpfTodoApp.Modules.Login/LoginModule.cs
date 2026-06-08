using Prism.Ioc;
using Prism.Modularity;
using WpfTodoApp.Modules.Login.Views;

namespace WpfTodoApp.Modules.Login;

public class LoginModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<LoginView>();
        containerRegistry.RegisterForNavigation<ChangePasswordView>();
    }
}
