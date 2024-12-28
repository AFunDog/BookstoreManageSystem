using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;
using BookstoreManageSystem.Core;
using CommunityToolkit.Mvvm.ComponentModel;

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
        }
    }
}
