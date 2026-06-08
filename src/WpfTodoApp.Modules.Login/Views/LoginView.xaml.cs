using System.Windows.Controls;
using WpfTodoApp.Modules.Login.ViewModels;

namespace WpfTodoApp.Modules.Login.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        PasswordBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        };
    }
}
