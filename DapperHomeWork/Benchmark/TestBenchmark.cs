using System.Text.Json;
//using BenchmarkDotNet.Attributes;
using DapperHomeWork.Interfaces.Repositories;
using DapperHomeWork.Models.User;
using DapperHomeWork.Realizations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace DapperHomeWork.Benchmark;

//[MemoryDiagnoser]
public class TestBenchmark
{
    /*private IDistributedCache _redisCache;
    private IUserRepository _userRepository;
    private Random _random;

    [Params(10000)]
    public int DataCount;

    [GlobalSetup]
    public void GlobalSetup()
    {
        RedisCacheOptions options = new RedisCacheOptions
        {
            Configuration = "192.168.0.103",
            InstanceName = "Redis Server"
        };

        _redisCache = new RedisCache(options);

        _userRepository = new UserRepository();

        _random = new Random();

        for (int i = 0; i < DataCount; i++)
        {
            User newUser = new User
            {
                Password = $"string{i}",
                Role = $"role{i}",
                UserName = $"name{i}"
            };

            _userRepository.Add(newUser);
        }

        for (int i = 0; i < DataCount; i++)
        {
            var user = _userRepository.GetUserById(i);
            var userString = JsonSerializer.Serialize(user);
            _redisCache.SetString($"user{i}", userString);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _userRepository.DeleteAll();
        
        for (int i = 0; i < DataCount; i++)
        {
            _redisCache.Remove($"user{i}");
        }
    }

    [Benchmark]
    public string TestRedisCache()
    {
        int id = _random.Next(0, DataCount);
        string userString = _redisCache.GetString("user" + id);
        return userString;
    }

    [Benchmark(Baseline = true)]
    public User? TestDB()
    {
        int id = _random.Next(0, DataCount);
        return _userRepository.GetUserByLogin($"name{id}");
    }*/
}
