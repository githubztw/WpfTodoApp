using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Serilog;
using WpfTodoApp.Modules.Login;
using WpfTodoApp.Modules.TodoList;
using WpfTodoApp.Modules.UserManagement;
using WpfTodoApp.Services;
using WpfTodoApp.Services.Data;
using WpfTodoApp.Services.Interfaces;
using WpfTodoApp.ViewModels;

namespace WpfTodoApp;

/// <summary>
/// TaskFlow 应用程序入口。使用 Prism.DryIoc 作为 DI 容器，
/// 负责 Serilog 初始化、服务注册、模块加载和启动导航。
/// </summary>
public partial class App : PrismApplication
{
    private string _dbPath = string.Empty;

    protected override Window CreateShell()
    {
        var shell = Container.Resolve<MainWindow>();
        shell.DataContext = Container.Resolve<MainWindowViewModel>();
        return shell;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // === 加载配置文件（appsettings.json + appsettings.{Env}.json） ===
        var env = Environment.GetEnvironmentVariable("TASKFLOW_ENV") ?? "Production";
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
            .Build();

        // === Serilog 初始化 ===
        Serilog.Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        var logger = new Modules.Logging.SerilogLogger();
        containerRegistry.RegisterInstance<Core.Interfaces.ILogger>(logger);
        logger.Info($"=== TaskFlow 启动 (环境: {env}) ===");

        // === 绑定各模块配置类并注册到 DI ===
        var coreConfig = config.GetSection("CoreConfig").Get<Core.Config.CoreConfig>()!;
        var loginConfig = config.GetSection("LoginConfig").Get<Modules.Login.Config.LoginConfig>()!;
        var todoConfig = config.GetSection("TodoConfig").Get<Modules.TodoList.Config.TodoConfig>()!;
        var userMgmtConfig = config.GetSection("UserManagementConfig").Get<Modules.UserManagement.Config.UserManagementConfig>()!;
        var reminderConfig = config.GetSection("ReminderConfig").Get<Services.Config.ReminderConfig>()!;

        containerRegistry.RegisterInstance(coreConfig);
        containerRegistry.RegisterInstance(loginConfig);
        containerRegistry.RegisterInstance(todoConfig);
        containerRegistry.RegisterInstance(userMgmtConfig);
        containerRegistry.RegisterInstance(reminderConfig);

        // Database
        _dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WpfTodoApp", "app.db");
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);

        containerRegistry.Register<AppDbContext>(CreateDbContext);

        // Services
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

    private AppDbContext CreateDbContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={_dbPath}").Options;
        return new AppDbContext(opts);
    }
}
