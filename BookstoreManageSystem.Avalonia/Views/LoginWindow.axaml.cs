using System;
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
        ViewModel.LoginSucceed += OnLoginSucceed;
        ViewModel.Logout += OnLogout;
    }

    private void OnLogout()
    {
        Show();
        toastHost.Manager = App.Instance.SukiToastManager;
    }

    private void OnLoginSucceed()
    {
        Hide();
        toastHost.Manager.DismissAll();
        toastHost.Manager = null;
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
