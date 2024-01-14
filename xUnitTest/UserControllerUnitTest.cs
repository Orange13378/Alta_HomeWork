using DapperHomeWork.Controllers;
using DapperHomeWork.Interfaces.Repositories;
using DapperHomeWork.Models;
using DapperHomeWork.Models.Consts;
using DapperHomeWork.Models.Shop;
using DapperHomeWork.Models.User;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace xUnitTest;

public class UserControllerUnitTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly UserController _userController;

    public UserControllerUnitTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCache = new Mock<IDistributedCache>();
        _userController = new UserController(_mockUserRepository.Object, _mockCache.Object);
    }

    [Fact]
    public void Authorize_ValidCredentials_ReturnsOkObjectResultWithTokenTest()
    {
        var auth = new Auth { Login = "testLogin", Password = "string" };
        var existingUser = new User
        {
            UserName = "randomUsername",
            Password = "$2a$11$sFdNOdG4WJcz2pnyTK1IZOMKbNGarCaKLZYEydjAusVhmVL21w8Xe",
            Role = "randomRole"
        };

        _mockUserRepository
            .Setup(repo => repo.GetUserByLogin(auth.Login))
            .Returns(existingUser);

        /*_mockCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>);*/

        var result = _userController.Authorize(auth) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var token = result.Value as string;
        Assert.NotNull(token);
        Assert.StartsWith("Bearer ", token);

        var jwt = new JwtSecurityTokenHandler().ReadToken(token.Replace("Bearer ", "")) as JwtSecurityToken;
        Assert.NotNull(jwt);
        Assert.Equal(AuthOptions.ISSUER, jwt.Issuer);
        Assert.Equal(AuthOptions.AUDIENCE, jwt.Audiences.FirstOrDefault());

        Assert.Contains(jwt.Claims, claim =>
        claim.Type == ClaimsIdentity.DefaultNameClaimType &&
        claim.Value == existingUser.UserName);

        Assert.Contains(jwt.Claims, claim =>
        claim.Type == ClaimsIdentity.DefaultRoleClaimType &&
        claim.Value == existingUser.Role);
    }

    [Fact]
    public void Authorize_InvalidCredentials_ReturnsUnauthorizedResultTest()
    {
        var auth = new Auth { Login = "testLogin", Password = "BadPassword" };

        var result = _userController.Authorize(auth);

        Assert.IsType<UnauthorizedResult>(result);

        var unAuthorizeResult = result as UnauthorizedResult;

        Assert.Equal(401, unAuthorizeResult.StatusCode);
    }

    [Fact]
    public void Register_ValidUser_ReturnsCreatedResultTest()
    {
        var user = new User { UserName = "testUser", Password = "testPassword", Role = "user", ShopId = 1 };

        _mockUserRepository
            .Setup(repo => repo.GetUserByLogin(user.UserName))
            .Returns((User)null);

        _mockUserRepository
            .Setup(repo => repo.GetShopById(user.ShopId))
            .Returns((Shop)null);

        var result = _userController.Register(user) as CreatedResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);

        var createdUser = result.Value as User;
        Assert.NotNull(createdUser);
        Assert.Equal(user.UserName, createdUser.UserName);
    }

    [Fact]
    public void Register_UserWithExistingLogin_ReturnsBadRequestTest()
    {
        var existingUser = new User { UserName = "existingUser", Password = "existingPassword", Role = "user" };
        var user = new User { UserName = "existingUser", Password = "newPassword", Role = "user" };

        _mockUserRepository
            .Setup(repo => repo.GetUserByLogin(user.UserName))
            .Returns(existingUser);

        var result = _userController.Register(user) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Login Exist in DB", result.Value);
    }

    [Fact]
    public void Register_UserWithInvalidShopId_ReturnsBadRequestTest()
    {
        var user = new User { UserName = "newUser", Password = "newPassword", Role = "user", ShopId = 11 };
        var existingShop = new Shop { Id = 1, Name = "ExistingShop" };

        _mockUserRepository
            .Setup(repo => repo.GetUserByLogin(user.UserName))
            .Returns((User)null);

        _mockUserRepository
            .Setup(repo => repo.GetShopById(user.ShopId))
            .Returns(existingShop);

        var result = _userController.Register(user) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Shop with this shopId not exist", result.Value);
    }

    [Fact]
    public void Register_EmptyPasswordOrUserNameOrRole_ReturnsBadRequestTest()
    {
        var userWithEmptyUserName = new User { UserName = "", Password = "newPassword", Role = "user" };
        var userWithEmptyPassword = new User { UserName = "newUser", Password = "", Role = "user" };
        var userWithEmptyRole = new User { UserName = "newUser", Password = "newPassword", Role = "" };

        var resultUserName = _userController.Register(userWithEmptyUserName) as BadRequestObjectResult;
        var resultPassword = _userController.Register(userWithEmptyPassword) as BadRequestObjectResult;
        var resultRole = _userController.Register(userWithEmptyRole) as BadRequestObjectResult;

        var results = new List<BadRequestObjectResult>
            {
                resultUserName,
                resultPassword,
                resultRole
            };

        foreach (var result in results)
        {
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Must be correct UserName or Password or Role", result.Value);
        }
    }
}
