using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;

namespace BookstoreManageSystem.Core;

public enum LoginType
{
    Visitor,
    Customer,
    Employee,
    Owner
}

public sealed class SQLDataProvider : IDisposable
{
    internal static Serilog.ILogger Logger { get; } =
        new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Verbose().CreateLogger();

    public NpgsqlDataSource DataSource { get; private set; }
    public LoginType LoginType { get; internal set; }

    internal SQLDataProvider(string username, string password)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Host = "localhost",
            Port = 5432,
            Username = username,
            Password = password,
            Database = "Bookstore"
        };
        var builder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);
        builder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog(Logger)));
        DataSource = builder.Build();
    }

    public static async Task<SQLDataProvider?> LoginAsVisitor()
    {
        try
        {
            var provider = new SQLDataProvider("Visitor", "Visitor")
            {
                LoginType = LoginType.Visitor
            };
            // 这一步用来检验连接是否有效
            using var c = await provider.DataSource.OpenConnectionAsync();
            return provider;
        }
        catch (Exception e)
        {
            Logger.Error(e, "以游客方式登录时错误");
        }
        return null;
    }

    public static async Task<SQLDataProvider?> LoginAsCustomer(string userInfo, string password)
    {
        try
        {
            using var tempPro = await LoginAsVisitor();
            int tempUID = int.TryParse(userInfo, out var value) ? value : -1;
            using var tempCmd = tempPro!.DataSource.CreateCommand(
                $"""SELECT * FROM getUIDFromUserInfo(@uid,@phone,@email)"""
            );
            tempCmd.Parameters.AddWithValue("@uid", tempUID);
            tempCmd.Parameters.AddWithValue("@phone", userInfo);
            tempCmd.Parameters.AddWithValue("@email", userInfo);
            using var reader = await tempCmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            int uid = reader.GetInt32(0);
            var provider = new SQLDataProvider($"Customer_{uid}", password)
            {
                LoginType = LoginType.Customer
            };
            // 这一步用来检验连接是否有效
            using var c = await provider.DataSource.OpenConnectionAsync();
            return provider;
        }
        catch (Exception e)
        {
            Logger.Error(e, "以顾客方式登录时错误");
        }
        return null;
    }

    public static async Task<SQLDataProvider?> LoginAsEmployee(string username, string password)
    {
        try
        {
            if (int.TryParse(username, out var value))
            {
                username = $"Employee_{value}";
            }
            var provider = new SQLDataProvider(username, password)
            {
                LoginType = LoginType.Employee
            };
            // 这一步用来检验连接是否有效
            using var c = await provider.DataSource.OpenConnectionAsync();
            return provider;
        }
        catch (Exception e)
        {
            Logger.Error(e, "以员工方式登录时错误");
        }
        return null;
    }

    public static async Task<SQLDataProvider?> LoginAsOwner(string password)
    {
        try
        {
            var provider = new SQLDataProvider("StoreOwner", password)
            {
                LoginType = LoginType.Owner
            };
            // 这一步用来检验连接是否有效
            using var c = await provider.DataSource.OpenConnectionAsync();
            return provider;
        }
        catch (Exception e)
        {
            Logger.Error(e, "以店主方式登录时错误");
        }
        return null;
    }

    public void Dispose()
    {
        DataSource.Dispose();
    }
}
