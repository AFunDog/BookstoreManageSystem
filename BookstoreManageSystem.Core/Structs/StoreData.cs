using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public class StoreData
{
    public string ISBN { get; set; } = string.Empty;
    public int Amount { get; set; }

    public static StoreData CreateFromReader(DbDataReader reader)
    {
        return new StoreData { ISBN = reader.GetString(0), Amount = reader.GetInt32(1) };
    }
}
