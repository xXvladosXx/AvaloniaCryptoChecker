using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services;

public static class ImportCryptoCoinsServices
{
    private const string DbPath = "AppDatabase.db";

    private static string ConnectionString 
        => $"URI=file:{AppDomain.CurrentDomain.BaseDirectory}{DbPath}";

    internal static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> func)
    {
        while (reader.Read())
        {
            yield return func(reader);
        }
    }

    private static async Task CreateDbIfNotExists()
    {
        try
        {
            if (!File.Exists(DbPath))
            {
                await File.WriteAllBytesAsync(DbPath, new byte[0]);

                using var sqlConnection = new SQLiteConnection($"Data Source={DbPath}");
                await sqlConnection.OpenAsync();

                var commandText = @"
create table cryptocoins_stats (
    name varchar(20), 
    datetime datetime, 
    price varchar(20)
);

create table computerparts_stats (
    name varchar(20),
    datetime datetime, 
    price varchar(20)
);";

                var command = new SQLiteCommand(commandText, sqlConnection);

                _ = await command.ExecuteNonQueryAsync();
                await sqlConnection.CloseAsync();
            }
        }
        catch (Exception e)
        {
            _ = e.Message;
            throw;
        }
    }

    public static async Task<CryptoCoinStats[]> LoadFromDbAsync()
    {
        await CreateDbIfNotExists();

        using var sqlConnection = new SQLiteConnection($"Data Source={DbPath}");
        sqlConnection.Open();

        var selectCommandText = "select * from cryptocoins_stats";
        var selectCommand = new SQLiteCommand(selectCommandText, sqlConnection);

        using var reader = await selectCommand.ExecuteReaderAsync();

        static CryptoCoinStats ParseStats((CryptoCoin Coin, (string _, string datetime, string price)[] Stats) data)
        {
            var stat = data.Stats.Select(stat => new Stat(DateTime.Parse(stat.datetime), double.Parse(stat.price))).ToArray();

            return new CryptoCoinStats(data.Coin, stat);
        }

        var result = reader.Select(state => (CoinName: state.GetString(0), StatDateTime: state.GetString(1), Price: state.GetString(2)))
            .GroupBy(state => state.CoinName)
            .Select(coinGroupStats => (Coin: Enum.Parse<CryptoCoin>(coinGroupStats.Key), Stats: coinGroupStats.ToArray()))
            .Select(ParseStats)
            .ToArray();
        
        await reader.CloseAsync();
        await sqlConnection.CloseAsync();

        return result;
    }

    public static async Task ClearAllAndInsertAsync(CryptoCoinStats[] cryptoCoinStats)
    {
        using var sqlConnection = new SQLiteConnection($"Data Source={DbPath}");
        sqlConnection.Open();

        var deleteAllCommand = new SQLiteCommand("delete from cryptocoins_stats", sqlConnection);
        _ = await deleteAllCommand.ExecuteNonQueryAsync();

        var insertionCommands = cryptoCoinStats
            .SelectMany(x => x.Stats.Select(y => (Coin: x.Coin, Moment: y.Time, Price: y.Price))
            .Select(x => $"insert into cryptocoins_stats (name, datetime, price) values ('{x.Coin}', '{x.Moment:yyyy-MM-dd HH:mm:ss}', {x.Price})"))
            .ToArray();

        foreach (var command in insertionCommands)
        {
            var sqlCommand = new SQLiteCommand(command, sqlConnection);
            _ = await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}
