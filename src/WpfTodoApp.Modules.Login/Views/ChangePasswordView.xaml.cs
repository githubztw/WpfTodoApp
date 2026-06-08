using System.Windows.Controls;
using WpfTodoApp.Modules.Login.ViewModels;

namespace WpfTodoApp.Modules.Login.Views;

public partial class ChangePasswordView : UserControl
{
    public ChangePasswordView()
    {
        InitializeComponent();
        OldPasswordBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.OldPassword = OldPasswordBox.Password;
        };
        NewPasswordBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.NewPassword = NewPasswordBox.Password;
        };
        ConfirmPasswordBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        };
    }
}
