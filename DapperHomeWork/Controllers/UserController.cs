using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using static System.String;

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

    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("authorize")]
    [AllowAnonymous]
    public IActionResult Authorize([FromBody] Auth auth)
    {
        var identity = GetIdentity(auth.Login, auth.Password);

        if (identity == null)
            return new UnauthorizedResult();
        
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

        var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);
        
        Log.Information(encoded);

        _userName = auth.Login;
        return Ok("Bearer " + encoded);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] User user)
    {
        if (IsNullOrEmpty(user.Password) || IsNullOrEmpty(user.UserName) || IsNullOrEmpty(user.Role))
            return BadRequest("Must be correct UserName or Password or Role");

        var existAuth = _userRepository.GetUserByLogin(user.UserName);

        if (existAuth != null)
            return BadRequest("Login Exist in DB");

        var existShop = _userRepository.GetShopById(user.ShopId);

        if (existShop != null)
            return BadRequest("Shop with this shopId not exist");

        user.Password = user.Password.HashPassword();
        
        _userRepository.Add(user);

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

        if (IsNullOrEmpty(user.UserName) || IsNullOrEmpty(user.Role))
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
        var users = _userRepository.GetAllUsers();
            
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


    private ClaimsIdentity? GetIdentity(string userName, string password)
    {
        var existAuth = _userRepository.GetUserByLogin(userName);

        if (existAuth == null)
            return null;

        if (!existAuth.Password.VerifyPassword(password))
            return null;
        
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, existAuth.UserName),
            new (ClaimsIdentity.DefaultRoleClaimType, existAuth.Role)
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
}

