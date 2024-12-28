using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;

namespace BookstoreManageSystem.Avalonia;

public partial class LoginWindow : SukiUI.Controls.SukiWindow
{
    internal LoginWindowViewModel ViewModel => (LoginWindowViewModel)DataContext!;

    public LoginWindow()
    {
        DataContext = new LoginWindowViewModel();
        InitializeComponent();
        toastHost.Manager = App.Instance.SukiToastManager;
        loginTablControl.SelectionChanged += LoginTabControl_OnSelectionChanged;
        ViewModel.LoginSucceed += Hide;
        ViewModel.Logout += Show;
    }

    private void LoginTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ViewModel.UserName = "";
        ViewModel.Password = "";
    }

    private void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        loginTablControl.SelectedIndex = 4;
    }
}
