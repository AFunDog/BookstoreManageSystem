using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using BookstoreManageSystem.Core;
using BookstoreManageSystem.Core.Structs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class CustomerMainViewModel : ViewModelBase
{
    public SQLDataProvider? DataProvider { get; set; }

    [ObservableProperty]
    private CustomerData _myCustomerData = new();

    [ObservableProperty]
    private ObservableCollection<SellBookData> _sellBookDatas = [];

    [ObservableProperty]
    private ObservableCollection<SellDealData> _mySellDealDatas = [];

    [ObservableProperty]
    private SellBookData _selectedSellBookData = new();

    [ObservableProperty]
    private string _filterISBN = "";

    [ObservableProperty]
    private string _filterBookName = "";

    [ObservableProperty]
    private string _filterAuthor = "";

    [ObservableProperty]
    private string _filterPublisher = "";

    [ObservableProperty]
    private string _editCustomerName = "";

    [ObservableProperty]
    private string _editCustomerAddress = "";

    [ObservableProperty]
    private string _editCustomerPhone = "";

    [ObservableProperty]
    private string _editCustomerEmail = "";

    [RelayCommand]
    private async Task UpdateMyCustomerData()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """
            UPDATE "Customers"
            SET "Name" = @CustomerName,
                "Address" = @CustomerAddress,
                "Phone" = @CustomerPhone,
                "Email" = @CustomerEmail;
            """
        );
        try
        {
            cmd.Parameters.AddWithValue("@CustomerName", EditCustomerName);
            cmd.Parameters.AddWithValue("@CustomerAddress", EditCustomerAddress);
            cmd.Parameters.AddWithValue("@CustomerPhone", EditCustomerPhone);
            cmd.Parameters.AddWithValue("@CustomerEmail", EditCustomerEmail);
            await cmd.ExecuteNonQueryAsync();
            await LoadMyCustomerData();
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("更新个人信息成功")
                .OfType(NotificationType.Success)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
        catch (Exception e)
        {
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("更新个人信息失败")
                .WithContent(e.Message)
                .OfType(NotificationType.Error)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    [RelayCommand]
    private async Task BuyBook(int amount)
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """ CALL buyBooks(@GoodsID,@Amount,NULL) """
        );
        cmd.Parameters.AddWithValue("@GoodsID", SelectedSellBookData.GoodsID);
        cmd.Parameters.AddWithValue("@Amount", amount);
        using var reader = await cmd.ExecuteReaderAsync();
        try
        {
            await reader.ReadAsync();
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("购买成功")
                .WithContent(
                    $"""
                    ISBN : {SelectedSellBookData.ISBN} 
                    数量 : {amount}
                    总价 : {SelectedSellBookData.Price * amount}
                    """
                )
                .OfType(NotificationType.Success)
                .Dismiss()
                .ByClicking()
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Queue();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("购买失败")
                .WithContent(e.Message)
                .OfType(NotificationType.Error)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
        await LoadDatas();
        //var dealID = reader.GetInt32(0);
    }

    [RelayCommand]
    private async Task ClearFilter()
    {
        FilterISBN = "";
        FilterBookName = "";
        FilterAuthor = "";
        FilterPublisher = "";
        await LoadSellBookDatas();
    }

    public async Task LoadDatas()
    {
        await Task.WhenAll([LoadSellBookDatas(), LoadMySellDealDatas(), LoadMyCustomerData()]);
    }

    [RelayCommand]
    private async Task LoadSellBookDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(
            """ 
            SELECT * FROM "SellBooksView" 
            WHERE 
            ("ISBN" ~ @FilterISBN OR @FilterISBN = '' OR @FilterISBN IS NULL)
            AND 
            ("BookName" ~ @FilterBookName OR @FilterBookName = '' OR @FilterBookName IS NULL)
            AND 
            ("Author" ~ @FilterAuthor OR @FilterAuthor = '' OR @FilterAuthor IS NULL)
            AND 
            ("Publisher" ~ @FilterPublisher OR @FilterPublisher = '' OR @FilterPublisher IS NULL)
            """
        );
        cmd.Parameters.AddWithValue("@FilterISBN", FilterISBN);
        cmd.Parameters.AddWithValue("@FilterBookName", FilterBookName);
        cmd.Parameters.AddWithValue("@FilterAuthor", FilterAuthor);
        cmd.Parameters.AddWithValue("@FilterPublisher", FilterPublisher);
        using var reader = await cmd.ExecuteReaderAsync();
        SellBookDatas.Clear();
        while (await reader.ReadAsync())
        {
            SellBookDatas.Add(SellBookData.CreateFromReader(reader));
        }
    }

    private async Task LoadMySellDealDatas()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(""" SELECT * FROM "SellDeals"; """);
        using var reader = await cmd.ExecuteReaderAsync();
        MySellDealDatas.Clear();
        while (await reader.ReadAsync())
        {
            MySellDealDatas.Add(SellDealData.CreateFromReader(reader));
        }
    }

    private async Task LoadMyCustomerData()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(""" SELECT * FROM "Customers"; """);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            MyCustomerData = CustomerData.CreateFromReader(reader);
        }
    }

    //[RelayCommand]
    //private void EditMyCustomerData()
    //{
    //    if (DataProvider is null)
    //        return;
    //}

    partial void OnMyCustomerDataChanged(CustomerData value)
    {
        EditCustomerName = value.Name;
        EditCustomerAddress = value.Address;
        EditCustomerPhone = value.Phone;
        EditCustomerEmail = value.Email;
    }
}
