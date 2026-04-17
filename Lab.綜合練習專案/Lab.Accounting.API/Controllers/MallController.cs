using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class MallController(IMallService mallService) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 查看單一商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> GetProducts([FromQuery] int productId)
    {
        return Ok(await mallService.GetProducts(productId, CurrentUserId));
    }

    /// <summary>
    /// 查看所有商品 ( 分頁 )
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <returns>商品列表</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ProductsResponse>>))]
    public async Task<IActionResult> GetAllProducts([FromQuery] int? pageIndex, [FromQuery] int? pageSize)
    {
        return Ok(await mallService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10));
    }

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="productsInsertRequest">新增商品資訊</param>
    /// <returns>商品資訊</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> CreateProducts([FromBody] ProductsInsertRequest productsInsertRequest)
    {
        productsInsertRequest.UserId = CurrentUserId;
        return Ok(await mallService.CreateProducts(productsInsertRequest));
    }

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="productId">商品 Id</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgUpload([FromForm] IFormFile productsImgsFiles, [FromForm] int productId)
    {
        return Ok(await mallService.ProductsImgUpload(productsImgsFiles, productId));
    }

    /// <summary>
    /// 商品圖片刪除
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>影響列數</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgDelete([FromQuery] int productsImgId)
    {
        return Ok(await mallService.ProductsImgDelete(productsImgId));
    }

    /// <summary>
    /// 使用者購買商品並評分
    /// </summary>
    /// <param name="Request">商品購買資訊 </param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UserBuyProductAndRate([FromBody] ProductsBuyRequest Request)
    {
        Request.UserId = CurrentUserId;
        return Ok(await mallService.UserBuyProductAndRate(Request));
    }

    /// <summary>
    /// 查看購物車中的所有商品
    /// </summary>
    /// <returns>購物車中的所有商品</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallShoppingCar>>))]
    public async Task<IActionResult> GetAllProductsInShoppingCar()
    {
        var userId = CurrentUserId;
        return Ok(await mallService.GetAllProductsInShoppingCar(userId));
    }
}
