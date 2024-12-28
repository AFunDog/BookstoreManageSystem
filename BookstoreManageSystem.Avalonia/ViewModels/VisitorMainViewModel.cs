using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManageSystem.Core;
using BookstoreManageSystem.Core.Structs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManageSystem.Avalonia.ViewModels;

internal sealed partial class VisitorMainViewModel : ViewModelBase
{
    public SQLDataProvider? DataProvider { get; set; }

    [ObservableProperty]
    private ObservableCollection<SellBookData> _sellBookDatas = [];

    public async Task LoadDatas()
    {
        await Task.WhenAll([LoadSellBookDatas()]);
    }

    private async Task LoadSellBookDatas()
    {
        if (DataProvider is null)
            return;

        using var cmd = DataProvider.DataSource.CreateCommand(
            """ SELECT * FROM "SellBooksView"; """
        );
        using var reader = await cmd.ExecuteReaderAsync();
        SellBookDatas.Clear();
        while (await reader.ReadAsync())
        {
            SellBookDatas.Add(SellBookData.CreateFromReader(reader));
        }
    }
}
