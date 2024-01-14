using DapperHomeWork.Interfaces.Models;
using MassTransit;

namespace DapperHomeWork;

public class NewUserConsumer : IConsumer<IUser>
{
    public async Task Consume(ConsumeContext<IUser> context)
    {
        await Task.Delay(1000);
        Console.WriteLine(context.Message.UserName);
    }
}