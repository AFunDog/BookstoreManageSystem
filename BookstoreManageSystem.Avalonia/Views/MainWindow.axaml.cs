using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using BookstoreManageSystem.Avalonia.ViewModels;
using BookstoreManageSystem.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;

namespace BookstoreManageSystem.Avalonia.Views;

public partial class MainWindow : SukiUI.Controls.SukiWindow
{
    internal MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
        toastHost.Manager = App.Instance.SukiToastManager;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        ViewModel.DataProvider?.Dispose();
        ViewModel.DataProvider = null;
        toastHost.Manager.DismissAll();
        toastHost.Manager = null;
    }
}
