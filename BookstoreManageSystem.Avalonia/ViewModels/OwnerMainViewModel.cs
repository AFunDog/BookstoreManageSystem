using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using BookstoreManageSystem.Core;
using BookstoreManageSystem.Core.Structs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class OwnerMainViewModel : ViewModelBase
{
    public SQLDataProvider? DataProvider { get; set; }

    [ObservableProperty]
    private ObservableCollection<BookData> _bookDatas = [];

    [ObservableProperty]
    private ObservableCollection<CustomerData> _customerDatas = [];

    [ObservableProperty]
    private ObservableCollection<EmployeeData> _employeeDatas = [];

    [ObservableProperty]
    private ObservableCollection<GoodsData> _goodsDatas = [];

    [ObservableProperty]
    private ObservableCollection<PurchaseDealData> _purchaseDealDatas = [];

    [ObservableProperty]
    private ObservableCollection<SellDealData> _sellDealDatas = [];

    [ObservableProperty]
    private ObservableCollection<StoreData> _storeDatas = [];

    [ObservableProperty]
    private string _registerEmployeeName = "";

    [ObservableProperty]
    private decimal _registerEmployeeSalary = 0;

    [ObservableProperty]
    private string _registerEmployeePassword = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Registered))]
    private int _registerEmplyeeID = -1;

    private bool Registered => RegisterEmplyeeID != -1;

    [RelayCommand]
    private async Task RegisterEmployee()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand(
            """ CALL registerEmployee(@Name,@Salary,@Password,NULL); """
        );
        cmd.Parameters.AddWithValue("@Name", RegisterEmployeeName);
        cmd.Parameters.AddWithValue("Salary", RegisterEmployeeSalary);
        cmd.Parameters.AddWithValue("@Password", RegisterEmployeePassword);
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        RegisterEmplyeeID = reader.GetInt32(0);
        await LoadEmployeeDatas();
    }

    public async Task LoadDatas()
    {
        await Task.WhenAll(
            [
                LoadBookDatas(),
                LoadCustomerDatas(),
                LoadEmployeeDatas(),
                LoadPurchaseDealDatas(),
                LoadSellDealDatas()
            ]
        );
    }

    private async Task LoadBookDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand("""SELECT * FROM "Books";""");
        using var reader = await cmd.ExecuteReaderAsync();
        BookDatas.Clear();
        while (await reader.ReadAsync())
        {
            BookDatas.Add(BookData.CreateFromReader(reader));
        }
    }

    private async Task LoadCustomerDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand("""SELECT * FROM "Customers";""");
        using var reader = await cmd.ExecuteReaderAsync();
        CustomerDatas.Clear();
        while (await reader.ReadAsync())
        {
            CustomerDatas.Add(CustomerData.CreateFromReader(reader));
        }
    }

    private async Task LoadEmployeeDatas()
    {
        if (DataProvider is null)
            return;
        using var cmd = DataProvider.DataSource.CreateCommand("""SELECT * FROM "Employees";""");
        using var reader = await cmd.ExecuteReaderAsync();
        EmployeeDatas.Clear();
        while (await reader.ReadAsync())
        {
            EmployeeDatas.Add(EmployeeData.CreateFromReader(reader));
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
