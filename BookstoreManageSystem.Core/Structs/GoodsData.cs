using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public class GoodsData
{
    public int GoodsID { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public int Amount { get; set; }
    public decimal Price { get; set; }

    public static GoodsData CreateFromReader(DbDataReader reader)
    {
        return new GoodsData
        {
            GoodsID = reader.GetInt32(0),
            ISBN = reader.GetString(1),
            Amount = reader.GetInt32(2),
            Price = reader.GetDecimal(3)
        };
    }
}
