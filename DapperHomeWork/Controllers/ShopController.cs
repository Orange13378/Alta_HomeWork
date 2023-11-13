using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DapperHomeWork.Controllers;

using Interfaces.Repositories;
using Models.Shop;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShopController : ControllerBase
{
    private readonly IShopRepository _shopRepository;

    public ShopController(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    [HttpGet]
    public IActionResult GetAllShop()
    {
        var shops = _shopRepository.GetAll();

        return Ok(shops);
    }

    [HttpGet("id")]
    public IActionResult GetShopById(int id)
    {
        if (id <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        var shops = _shopRepository.GetShopById(id);

        if (shops == null)
            return NotFound();

        return Ok(shops);
    }

    [HttpGet("Id/Sort")]
    public IActionResult GetShopSorted(bool isDESC = false)
    {
        var shops = _shopRepository.GetSortedShop(isDESC);

        return Ok(shops);
    }

    [HttpGet("types")]
    public IActionResult GetAllTypeShop()
    {
        var shops = _shopRepository.GetAllShopType();

        return Ok(shops);
    }


    [HttpPost]
    public IActionResult CreateShop([FromBody] Shop? shop)
    {
        if (shop == null)
            return BadRequest("Данные не могут быть пустыми");

        var shops = _shopRepository.GetAll();

        if (shops.Any(login => shop.Login == login.Login))
            return BadRequest("Магазин с таким логином уже существует");

        var rows = _shopRepository.Insert(shop);

        return rows switch
        {
            null => BadRequest("Тип должен быть либо 1 либо 2"),
            false => BadRequest("Не удалось создать магазин"),
            _ => Ok() 
        };
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Update(int id, [FromBody] UpdateShop? shop)
    {
        if (id <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        if (shop == null)
            return BadRequest();

        shop.Id = id;

        if (!_shopRepository.Update(shop))
            return BadRequest();

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
            return BadRequest("Id не может быть меньше или равен 0");

        if (!_shopRepository.Delete(id))
            return BadRequest();

        return Ok();
    }
}
