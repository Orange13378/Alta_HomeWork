using DapperHomeWork.Interfaces.Models;

namespace DapperHomeWork.Models.User;

public class UpdateUser : IUser
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
}