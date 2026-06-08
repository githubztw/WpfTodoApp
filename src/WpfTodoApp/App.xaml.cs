using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WpfTodoApp.Modules.Login;
using WpfTodoApp.Modules.TodoList;
using WpfTodoApp.Modules.UserManagement;
using WpfTodoApp.Services;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;
using WpfTodoApp.ViewModels;

namespace WpfTodoApp;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        var shell = Container.Resolve<MainWindow>();
        shell.DataContext = Container.Resolve<MainWindowViewModel>();
        return shell;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Database
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WpfTodoApp", "app.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        containerRegistry.Register<AppDbContext>(() =>
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}").Options;
            return new AppDbContext(opts);
        });

        // Services (transient for fresh DbContext each time)
        containerRegistry.Register<IAuthService, AuthService>();
        containerRegistry.Register<IUserService, UserService>();
        containerRegistry.Register<ITodoService, TodoService>();
        containerRegistry.RegisterSingleton<IReminderService, ReminderService>();

        // Managers
        containerRegistry.RegisterSingleton<Modules.Login.Managers.LoginManager>();
        containerRegistry.Register<Modules.TodoList.Managers.TodoManager>();
        containerRegistry.Register<Modules.UserManagement.Managers.UserManager>();

        // Shell ViewModel
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<LoginModule>();
        moduleCatalog.AddModule<TodoListModule>();
        moduleCatalog.AddModule<UserManagementModule>();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var db = Container.Resolve<AppDbContext>();
        db.Database.EnsureCreated();

        Container.Resolve<IReminderService>().StartMonitoring();

        Container.Resolve<IRegionManager>()
            .RequestNavigate("ContentRegion", "LoginView");
    }
}
