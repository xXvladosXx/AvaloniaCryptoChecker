using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services;

public static class ImportComputerPartsServices
{
    private const string DbPath = "AppDatabase.db";

    private static string ConnectionString
        => $"URI=file:{AppDomain.CurrentDomain.BaseDirectory}{DbPath}";

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

    public static async Task<ComputerPartStats[]> LoadFromDbAsync()
    {
        await CreateDbIfNotExists();

        using var sqlConnection = new SQLiteConnection($"Data Source={DbPath}");
        sqlConnection.Open();

        var selectCommandText = "select * from computerparts_stats";
        var selectCommand = new SQLiteCommand(selectCommandText, sqlConnection);

        using var reader = await selectCommand.ExecuteReaderAsync();

        static ComputerPartStats ParseStats((ComputerPart Part, (string _, string datetime, string price)[] Stats) data)
        {
            var stat = data.Stats.Select(stat => new Stat(DateTime.Parse(stat.datetime), double.Parse(stat.price))).ToArray();

            return new ComputerPartStats(data.Part, stat);
        }

        var result = reader.Select(state => (CoinName: state.GetString(0), StatDateTime: state.GetString(1), Price: state.GetString(2)))
            .GroupBy(state => state.CoinName)
            .Select(coinGroupStats => (Coin: Enum.Parse<ComputerPart>(coinGroupStats.Key), Stats: coinGroupStats.ToArray()))
            .Select(ParseStats)
            .ToArray();

        await reader.CloseAsync();
        await sqlConnection.CloseAsync();

        return result;
    }

    public static async Task ClearAllAndInsertAsync(ComputerPartStats[] cryptoPartStats)
    {
        using var sqlConnection = new SQLiteConnection($"Data Source={DbPath}");
        sqlConnection.Open();

        var deleteAllCommand = new SQLiteCommand("delete from computerparts_stats", sqlConnection);
        _ = await deleteAllCommand.ExecuteNonQueryAsync();

        var insertionCommands = cryptoPartStats
            .SelectMany(x => x.Stats.Select(y => (Part: x.Part, Moment: y.Time, Price: y.Price))
            .Select(x => $"insert into computerparts_stats (name, datetime, price) values ('{x.Part}', '{x.Moment:yyyy-MM-dd HH:mm:ss}', {x.Price})"))
            .ToArray();

        foreach (var command in insertionCommands)
        {
            var sqlCommand = new SQLiteCommand(command, sqlConnection);
            _ = await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}
