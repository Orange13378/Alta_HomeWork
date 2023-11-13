using DapperHomeWork.Interfaces.Repositories;
using DapperHomeWork.Realizations;

namespace DapperHomeWork.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<ILoggerRepository, LogRepository>();
    }
}