using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BookstoreManageSystem.Avalonia.Views;
using BookstoreManageSystem.Core.Structs;
using SukiUI;
using SukiUI.Models;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia
{
    public partial class App : Application
    {
        public static App Instance => (App)Current!;

        public MainWindow? ShowWindow { get; set; }

        public ISukiToastManager SukiToastManager { get; set; } = new SukiToastManager();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var orangeTheme = new SukiColorTheme("Orange", Colors.Orange, Colors.LightYellow);
                SukiTheme.GetInstance().AddColorTheme(orangeTheme);
                SukiTheme.GetInstance().ChangeColorTheme(orangeTheme);

                BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = new LoginWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async void ConfirmEditButton_Click(object? sender, RoutedEventArgs e)
        {
            if (ShowWindow is null || ShowWindow.ViewModel.DataProvider is null)
                return;

            if (sender is Control control && control.DataContext is BookData data)
            {
                using var cmd = ShowWindow.ViewModel.DataProvider.DataSource.CreateCommand(
                    """ 
                    UPDATE "Books"
                    SET "BookName" = @BookName,"Author" = @Author,"Publisher" = @Publisher,"Introduction" = @Introduction
                    WHERE "ISBN" = @ISBN;
                    """
                );
                cmd.Parameters.AddWithValue("@ISBN", data.ISBN);
                cmd.Parameters.AddWithValue("@BookName", data.BookName);
                cmd.Parameters.AddWithValue("@Author", data.Author);
                cmd.Parameters.AddWithValue("@Publisher", data.Publisher);
                cmd.Parameters.AddWithValue("@Introduction", data.Introduction);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    await ShowWindow.ViewModel.RefreshDatas();
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private async void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            if (ShowWindow is null || ShowWindow.ViewModel.DataProvider is null)
                return;

            if (sender is Control control && control.DataContext is BookData data)
            {
                using var cmd = ShowWindow.ViewModel.DataProvider.DataSource.CreateCommand(
                    """
                    DELETE FROM "Books"
                    WHERE "ISBN" = @ISBN;
                    """
                );
                cmd.Parameters.AddWithValue("@ISBN", data.ISBN);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    await ShowWindow.ViewModel.RefreshDatas();
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
