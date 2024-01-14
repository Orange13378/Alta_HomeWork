namespace DapperHomeWork.Configuration;

using Interfaces.Repositories;
using Realizations;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<ILoggerRepository, LogRepository>();
    }
}