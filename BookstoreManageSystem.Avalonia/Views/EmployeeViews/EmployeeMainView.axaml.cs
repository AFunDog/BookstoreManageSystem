using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;
using BookstoreManageSystem.Core.Structs;

namespace BookstoreManageSystem.Avalonia;

public partial class EmployeeMainView : UserControl
{
    internal EmployeeMainViewModel ViewModel => (EmployeeMainViewModel)DataContext!;

    public EmployeeMainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is MainWindowViewModel pvm && pvm.DataProvider is not null)
        {
            DataContext = new EmployeeMainViewModel { DataProvider = pvm.DataProvider };
            pvm.Refreshed += ViewModel.LoadDatas;
            _ = ViewModel.LoadDatas();
        }
    }

    private async void AddGoodsMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        await ViewModel.AddGoodsCommand.ExecuteAsync(null);
    }

    private void StoreDataCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control c && c.DataContext is StoreData data)
        {
            ViewModel.SelectedStoreData = data;
        }
    }

    private void PurchaseBooksButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextFlyout is not null)
        {
            button.ContextFlyout.ShowAt(button);
        }
    }

    private void RegisterBookButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextFlyout is not null)
        {
            button.ContextFlyout.ShowAt(button);
        }
    }
}
