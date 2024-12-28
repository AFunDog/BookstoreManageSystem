using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia;

public partial class VisitorMainView : UserControl
{
    internal VisitorMainViewModel ViewModel => (VisitorMainViewModel)DataContext!;

    public VisitorMainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is MainWindowViewModel pvm && pvm.DataProvider is not null)
        {
            DataContext = new VisitorMainViewModel { DataProvider = pvm.DataProvider };
            _ = ViewModel.LoadDatas();
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("以游客身份访问")
                .WithContent("欢迎使用书店管理系统！")
                .OfType(NotificationType.Success)
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }
}
