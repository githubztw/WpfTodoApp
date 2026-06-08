using Prism.Ioc;
using Prism.Modularity;
using WpfTodoApp.Modules.UserManagement.Views;

namespace WpfTodoApp.Modules.UserManagement;

public class UserManagementModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<UserManagementView>();
    }
}
