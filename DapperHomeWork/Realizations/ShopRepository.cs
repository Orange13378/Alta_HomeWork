using System.Data;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace DapperHomeWork.Realizations;

using Models.Shop;
using Options;
using Interfaces.Repositories;
using Interfaces.Models;

public class ShopRepository : IShopRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public ShopRepository(IOptions<ConnectionStringsConfiguration> config, IMemoryCache cache)
    {
        _dbConnection = new NpgsqlConnection(config.Value.DbConnection);
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions();
        _cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
    }

    public IEnumerable<Shop> GetAll()
    {
        return _dbConnection.Query<Shop>("SELECT * FROM Shops");
    }

    public IEnumerable<ShopType> GetAllShopType()
    {
        if (_cache.TryGetValue(nameof(GetAllShopType), out IEnumerable<ShopType>? shopTypes))
            return shopTypes!;

        shopTypes = _dbConnection.Query<ShopType>("SELECT * FROM ShopTypes");

        _cache.Set(nameof(GetAllShopType), shopTypes, _cacheOptions);

        return shopTypes;
    }

    public IShop? GetShopById(int id)
    {
        if (_cache.TryGetValue(nameof(GetShopById) + id, out IShop? shop)) 
            return shop;

        shop = _dbConnection.QueryFirstOrDefault<Shop>("SELECT * FROM Shops WHERE Id = @Id", new { Id = id });

        if (shop == null)
            return null;

        _cache.Set(nameof(GetShopById) + id, shop);

        return shop;
    }

    public IShop? GetShopByLogin(string login)
    {
        var shop = _dbConnection.QueryFirstOrDefault<Shop>("SELECT * FROM Shops WHERE Login = @Login",
            new { UserName = login });

        return shop;
    }

    public IEnumerable<IShop> GetSortedShop(bool isDesc)
    {
        if (isDesc) 
            return _dbConnection.Query<Shop>("SELECT * FROM Shops ORDER BY Id DESC;");
        else
            return _dbConnection.Query<Shop>("SELECT * FROM Shops ORDER BY Id ASC;");
    }

    public bool? Insert(IShop shop)
    {
        return IsNullShopType(shop) ? null :
            _dbConnection.Execute("INSERT INTO Shops (CreateDate, ChangeDate, Name, Code, Address, Phone, TypeId, Login, Enabled) " +
                                  "VALUES (@CreateDate, @ChangeDate, @Name, @Code, @Address, @Phone, @TypeId, @Login, @Enabled);",
                shop) > 0;
    }

    public bool Update(IShop shop)
    {
        if (IsNullShopType(shop) || GetShopById(shop.Id) == null)
            return false;

        if (_dbConnection.Execute("""
        UPDATE Shops SET ChangeDate = @ChangeDate, Name = @Name, 
        Code = @Code, Address = @Address, TypeId = @TypeId, Enabled = @Enabled 
        WHERE Id = @Id
        """, shop) <= 0) 
            return false;

        _cache.Set(nameof(GetShopById) + shop.Id, shop, _cacheOptions);
        
        return true;
    }

    public bool Delete(int id)
    {
        _cache.Remove(nameof(GetShopById) + id);

        return _dbConnection.Execute("DELETE FROM Shops WHERE Id = @Id", new { Id = id }) > 0;
    }

    private bool IsNullShopType(IShop shop)
    {
        var shopType = GetAllShopType();

        return shopType.FirstOrDefault(x => x.Id == shop.TypeId) == null;
    }

    public void Dispose() => _dbConnection.Dispose();
}

