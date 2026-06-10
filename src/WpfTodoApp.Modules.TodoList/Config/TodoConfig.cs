namespace WpfTodoApp.Modules.TodoList.Config;

/// <summary>
/// TodoList 模块配置。由启动程序从 appsettings.json 的 TodoConfig 节加载。
/// </summary>
public class TodoConfig
{
    /// <summary>任务标题最大长度。</summary>
    public int TitleMaxLength { get; set; } = 200;

    /// <summary>任务描述最大长度。</summary>
    public int DescriptionMaxLength { get; set; } = 1000;

    /// <summary>单用户最大任务数（0 表示无限制）。</summary>
    public int MaxTodosPerUser { get; set; } = 0;
}
