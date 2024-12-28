using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public class EmployeeData
{
    public int EmployeeID { get; set; }
    public DateTime Hiredate { get; set; }
    public decimal Salary { get; set; }
    public string Name { get; set; } = string.Empty;

    public static EmployeeData CreateFromReader(DbDataReader reader)
    {
        return new EmployeeData
        {
            EmployeeID = reader.GetInt32(0),
            Hiredate = reader.GetDateTime(1),
            Salary = reader.GetDecimal(2),
            Name = reader.GetString(3)
        };
    }
}
