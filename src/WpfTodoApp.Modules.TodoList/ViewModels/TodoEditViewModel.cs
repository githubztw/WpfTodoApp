using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using WpfTodoApp.Core.Dtos;
using WpfTodoApp.Core.Models;
using WpfTodoApp.Modules.TodoList.Managers;

namespace WpfTodoApp.Modules.TodoList.ViewModels;

public class TodoEditViewModel : BindableBase, INavigationAware, IDisposable
{
    private readonly TodoManager _todoManager;
    private readonly IRegionManager _regionManager;
    private int _userId;
    private int? _editTodoId;

    private string _title = string.Empty;
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    private string? _description;
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    private DateTime? _dueDate;
    public DateTime? DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }

    private TodoPriority _priority = TodoPriority.Medium;
    public TodoPriority Priority { get => _priority; set => SetProperty(ref _priority, value); }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private string _dialogTitle = "添加任务";
    public string DialogTitle { get => _dialogTitle; set => SetProperty(ref _dialogTitle, value); }

    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public TodoEditViewModel(TodoManager todoManager, IRegionManager regionManager)
    {
        _todoManager = todoManager;
        _regionManager = regionManager;
        SaveCommand = new DelegateCommand(ExecuteSave);
        CancelCommand = new DelegateCommand(() => _regionManager.RequestNavigate("ContentRegion", "TodoListView"));
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
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    public void OnNavigatedTo(NavigationContext ctx)
    {
        _userId = ctx.Parameters.GetValue<int>("userId");
        if (ctx.Parameters.TryGetValue<TodoItemDto>("editTodo", out var todo))
        {
            _editTodoId = todo.Id;
            Title = todo.Title; Description = todo.Description;
            DueDate = todo.DueDate; Priority = todo.Priority;
            DialogTitle = "编辑任务";
        }
    }

    public bool IsNavigationTarget(NavigationContext ctx) => true;
    public void OnNavigatedFrom(NavigationContext ctx) { }
    public void Dispose() { }
}
