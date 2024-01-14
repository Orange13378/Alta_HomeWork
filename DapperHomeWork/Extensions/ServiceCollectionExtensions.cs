using MassTransit;

namespace DapperHomeWork.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBus(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<NewUserConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configurationSection.GetValue<string>("Host")), h =>
                {
                    h.Username(configurationSection.GetValue<string>("User"));
                    h.Password(configurationSection.GetValue<string>("Password"));
                });

                cfg.ReceiveEndpoint("new_user", u =>
                {
                    u.ConfigureConsumer<NewUserConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}