using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using BookstoreManageSystem.Core;
using BookstoreManageSystem.Core.Structs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;

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
    private ObservableCollection<BudgetData> _budgetDatas = [];

    [ObservableProperty]
    private string _registerEmployeeName = "";

    [ObservableProperty]
    private decimal _registerEmployeeSalary = 0;

    [ObservableProperty]
    private string _registerEmployeePassword = "";

    [ObservableProperty]
    private EmployeeData _selectedEmployee = new();

    //[ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(Registered))]
    //private int _registerEmplyeeID = -1;

    //private bool Registered => RegisterEmplyeeID != -1;

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
        try
        {
            await reader.ReadAsync();
            var EID = reader.GetInt32(0);
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Success)
                .WithTitle("注册员工账号成功")
                .WithContent($"EID: {EID}")
                .Dismiss()
                .ByClicking()
                .Queue();
            await LoadEmployeeDatas();
        }
        catch (System.Exception e)
        {
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Error)
                .WithTitle("注册员工账号失败")
                .WithContent(e.Message)
                .Dismiss()
                .ByClicking()
                .Queue();
        }
    }

    [RelayCommand]
    private async Task EditEmployeeSalary(decimal salary)
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """ UPDATE "Employees" SET "Salary" = @Salary WHERE "EmployeeID" = @EID; """
        );
        cmd.Parameters.AddWithValue("@Salary", salary);
        cmd.Parameters.AddWithValue("@EID", SelectedEmployee.EmployeeID);
        try
        {
            await cmd.ExecuteNonQueryAsync();
            await LoadEmployeeDatas();
            App.Instance.SukiToastManager.CreateToast()
                .OfType(NotificationType.Success)
                .WithTitle("修改员工薪水成功")
                .WithContent($"EID: {SelectedEmployee.EmployeeID}, Salary: {salary}")
                .Dismiss()
                .ByClicking()
                .Queue();
        }
        catch (System.Exception e)
        {
            App.Instance.SukiToastManager.CreateToast()
                .WithTitle("修改员工薪水失败")
                .WithContent(e.Message)
                .Dismiss()
                .ByClicking()
                .OfType(NotificationType.Error)
                .Queue();
        }
    }

    public async Task LoadDatas()
    {
        await Task.WhenAll(
            [
                LoadBookDatas(),
                LoadCustomerDatas(),
                LoadEmployeeDatas(),
                LoadPurchaseDealDatas(),
                LoadSellDealDatas(),
                LoadBudgetDatas()
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

    private async Task LoadBudgetDatas()
    {
        if (DataProvider is null)
            return;

        using var costCmd = DataProvider.DataSource.CreateCommand(
            """
            SELECT SUM("DealAmount" * "DealPrice") "cost"
            FROM "PurchaseDeals";
            """
        );
        using var incomeCmd = DataProvider.DataSource.CreateCommand(
            """
            SELECT SUM("DealAmount" * "DealPrice") "income"
            FROM "SellDeals";
            """
        );
        BudgetDatas.Clear();
        try
        {
            using var costReader = await costCmd.ExecuteReaderAsync();
            using var incomeReader = await incomeCmd.ExecuteReaderAsync();
            await costReader.ReadAsync();
            await incomeReader.ReadAsync();
            var cost = costReader.GetDecimal(0);
            var income = incomeReader.GetDecimal(0);
            BudgetDatas.Add(new BudgetData("售出收入", income));
            BudgetDatas.Add(new BudgetData("采购支出", -cost));
            BudgetDatas.Add(new BudgetData("利润", income - cost));
        }
        catch (System.Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}
