using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using Dapper;

namespace DapperHomeWork.Realizations;

using Models.Log;
using Interfaces.Repositories;
using Interfaces.Models;
using Options;

public class LogRepository : ILoggerRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;

    public LogRepository(IOptions<ConnectionStringsConfiguration> config)
    {
        _dbConnection = new NpgsqlConnection(config.Value.DbConnection);
    }

    public IEnumerable<ILog> GetAllLogs()
    {
        return _dbConnection.Query<Log>("SELECT * FROM Logs");
    }

    public bool Delete()
    {
        return _dbConnection.Execute("DELETE FROM Logs") > 0;
    }

    public void Dispose() => _dbConnection.Dispose();
}