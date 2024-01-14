using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using Prometheus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace DapperHomeWork.Controllers;

using Extensions;
using Models.Consts;
using Models.User;
using Models;
using Interfaces.Repositories;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private static string _userName = string.Empty;
    private string _secretKey = "MySuperSecretKey1111";

    private readonly IUserRepository _userRepository;
    private readonly IDistributedCache _redisCache;
    private readonly IBus _bus;

    public UserController(IUserRepository userRepository, IDistributedCache redisCache, IBus bus)
    {
        _userRepository = userRepository;
        _redisCache = redisCache;
        _bus = bus;
    }

    [HttpPost("authorize")]
    [AllowAnonymous]
    public IActionResult Authorize([FromBody] Auth auth)
    {
        ClaimsIdentity? identity = null;

        var workingTimeGauge = Metrics.CreateGauge("working_time_authorize", "Сколько по времени работает метод Autorize");

        var stopwatch = Stopwatch.StartNew();

        var identityString = _redisCache.GetString(auth.Login + auth.Password + _secretKey);

        if (identityString == null)
        {
            identity = GetIdentity(auth.Login, auth.Password);

            if (identity == null)
                return new UnauthorizedResult();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            identityString = JsonSerializer.Serialize(identity, options);

            var redisOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24));

            _redisCache.SetString(auth.Login + auth.Password + _secretKey, identityString, redisOptions);
        }
        else
        {
            var identityContainer = JsonSerializer.Deserialize<ClaimsIdentityContainer>(identityString);

            List<Claim> claimsList = identityContainer.Claims.Values.Select(claim =>
                new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject)).ToList();

            identity = new ClaimsIdentity(
                claimsList,
                identityContainer.AuthenticationType,
                identityContainer.NameClaimType,
                identityContainer.RoleClaimType
            );
        }

        var jwt = GetJwt(identity);

        var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);

        _userName = auth.Login;

        stopwatch.Stop();

        workingTimeGauge.Set(stopwatch.ElapsedMilliseconds);

        return Ok("Bearer " + encoded);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] User user)
    {
        if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Role))
            return BadRequest("Must be correct UserName or Password or Role");

        var existAuth = _userRepository.GetUserByLogin(user.UserName);

        if (existAuth != null)
            return BadRequest("Login Exist in DB");

        var existShop = _userRepository.GetShopById(user.ShopId);

        if (existShop != null)
            return BadRequest("Shop with this shopId not exist");

        user.Password = user.Password.HashPassword();

        _userRepository.Add(user);

        _bus.Publish(user);

        return Created("New User: ", user);
    }

    [HttpPost("updateUser/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateUserById(int id, [FromBody] UpdateUser? user)
    {
        if (id <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        if (user == null)
            return BadRequest();

        if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Role))
            return BadRequest("Must be correct UserName or Role");

        user.Id = id;

        if (!_userRepository.Update(user))
            return BadRequest();

        return Created("New User: ", user);
    }

    [HttpPut("{shopId}")]
    [Authorize]
    public IActionResult AddShopToUser(int shopId)
    {
        if (shopId <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        var currentUser = _userRepository.GetUserByLogin(_userName);

        if (currentUser == null)
            return BadRequest();

        if (!_userRepository.UpdateShopId(currentUser, shopId))
            return BadRequest();

        return Ok();
    }

    [HttpGet("allUsers")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAllUsers()
    {
        var startMemory = GC.GetTotalMemory(true);

        var users = _userRepository.GetAllUsers();
        
        var endMemory = GC.GetTotalMemory(true);
        var memoryUsage = endMemory - startMemory;

        var memoryGauge = Metrics.CreateGauge("memory_usage_get_all_users", "Сколько по времени работает метод Autorize");
        memoryGauge.Set(memoryUsage);

        return Ok(users);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        var currentUser = _userRepository.GetUserById(id);

        if (currentUser == null)
            return BadRequest();

        var currentUserAuth = _userRepository.GetUserByLogin(_userName);

        if (currentUserAuth != null)
            return BadRequest();

        if (currentUser.Id == currentUserAuth.Id)
            return BadRequest("Нельзя удалить себя же");

        if (!_userRepository.Delete(id))
            return BadRequest();

        return Ok();
    }

    private static JwtSecurityToken GetJwt(ClaimsIdentity? identity)
    {
        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.AddHours(AuthOptions.LIFETIME),
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        return jwt;
    }

    private ClaimsIdentity? GetIdentity(string userName, string password)
    {
        var existAuth = _userRepository.GetUserByLogin(userName);

        if (existAuth == null)
            return null;

        if (!existAuth.Password.VerifyPassword(password))
            return null;

        var claims = new Claim[]
        {
            new (ClaimsIdentity.DefaultNameClaimType, existAuth.UserName),
            new (ClaimsIdentity.DefaultRoleClaimType, existAuth.Role)
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
}

#region ClaimsIdentityContainer

public class ClaimsIdentityContainer
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }
    [JsonPropertyName("$AuthenticationType")]
    public string? AuthenticationType { get; set; }
    public bool? IsAuthenticated { get; set; }
    public Claims? Claims { get; set; }
    public string? Label { get; set; }
    public string? Name { get; set; }
    public string? NameClaimType { get; set; }
    public string? RoleClaimType { get; set; }
}

public class Claims
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("$values")]
    public List<ClaimWithParameterlessConstructor>? Values { get; set; }
}

public class ClaimWithParameterlessConstructor
{
    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("Issuer")]
    public string? Issuer { get; set; }

    [JsonPropertyName("OriginalIssuer")]
    public string? OriginalIssuer { get; set; }

    [JsonPropertyName("Properties")]
    public object? Properties { get; set; }

    [JsonPropertyName("Subject")]
    public ClaimsIdentity? Subject { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Value")]
    public string? Value { get; set; }

    [JsonPropertyName("ValueType")]
    public string? ValueType { get; set; }
}

#endregion

