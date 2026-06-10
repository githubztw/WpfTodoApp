using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using WpfTodoApp.Core;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Core.Interfaces;
using WpfTodoApp.Modules.TodoList.Managers;

namespace WpfTodoApp.Modules.TodoList.ViewModels;

/// <summary>
/// 任务列表界面 ViewModel。管理任务的加载、筛选、CRUD 操作，
/// 通过 IEventAggregator 监听登录事件以加载对应用户的任务。
/// </summary>
public class TodoListViewModel : ViewModelBase, IDisposable
{
    private readonly TodoManager _todoManager;
    private readonly IRegionManager _regionManager;
    private int _userId;

    public ObservableCollection<TodoItemDto> Todos { get; } = new();

    private TodoFilter _currentFilter = TodoFilter.All;
    public TodoFilter CurrentFilter
    {
        get => _currentFilter;
        set
        {
            SetProperty(ref _currentFilter, value);
            LoadTodos();
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddTodoCommand { get; }
    public DelegateCommand<TodoItemDto> EditTodoCommand { get; }
    public DelegateCommand<TodoItemDto> ToggleCompleteCommand { get; }
    public DelegateCommand<TodoItemDto> DeleteTodoCommand { get; }
    public DelegateCommand FilterAllCommand { get; }
    public DelegateCommand FilterIncompleteCommand { get; }
    public DelegateCommand FilterCompletedCommand { get; }

    private SubscriptionToken? _loginToken;

    public TodoListViewModel(
        TodoManager todoManager,
        IRegionManager regionManager,
        IEventAggregator eventAggregator,
        ILogger logger)
        : base(logger)
    {
        _todoManager = todoManager;
        _regionManager = regionManager;

        AddTodoCommand = new DelegateCommand(ExecuteAddTodo);
        EditTodoCommand = new DelegateCommand<TodoItemDto>(ExecuteEditTodo);
        ToggleCompleteCommand = new DelegateCommand<TodoItemDto>(ExecuteToggleComplete);
        DeleteTodoCommand = new DelegateCommand<TodoItemDto>(ExecuteDeleteTodo);
        FilterAllCommand = new DelegateCommand(SetFilterAll);
        FilterIncompleteCommand = new DelegateCommand(SetFilterIncomplete);
        FilterCompletedCommand = new DelegateCommand(SetFilterCompleted);

        _loginToken = eventAggregator
            .GetEvent<UserLoggedInEvent>()
            .Subscribe(OnUserLoggedIn);
    }

    private void SetFilterAll() => CurrentFilter = TodoFilter.All;
    private void SetFilterIncomplete() => CurrentFilter = TodoFilter.Incomplete;
    private void SetFilterCompleted() => CurrentFilter = TodoFilter.Completed;

    private void OnUserLoggedIn(UserLoggedInEvent.Payload payload)
    {
        _userId = payload.UserId;
        LoadTodos();
    }

    private void LoadTodos()
    {
        if (_userId == 0)
            return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var items = _todoManager.GetTodos(_userId, CurrentFilter);
            Todos.Clear();
            foreach (var item in items)
                Todos.Add(item);
        }
        catch (Exception ex)
        {
            Logger.Error("加载任务失败", ex);
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ExecuteAddTodo()
    {
        var parameters = new NavigationParameters { { "userId", _userId } };
        _regionManager.RequestNavigate("ContentRegion", "TodoEditView", parameters);
    }

    private void ExecuteEditTodo(TodoItemDto todo)
    {
        var parameters = new NavigationParameters
        {
            { "userId", _userId },
            { "editTodo", todo }
        };
        _regionManager.RequestNavigate("ContentRegion", "TodoEditView", parameters);
    }

    private void ExecuteToggleComplete(TodoItemDto todo)
    {
        try
        {
            _todoManager.ToggleComplete(todo.Id);
            LoadTodos();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void ExecuteDeleteTodo(TodoItemDto todo)
    {
        try
        {
            _todoManager.DeleteTodo(todo.Id);
            Todos.Remove(todo);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public void Dispose()
    {
        _loginToken?.Dispose();
    }
}
