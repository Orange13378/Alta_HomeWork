using Dapper;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;

namespace DapperHomeWork.Realizations;

using DapperHomeWork.Interfaces.Models;
using Interfaces.Repositories;
using Models.Shop;
using Models.User;
using Options;

public class UserRepository: IUserRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;

    public UserRepository(IOptions<ConnectionStringsConfiguration> config)
    {
        _dbConnection = new NpgsqlConnection(config.Value.DbConnection);
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
        return _dbConnection.QueryFirstOrDefault<Shop>("""
             SELECT * FROM Shops WHERE Id = @Id
             """,
            new { Id = id });
    }

    public User? GetUserByLogin(string userName)
    {
        return _dbConnection.QueryFirstOrDefault<User>("""
             SELECT * FROM Users WHERE UserName = @UserName
             """,
            new { UserName = userName });
    }

    public bool Update(IUser user)
    {
        if (GetUserById(user.Id) == null)
            return false;

        if (GetUserByLogin(user.UserName) != null)
            return false;

        return _dbConnection.Execute("""
        UPDATE Users SET UserName = @UserName, Role = @Role 
        WHERE Id = @Id
        """, user) > 0;
    }

    public bool UpdateShopId(User user, int shopId)
    {
        var shop = GetShopById(shopId);

        if (shop == null)
            return false;

        user.ShopId = shopId;

        return _dbConnection.Execute("""
        UPDATE Users SET ShopId = @ShopId 
        WHERE Id = @Id
        """, user
        ) > 0;
    }

    public User? GetUserById(int id)
    {
        return _dbConnection.QueryFirstOrDefault<User>("""
             SELECT * FROM Users WHERE Id = @Id
             """, new { Id = id });
    }

    public bool Delete(int id)
    {
        return _dbConnection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = id }) > 0;
    }

    public void Dispose() => _dbConnection.Dispose();
}