using System.Text.Json.Serialization;

namespace DapperHomeWork.Models.Shop;

using DapperHomeWork.Interfaces.Models;

public class UpdateShop : IShop
{
    [JsonIgnore]
    public int Id { get; set; }
    public DateTime ChangeDate { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public int TypeId { get; set; }
    public bool Enabled { get; set; }
}

