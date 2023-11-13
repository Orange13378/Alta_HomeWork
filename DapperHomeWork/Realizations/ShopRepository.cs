using System.Data;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Options;

namespace DapperHomeWork.Realizations;

using Models.Shop;
using Options;
using Interfaces.Repositories;
using DapperHomeWork.Interfaces.Models;

public class ShopRepository : IShopRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;

    public ShopRepository(IOptions<ConnectionStringsConfiguration> config)
    {
        _dbConnection = new NpgsqlConnection(config.Value.DbConnection);
    }

    public IEnumerable<IShop> GetAll()
    {
        return _dbConnection.Query<Shop>("SELECT * FROM Shops");
    }

    public IEnumerable<ShopType> GetAllShopType()
    {
        return _dbConnection.Query<ShopType>("SELECT * FROM ShopTypes");
    }

    public IShop? GetShopById(int id)
    {
        return _dbConnection.QueryFirstOrDefault<Shop>("SELECT * FROM Shops WHERE Id = @Id", new { Id = id });
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

        return _dbConnection.Execute("""
        UPDATE Shops SET ChangeDate = @ChangeDate, Name = @Name, 
        Code = @Code, Address = @Address, TypeId = @TypeId, Login = @Login, Enabled = @Enabled 
        WHERE Id = @Id
        """, shop) > 0;
    }

    public bool Delete(int id)
    {
        return _dbConnection.Execute("DELETE FROM Shops WHERE Id = @Id", new { Id = id }) > 0;
    }

    private bool IsNullShopType(IShop shop)
    {
        var shopType = GetAllShopType();

        return shopType.FirstOrDefault(x => x.Id == shop.TypeId) == null;
    }

    public void Dispose() => _dbConnection.Dispose();
}

