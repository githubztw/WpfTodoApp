using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Events;
using WpfTodoApp.Modules.TodoList.Managers;

namespace WpfTodoApp.Modules.TodoList.ViewModels;

public class TodoListViewModel : BindableBase, IDisposable
{
    private readonly TodoManager _todoManager;
    private readonly IRegionManager _regionManager;
    private int _userId;

    public ObservableCollection<TodoItemDto> Todos { get; } = new();

    private TodoFilter _currentFilter = TodoFilter.All;
    public TodoFilter CurrentFilter { get => _currentFilter; set { SetProperty(ref _currentFilter, value); LoadTodos(); } }

    private bool _isLoading;
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public DelegateCommand AddTodoCommand { get; }
    public DelegateCommand<TodoItemDto> EditTodoCommand { get; }
    public DelegateCommand<TodoItemDto> ToggleCompleteCommand { get; }
    public DelegateCommand<TodoItemDto> DeleteTodoCommand { get; }
    public DelegateCommand FilterAllCommand { get; }
    public DelegateCommand FilterIncompleteCommand { get; }
    public DelegateCommand FilterCompletedCommand { get; }

    private SubscriptionToken? _loginToken;

    public TodoListViewModel(TodoManager todoManager, IRegionManager regionManager,
        IEventAggregator eventAggregator)
    {
        _todoManager = todoManager;
        _regionManager = regionManager;
        AddTodoCommand = new DelegateCommand(ExecuteAddTodo);
        EditTodoCommand = new DelegateCommand<TodoItemDto>(ExecuteEditTodo);
        ToggleCompleteCommand = new DelegateCommand<TodoItemDto>(ExecuteToggleComplete);
        DeleteTodoCommand = new DelegateCommand<TodoItemDto>(ExecuteDeleteTodo);
        FilterAllCommand = new DelegateCommand(() => CurrentFilter = TodoFilter.All);
        FilterIncompleteCommand = new DelegateCommand(() => CurrentFilter = TodoFilter.Incomplete);
        FilterCompletedCommand = new DelegateCommand(() => CurrentFilter = TodoFilter.Completed);
        _loginToken = eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe(p => { _userId = p.UserId; LoadTodos(); });
    }

    private void LoadTodos()
    {
        if (_userId == 0) return;
        IsLoading = true; ErrorMessage = null;
        try { Todos.Clear(); foreach (var t in _todoManager.GetTodos(_userId, CurrentFilter)) Todos.Add(t); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }

    private void ExecuteAddTodo()
    {
        var p = new NavigationParameters { { "userId", _userId } };
        _regionManager.RequestNavigate("ContentRegion", "TodoEditView", p);
    }

    private void ExecuteEditTodo(TodoItemDto todo)
    {
        var p = new NavigationParameters { { "userId", _userId }, { "editTodo", todo } };
        _regionManager.RequestNavigate("ContentRegion", "TodoEditView", p);
    }

    private void ExecuteToggleComplete(TodoItemDto t)
    {
        try { _todoManager.ToggleComplete(t.Id); LoadTodos(); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    private void ExecuteDeleteTodo(TodoItemDto t)
    {
        try { _todoManager.DeleteTodo(t.Id); Todos.Remove(t); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    public void Dispose() { _loginToken?.Dispose(); }
}
