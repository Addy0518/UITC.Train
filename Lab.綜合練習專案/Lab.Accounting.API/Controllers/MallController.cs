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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LedgerItemJoinCategoryView>))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LedgerItemJoinCategoryView>))]
    public async Task<IActionResult> GetAllProducts([FromQuery] int? pageIndex, [FromQuery] int? pageSize)
    {
        return Ok(await mallService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10));
    }

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <param name="productcategory">商品類別</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LedgerItemJoinCategoryView>))]
    public async Task<IActionResult> CreateProducts(ProductsInsertRequest productsInsertRequest)
    {
        productsInsertRequest.UserId = CurrentUserId;
        return Ok(await mallService.CreateProducts(productsInsertRequest));
    }
}
