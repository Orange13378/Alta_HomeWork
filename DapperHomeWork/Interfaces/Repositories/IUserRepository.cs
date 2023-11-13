using DapperHomeWork.Interfaces.Models;
using DapperHomeWork.Models.User;

namespace DapperHomeWork.Interfaces.Repositories;

public interface IUserRepository
{
    public IEnumerable<User> GetAllUsers();
    public int Add(IUser user);
    public User? GetUserByLogin(string UserName);
    public User? GetUserById(int id);
    public IShop? GetShopById(int id);
    public bool Update(IUser user);
    public bool UpdateShopId(User user, int shopId);
    public bool Delete(int id);
}