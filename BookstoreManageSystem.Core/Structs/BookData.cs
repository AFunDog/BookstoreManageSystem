using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManageSystem.Core.Structs;

public sealed class BookData
{
    public string ISBN { get; set; } = string.Empty;

    public string BookName { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public string Introduction { get; set; } = string.Empty;

    public static BookData CreateFromReader(DbDataReader reader)
    {
        return new BookData
        {
            ISBN = reader.GetString(0),
            BookName = reader.GetString(1),
            Author = reader.GetString(2),
            Publisher = reader.GetString(3),
            Introduction = reader.GetString(4),
        };
    }
}
