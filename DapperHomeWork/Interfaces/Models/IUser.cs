namespace DapperHomeWork.Interfaces.Models;

public interface IUser
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
}