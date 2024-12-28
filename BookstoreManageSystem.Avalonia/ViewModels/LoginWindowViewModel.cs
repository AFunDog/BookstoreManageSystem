using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Templates;
using BookstoreManageSystem.Avalonia.Views;
using BookstoreManageSystem.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class LoginWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _registerUserName = "";

    [ObservableProperty]
    private string _registerAddress = "";

    [ObservableProperty]
    private string _registerPhone = "";

    [ObservableProperty]
    private string _registerEmail = "";

    [ObservableProperty]
    private string _registerPassword = "";

    [ObservableProperty]
    private int _registerUID = -1;

    internal event Action? LoginSucceed;
    internal event Action? LoginFailed;
    internal event Action? Logout;

    [RelayCommand]
    private async Task LoginAsVisitor()
    {
        var provider = await SQLDataProvider.LoginAsVisitor();
        if (provider is not null)
        {
            ToMainWindow(provider);
        }
    }

    [RelayCommand]
    private async Task LoginAsCustomer()
    {
        var provider = await SQLDataProvider.LoginAsCustomer(UserName, Password);
        if (provider is not null)
        {
            ToMainWindow(provider);
        }
        else
        {
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("顾客账号登录失败")
                .WithContent("请检查信息是否有误")
                .Dismiss()
                .ByClicking()
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

    [RelayCommand]
    private async Task RegisterCustomerAccount()
    {
        using var provider = await SQLDataProvider.LoginAsVisitor();
        if (provider is not null)
        {
            using var cmd = provider.DataSource.CreateCommand(
                """ CALL registerUser(@Name,@Address,@Phone,@Email,@Password,NULL) """
            );
            cmd.Parameters.AddWithValue("@Name", RegisterUserName);
            cmd.Parameters.AddWithValue("@Address", RegisterAddress);
            cmd.Parameters.AddWithValue("@Phone", RegisterPhone);
            cmd.Parameters.AddWithValue("@Email", RegisterEmail);
            cmd.Parameters.AddWithValue("@Password", RegisterPassword);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            RegisterUID = reader.GetInt32(0);
        }
    }

    [RelayCommand]
    private async Task LoginAsEmployee()
    {
        var provider = await SQLDataProvider.LoginAsEmployee(UserName, Password);
        if (provider is not null)
        {
            ToMainWindow(provider);
        }
        else
        {
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("员工账号登录失败")
                .WithContent("请检查信息是否有误")
                .Dismiss()
                .ByClicking()
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

    [RelayCommand]
    private async Task LoginAsOwner()
    {
        var provider = await SQLDataProvider.LoginAsOwner(Password);
        if (provider is not null)
        {
            ToMainWindow(provider);
        }
        else
        {
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("店主账号登录失败")
                .WithContent("请检查信息是否有误")
                .Dismiss()
                .ByClicking()
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

    private void ToMainWindow(SQLDataProvider dataProvider)
    {
        App.Instance.ShowWindow = new MainWindow();
        App.Instance.ShowWindow.ViewModel.DataProvider = dataProvider;
        App.Instance.ShowWindow.Closing += (s, e) =>
        {
            Logout?.Invoke();
        };
        LoginSucceed?.Invoke();
        App.Instance.ShowWindow.Show();
    }
}
