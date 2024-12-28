using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public class CustomerData
{
    public int UID { get; set; }

    public DateTime RegisterTime { get; set; }

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public static CustomerData CreateFromReader(DbDataReader reader)
    {
        return new CustomerData
        {
            UID = reader.GetInt32(0),
            RegisterTime = reader.GetDateTime(1),
            Address = reader.GetString(2),
            Phone = reader.GetString(3),
            Email = reader.GetString(4),
            Name = reader.GetString(5)
        };
    }
}
