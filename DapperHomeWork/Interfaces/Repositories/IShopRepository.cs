namespace DapperHomeWork.Interfaces.Repositories;

using DapperHomeWork.Models.Shop;
using Models;

public interface IShopRepository
{
    public IEnumerable<Shop> GetAll();
    public IEnumerable<ShopType> GetAllShopType();
    public IShop? GetShopById(int id);
    public IShop? GetShopByLogin(string login);
    public IEnumerable<IShop> GetSortedShop(bool isDesc);
    public bool? Insert(IShop shop);
    public bool Update(IShop shop);
    public bool Delete(int id);
}

