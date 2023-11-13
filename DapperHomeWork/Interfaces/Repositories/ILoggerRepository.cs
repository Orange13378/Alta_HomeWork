namespace DapperHomeWork.Interfaces.Repositories;

using Models;

public interface ILoggerRepository
{
    public IEnumerable<ILog> GetAllLogs();
    public bool Delete();
}