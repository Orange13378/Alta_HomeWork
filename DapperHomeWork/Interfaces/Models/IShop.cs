namespace DapperHomeWork.Interfaces.Models;

public interface IShop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public int TypeId { get; set; }
    public bool Enabled { get; set; }
}

