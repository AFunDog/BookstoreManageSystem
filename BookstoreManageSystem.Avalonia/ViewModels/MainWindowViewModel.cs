using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using BookstoreManageSystem.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowContent), nameof(WindowTitle))]
    private SQLDataProvider? _dataProvider;

    public event Func<Task>? Refreshed;

    public async Task RefreshDatas()
    {
        if (Refreshed is not null)
        {
            await Refreshed();
        }
    }

    public UserControl? WindowContent =>
        DataProvider is null
            ? null
            : DataProvider.LoginType switch
            {
                LoginType.Visitor => new VisitorMainView(),
                LoginType.Customer => new CustomerMainView(),
                LoginType.Employee => new EmployeeMainView(),
                LoginType.Owner => new OwnerMainView(),
                _ => null,
            };

    public string? WindowTitle =>
        DataProvider is null
            ? null
            : $"图书管理系统 - { DataProvider.LoginType switch
            {
                LoginType.Visitor => "游客",
                LoginType.Customer => "顾客",
                LoginType.Employee => "员工",
                LoginType.Owner => "店长",
                _ => null,
            }}";
}
