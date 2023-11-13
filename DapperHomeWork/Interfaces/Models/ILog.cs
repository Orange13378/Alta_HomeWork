namespace DapperHomeWork.Interfaces.Models;

public interface ILog
{
    public int Id { get; set; }
    public string LogLevel { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}