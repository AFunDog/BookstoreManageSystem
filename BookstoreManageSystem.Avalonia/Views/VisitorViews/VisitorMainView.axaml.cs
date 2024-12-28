using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BookstoreManageSystem.Avalonia.ViewModels;

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
        if (
            Parent is not null
            && Parent.DataContext is MainWindowViewModel pvm
            && pvm.DataProvider is not null
        )
        {
            DataContext = new VisitorMainViewModel { DataProvider = pvm.DataProvider };
            _ = ViewModel.LoadDatas();
        }
    }
}
