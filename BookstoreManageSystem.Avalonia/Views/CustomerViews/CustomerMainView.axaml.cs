using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;
using BookstoreManageSystem.Core.Structs;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia;

public partial class CustomerMainView : UserControl
{
    internal CustomerMainViewModel ViewModel => (CustomerMainViewModel)DataContext!;

    public CustomerMainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is MainWindowViewModel pvm && pvm.DataProvider is not null)
        {
            DataContext = new CustomerMainViewModel { DataProvider = pvm.DataProvider };
            _ = ViewModel.LoadDatas();
        }
    }

    private void SellBookCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.DataContext is SellBookData data)
        {
            ViewModel.SelectedSellBookData = data;
            //var toast = new SukiToastManager();
            //toast
            //    .CreateSimpleInfoToast()
            //    .WithTitle("Selected")
            //    .WithContent($"Selected {data.BookName}")
            //    .Queue();
        }
    }

    private async void SellBookListContextMenuItem_BuyBookClick(object? sender, RoutedEventArgs e)
    {
        await ViewModel.BuyBookCommand.ExecuteAsync(AmountSlider.Value);
    }
}
