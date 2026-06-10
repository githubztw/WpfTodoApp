using Prism.Mvvm;
using WpfTodoApp.Core.Interfaces;

namespace WpfTodoApp.Core;

/// <summary>
/// 所有 ViewModel 的基类。继承 Prism BindableBase，
/// 通过构造函数注入 ILogger。
/// </summary>
public abstract class ViewModelBase : BindableBase
{
    /// <summary>日志记录器，子类可直接使用。</summary>
    protected ILogger Logger { get; }

    /// <summary>注入日志实例。</summary>
    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
    }
}
