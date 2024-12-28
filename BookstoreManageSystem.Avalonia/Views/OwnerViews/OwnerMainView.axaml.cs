using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;
using BookstoreManageSystem.Core;
using BookstoreManageSystem.Core.Structs;
using CommunityToolkit.Mvvm.ComponentModel;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia;

public partial class OwnerMainView : UserControl
{
    internal OwnerMainViewModel ViewModel => (OwnerMainViewModel)DataContext!;

    public OwnerMainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // 加载数据
        if (DataContext is MainWindowViewModel pvm && pvm.DataProvider is not null)
        {
            DataContext = new OwnerMainViewModel { DataProvider = pvm.DataProvider };
            pvm.Refreshed += ViewModel.LoadDatas;
            _ = ViewModel.LoadDatas();
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("店主登录成功")
                .WithContent("欢迎使用书店管理系统！")
                .OfType(NotificationType.Success)
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    private async void EditSalaryMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { Tag: decimal salary })
            await ViewModel.EditEmployeeSalaryCommand.ExecuteAsync(salary);
    }

    private void EmployeeGlassCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control { DataContext: EmployeeData data })
            ViewModel.SelectedEmployee = data;
    }

    private void RegisterEmployeeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control && control.ContextFlyout is not null)
        {
            control.ContextFlyout.ShowAt(control);
        }
    }
}
