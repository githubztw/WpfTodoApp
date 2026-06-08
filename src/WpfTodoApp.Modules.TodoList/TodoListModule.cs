using Prism.Ioc;
using Prism.Modularity;
using WpfTodoApp.Modules.TodoList.Views;

namespace WpfTodoApp.Modules.TodoList;

public class TodoListModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<TodoListView>();
        containerRegistry.RegisterForNavigation<TodoEditView>();
    }
}
