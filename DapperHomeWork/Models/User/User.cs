using System.Text.Json.Serialization;

namespace DapperHomeWork.Models.User;

using DapperHomeWork.Interfaces.Models;

public class User : IUser
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    [JsonIgnore]
    public int ShopId { get; set; }
}