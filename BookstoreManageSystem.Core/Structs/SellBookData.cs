using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public sealed partial class SellBookData
{
    public int GoodsID { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string BookName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public int Amount { get; set; }
    public decimal Price { get; set; }

    public static SellBookData CreateFromReader(DbDataReader reader)
    {
        return new SellBookData
        {
            GoodsID = reader.GetInt32(0),
            ISBN = reader.GetString(1),
            BookName = reader.GetString(2),
            Author = reader.GetString(3),
            Publisher = reader.GetString(4),
            Introduction = reader.GetString(5),
            Amount = reader.GetInt32(6),
            Price = reader.GetDecimal(7)
        };
    }
}
