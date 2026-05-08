using UBOT_Domain.Models.Constants;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class ShoppingCarController(IShoppingCarService shoppingCarService) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 查看購物車中的所有商品
    /// </summary>
    /// <returns>購物車中的所有商品</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallShoppingCar>>))]
    public async Task<IActionResult> GetAllProductsInShoppingCar()
    {
        return Ok(await shoppingCarService.GetAllProductsInShoppingCar(CurrentUserId));
    }

    /// <summary>
    /// 新增單一商品到購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> AddProductsInShoppingCar(int productsId)
    {
        return Ok(await shoppingCarService.AddProductsInShoppingCar(productsId, CurrentUserId));
    }

    /// <summary>
    /// 刪除單一商品從購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <returns>影響列數</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> DeleteProductsInShoppingCar(int productsId)
    {
        return Ok(await shoppingCarService.DeleteProductsInShoppingCar(productsId, CurrentUserId));
    }
}
