using Prism.Commands;
using Prism.Regions;
using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Modules.Logging;
using WpfTodoApp.Core.Models;
using WpfTodoApp.Modules.TodoList.Managers;

namespace WpfTodoApp.Modules.TodoList.ViewModels;

/// <summary>
/// 任务编辑界面 ViewModel。支持创建和编辑两种模式，
/// 通过 INavigationAware 接收 TodoItemDto 参数判断模式。
/// </summary>
public class TodoEditViewModel : ViewModelBase, INavigationAware, IDisposable
{
    private readonly TodoManager _todoManager;
    private readonly IRegionManager _regionManager;
    private int _userId;
    private int? _editTodoId;

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private DateTime? _dueDate;
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    private TodoPriority _priority = TodoPriority.Medium;
    public TodoPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string _dialogTitle = "添加任务";
    public string DialogTitle
    {
        get => _dialogTitle;
        set => SetProperty(ref _dialogTitle, value);
    }

    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public TodoEditViewModel(
        TodoManager todoManager,
        IRegionManager regionManager,
        ILogger logger)
        : base(logger)
    {
        _todoManager = todoManager;
        _regionManager = regionManager;

        SaveCommand = new DelegateCommand(ExecuteSave);
        CancelCommand = new DelegateCommand(ExecuteCancel);
    }

    private void ExecuteSave()
    {
        ErrorMessage = null;

        try
        {
            if (_editTodoId.HasValue)
                _todoManager.UpdateTodo(_editTodoId.Value, Title, Description, DueDate, Priority);
            else
                _todoManager.CreateTodo(_userId, Title, Description, DueDate, Priority);

            _regionManager.RequestNavigate("ContentRegion", "TodoListView");
        }
        catch (Exception ex)
        {
            Logger.Error("保存任务失败", ex);
            ErrorMessage = ex.Message;
        }
    }

    private void ExecuteCancel()
    {
        _regionManager.RequestNavigate("ContentRegion", "TodoListView");
    }

    public void OnNavigatedTo(NavigationContext ctx)
    {
        _userId = ctx.Parameters.GetValue<int>("userId");

        if (ctx.Parameters.TryGetValue<TodoItemDto>("editTodo", out var todo))
        {
            _editTodoId = todo.Id;
            Title = todo.Title;
            Description = todo.Description;
            DueDate = todo.DueDate;
            Priority = todo.Priority;
            DialogTitle = "编辑任务";
        }
    }

    public bool IsNavigationTarget(NavigationContext ctx)
    {
        return true;
    }

    public void OnNavigatedFrom(NavigationContext ctx)
    {
    }

    public void Dispose()
    {
    }
}
