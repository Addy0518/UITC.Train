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
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> GetProducts([FromQuery] int productId)
    {
        return Ok(await mallService.GetProducts(productId));
    }

    /// <summary>
    /// 查看所有商品
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
    /// 查看賣家所有商品
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <param name="isDelete">是否為刪除狀態</param>
    /// <returns>商品列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ProductsResponse>>))]
    public async Task<IActionResult> GetSellerAllProducts(
        [FromQuery] int? pageIndex,
        [FromQuery] int? pageSize,
        [FromQuery] bool? isDelete = false
    )
    {
        return Ok(await mallService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10, CurrentUserId, isDelete));
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
    /// 更新單一商品
    /// </summary>
    /// <param name="productsUpdateRequest">商品更新資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> UpdateProducts(ProductsUpdateRequest productsUpdateRequest)
    {
        productsUpdateRequest.UserId = CurrentUserId;
        return Ok(await mallService.UpdateProducts(productsUpdateRequest));
    }

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> UpdateProductsDeleteStatus([FromBody] IEnumerable<int> productId)
    {
        return Ok(await mallService.UpdateProductsDeleteStatus(CurrentUserId, productId));
    }

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <returns>影響列數</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> DeleteProducts([FromQuery] int productsId)
    {
        return Ok(await mallService.DeleteProducts(productsId, CurrentUserId));
    }

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="productId">商品 Id</param>
    /// <returns>新增成功的圖片</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgUpload([FromForm] IFormFile productsImgsFiles, [FromForm] int productId)
    {
        return Ok(await mallService.ProductsImgUpload(productsImgsFiles, productId));
    }

    /// <summary>
    /// 刪除商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>刪除的圖片</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgDelete([FromQuery] int productsImgId)
    {
        return Ok(await mallService.DeleteProductsImg(productsImgId));
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
        return Ok(await mallService.GetAllProductsInShoppingCar(CurrentUserId));
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
        return Ok(await mallService.AddProductsInShoppingCar(productsId, CurrentUserId));
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
        return Ok(await mallService.DeleteProductsInShoppingCar(productsId, CurrentUserId));
    }
}
