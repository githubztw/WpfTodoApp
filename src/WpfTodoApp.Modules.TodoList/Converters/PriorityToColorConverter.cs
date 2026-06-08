using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Modules.TodoList.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TodoPriority priority)
        {
            return priority switch
            {
                TodoPriority.High => new SolidColorBrush(Color.FromRgb(243, 139, 168)),
                TodoPriority.Medium => new SolidColorBrush(Color.FromRgb(250, 179, 135)),
                TodoPriority.Low => new SolidColorBrush(Color.FromRgb(166, 227, 161)),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
