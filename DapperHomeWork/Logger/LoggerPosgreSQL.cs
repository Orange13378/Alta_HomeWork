using Npgsql;

namespace DapperHomeWork.Logger;

public class LoggerPosgreSQL
{
    public static async Task LogToPostgreSQLAsync(string level, string message, string timestamp)
    {
        //var connectionSring = "Host=postgres;Port=5432;Database=homework;Username=postgres;Password=Muslim213;";
        var connectionSring = "Host=localhost;Port=5432;Database=homework;Username=postgres;Password=Muslim213;";

        using (var connection = new NpgsqlConnection(connectionSring))
        {
            await connection.OpenAsync();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;
                
                command.CommandText = """
                INSERT INTO Logs (LogLevel, Message, CreatedAt)
                VALUES (@level, @message, @time);
                """;

                command.Parameters.AddWithValue("@level", level);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@time", DateTime.Parse(timestamp));

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}