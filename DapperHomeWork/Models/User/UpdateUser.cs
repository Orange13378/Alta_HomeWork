using DapperHomeWork.Interfaces.Models;
using Newtonsoft.Json;

namespace DapperHomeWork.Models.User;

public class UpdateUser : IUser
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
}