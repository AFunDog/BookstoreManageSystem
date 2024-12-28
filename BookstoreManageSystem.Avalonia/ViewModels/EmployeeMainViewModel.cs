using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
using SukiUI.Toasts;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class EmployeeMainViewModel : ViewModelBase
{
    public SQLDataProvider? DataProvider { get; set; }

    [ObservableProperty]
    private ObservableCollection<BookData> _bookDatas = [];

    [ObservableProperty]
    private ObservableCollection<StoreData> _storeDatas = [];

    [ObservableProperty]
    private ObservableCollection<GoodsData> _goodsDatas = [];

    [ObservableProperty]
    private ObservableCollection<PurchaseDealData> _purchaseDealDatas = [];

    [ObservableProperty]
    private ObservableCollection<SellDealData> _sellDealDatas = [];

    [ObservableProperty]
    private StoreData _selectedStoreData = new();

    [ObservableProperty]
    private int _addAmount = 0;

    [ObservableProperty]
    private decimal _addPrice = 0;

    [ObservableProperty]
    [property: StringLength(13)]
    private string _purchaseISBN = "";

    [ObservableProperty]
    private int _purchaseAmount = 0;

    [ObservableProperty]
    private decimal _purchasePrice = 0;

    [ObservableProperty]
    private string _registerISBN = "";

    [ObservableProperty]
    private string _registerBookName = "";

    [ObservableProperty]
    private string _registerAuthor = "";

    [ObservableProperty]
    private string _registerPublisher = "";

    [ObservableProperty]
    private string _registerIntroduction = "";

    [RelayCommand]
    private async Task AddBook()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """ 
            INSERT INTO "Books"
            VALUES (@ISBN,@BookName,@Author,@Publisher,@Introduction)
            """
        );
        cmd.Parameters.AddWithValue("@ISBN", RegisterISBN);
        cmd.Parameters.AddWithValue("@BookName", RegisterBookName);
        cmd.Parameters.AddWithValue("@Author", RegisterAuthor);
        cmd.Parameters.AddWithValue("@Publisher", RegisterAuthor);
        cmd.Parameters.AddWithValue("@Introduction", RegisterAuthor);
        try
        {
            await cmd.ExecuteNonQueryAsync();
            await LoadDatas();
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Success)
                .WithTitle("添加书籍成功")
                .WithContent(
                    $"""
                    ISBN : {RegisterISBN}
                    书名 : {RegisterBookName}
                    """
                )
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Dismiss()
                .ByClicking()
                .Queue();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Error)
                .WithTitle("添加书籍失败")
                .WithContent(e.Message)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    [RelayCommand]
    private async Task AddGoods()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """ CALL putBooksToGoods(@ISBN,@Amount,@Price,NULL); """
        );
        cmd.Parameters.AddWithValue("@ISBN", SelectedStoreData.ISBN);
        cmd.Parameters.AddWithValue("@Amount", AddAmount);
        cmd.Parameters.AddWithValue("@Price", AddPrice);
        try
        {
            using var reader = await cmd.ExecuteReaderAsync();
            var goodsId = await reader.ReadAsync();

            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Success)
                .WithTitle("上架商品成功")
                .WithContent(
                    $"""
                    GoodsId : {goodsId}
                    ISBN : {SelectedStoreData.ISBN}
                    """
                )
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Dismiss()
                .ByClicking()
                .Queue();

            await LoadDatas();
        }
        catch (Exception e)
        {
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Error)
                .WithTitle("上架商品失败")
                .WithContent(e.Message)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    [RelayCommand]
    private async Task PurchaseBooks()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(
            """ CALL purchaseBooks(@ISBN,@Amount,@Price,NULL) """
        );
        cmd.Parameters.AddWithValue("@ISBN", PurchaseISBN);
        cmd.Parameters.AddWithValue("@Amount", PurchaseAmount);
        cmd.Parameters.AddWithValue("@Price", PurchasePrice);
        try
        {
            using var reader = await cmd.ExecuteReaderAsync();
            var purchaseDealId = await reader.ReadAsync();

            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Success)
                .WithTitle("采购记录成功")
                .WithContent(
                    $"""
                    PurchaseDealId : {purchaseDealId}
                    ISBN : {PurchaseISBN}
                    """
                )
                .Dismiss()
                .After(TimeSpan.FromSeconds(3))
                .Dismiss()
                .ByClicking()
                .Queue();

            await LoadDatas();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Error)
                .WithTitle("采购记录失败")
                .WithContent(e.Message)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    public async Task LoadDatas()
    {
        await Task.WhenAll(
            [
                LoadBookDatas(),
                LoadStorekDatas(),
                LoadGoodsDatas(),
                LoadPurchaseDealDatas(),
                LoadSellDealDatas()
            ]
        );
    }

    private async Task LoadBookDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(""" SELECT * FROM "Books"; """);
        using var reader = await cmd.ExecuteReaderAsync();
        BookDatas.Clear();
        while (await reader.ReadAsync())
        {
            BookDatas.Add(BookData.CreateFromReader(reader));
        }
    }

    private async Task LoadStorekDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(""" SELECT * FROM "Stores"; """);
        using var reader = await cmd.ExecuteReaderAsync();
        StoreDatas.Clear();
        while (await reader.ReadAsync())
        {
            StoreDatas.Add(StoreData.CreateFromReader(reader));
        }
    }

    private async Task LoadGoodsDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(""" SELECT * FROM "Goods"; """);
        using var reader = await cmd.ExecuteReaderAsync();
        GoodsDatas.Clear();
        while (await reader.ReadAsync())
        {
            GoodsDatas.Add(GoodsData.CreateFromReader(reader));
        }
    }

    private async Task LoadPurchaseDealDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand("""SELECT * FROM "PurchaseDeals";""");
        using var reader = await cmd.ExecuteReaderAsync();
        PurchaseDealDatas.Clear();
        while (await reader.ReadAsync())
        {
            PurchaseDealDatas.Add(PurchaseDealData.CreateFromReader(reader));
        }
    }

    private async Task LoadSellDealDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand("""SELECT * FROM "SellDeals";""");
        using var reader = await cmd.ExecuteReaderAsync();
        SellDealDatas.Clear();
        while (await reader.ReadAsync())
        {
            SellDealDatas.Add(SellDealData.CreateFromReader(reader));
        }
    }
}
