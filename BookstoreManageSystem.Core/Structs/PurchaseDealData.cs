using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public class PurchaseDealData
{
    public int DealID { get; set; }
    public DateTime DealDate { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public int DealAmount { get; set; }
    public decimal DealPrice { get; set; }
    public int EmployeeID { get; set; }

    public static PurchaseDealData CreateFromReader(DbDataReader reader)
    {
        return new PurchaseDealData
        {
            DealID = reader.GetInt32(0),
            DealDate = reader.GetDateTime(1),
            ISBN = reader.GetString(2),
            DealAmount = reader.GetInt32(3),
            DealPrice = reader.GetDecimal(4),
            EmployeeID = reader.GetInt32(5)
        };
    }
}
