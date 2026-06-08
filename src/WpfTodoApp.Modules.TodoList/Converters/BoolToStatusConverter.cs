using System.Globalization;
using System.Windows.Data;

namespace WpfTodoApp.Modules.TodoList.Converters;

public class BoolToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isCompleted)
            return isCompleted ? "已完成" : "未完成";
        return "未知";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
