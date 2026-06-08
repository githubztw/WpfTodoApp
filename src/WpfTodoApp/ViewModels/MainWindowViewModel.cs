using System.Collections.ObjectModel;
using System.Windows;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using WpfTodoApp.Core.Events;

namespace WpfTodoApp.ViewModels;

public class MenuItem
{
    public string Text { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public bool IsLogout { get; set; }
}

public class MainWindowViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private SubscriptionToken? _loginToken;

    public ObservableCollection<MenuItem> MenuItems { get; } = new();

    private MenuItem? _selectedMenuItem;
    public MenuItem? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set
        {
            if (SetProperty(ref _selectedMenuItem, value) && value is not null)
            {
                if (value.IsLogout)
                {
                    _isLoggedIn = false;
                    MenuItems.Clear();
                    _regionManager.RequestNavigate("ContentRegion", "LoginView");
                }
                else if (!string.IsNullOrEmpty(value.ViewName))
                {
                    _regionManager.RequestNavigate("ContentRegion", value.ViewName);
                }
            }
        }
    }

    private bool _isLoggedIn;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    private string _windowTitle = "TaskFlow";
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }

    public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
    {
        _regionManager = regionManager;
        _loginToken = eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe(OnUserLoggedIn);
    }

    private void OnUserLoggedIn(UserLoggedInEvent.Payload payload)
    {
        IsLoggedIn = true;
        Application.Current.Dispatcher.Invoke(() => BuildMenu(payload.Username, payload.IsAdmin));
    }

    private void BuildMenu(string username, bool isAdmin)
    {
        MenuItems.Clear();
        MenuItems.Add(new MenuItem { Text = "我的任务", ViewName = "TodoListView" });
        if (isAdmin)
            MenuItems.Add(new MenuItem { Text = "用户管理", ViewName = "UserManagementView" });
        MenuItems.Add(new MenuItem { Text = $"  {username}  " });
        MenuItems.Add(new MenuItem { Text = "退出", IsLogout = true });
        SelectedMenuItem = MenuItems.First();
    }
}
