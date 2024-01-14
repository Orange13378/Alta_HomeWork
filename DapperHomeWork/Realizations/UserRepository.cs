using Dapper;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace DapperHomeWork.Realizations;

using DapperHomeWork.Interfaces.Models;
using Interfaces.Repositories;
using Models.Shop;
using Models.User;
using Options;

public class UserRepository: IUserRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public UserRepository(IOptions<ConnectionStringsConfiguration> config, IMemoryCache cache)
    {
        _dbConnection = new NpgsqlConnection(config.Value.DbConnection);
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions();
        _cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
    }

    public int GetUsersCount()
    {
        return _dbConnection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _dbConnection.Query<User>("SELECT * FROM Users;");
    }

    public int Add(IUser user)
    {
        return _dbConnection.Execute("INSERT INTO Users (UserName, Password, Role) " +
                                     "VALUES (@UserName, @Password, @Role);", user);
    }

    public IShop? GetShopById(int id)
    {

        if (_cache.TryGetValue(nameof(GetShopById) + id, out IShop? shop))
            return shop;

        shop = _dbConnection.QueryFirstOrDefault<Shop>("SELECT * FROM Shops WHERE Id = @Id",
            new { Id = id });

        if (shop == null)
            return null;

        _cache.Set(nameof(GetShopById) + id, shop, _cacheOptions);

        return shop;
    }

    public User? GetUserByLogin(string userName)
    {
        var user = _dbConnection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE UserName = @UserName",
            new { UserName = userName });
        
        return user;
    }

    public bool Update(IUser user)
    {
        if (GetUserById(user.Id) == null)
            return false;

        if (GetUserByLogin(user.UserName!) != null)
            return false;

        if (_dbConnection.Execute("""
        UPDATE Users SET UserName = @UserName, Role = @Role 
        WHERE Id = @Id
        """, user) <= 0) 
            return false;

        _cache.Set(nameof(GetUserById) + user.Id, user, _cacheOptions);
        
        return true;
    }

    public bool UpdateShopId(User user, int shopId)
    {
        var shop = GetShopById(shopId);

        if (shop == null)
            return false;

        user.ShopId = shopId;

        if (_dbConnection.Execute("""
        UPDATE Users SET ShopId = @ShopId 
        WHERE Id = @Id
        """, user) <= 0) 
            return false;
        
        _cache.Set(nameof(GetUserById) + user.Id, user, _cacheOptions);
        
        return true;
    }

    public User? GetUserById(int id)
    {
        if (_cache.TryGetValue(nameof(GetUserById) + id, out User? user))
            return user;

        user = _dbConnection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Id = @Id",
            new { Id = id });

        if (user == null)
            return null;

        _cache.Set(nameof(GetUserById) + id, user, _cacheOptions);

        return user;
    }

    public bool Delete(int id)
    {
        _cache.Remove(nameof(GetUserById) + id);
        return _dbConnection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = id }) > 0;
    }

    public int DeleteAll()
    {
        return _dbConnection.Execute("DELETE FROM Users;");
    }

    public void Dispose() => _dbConnection.Dispose();
}