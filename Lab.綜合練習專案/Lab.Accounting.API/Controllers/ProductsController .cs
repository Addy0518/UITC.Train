using Lab.Accounting.API.Common.Requests.Products;
using UBOT_Domain.Models.Constants;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class ProductsController(IProductsService productsService) : ControllerBase
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductDetails>))]
    public async Task<IActionResult> GetProducts([FromQuery] int productId)
    {
        return Ok(await productsService.GetProducts(productId));
    }

    /// <summary>
    /// 查看所有商品
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductsSearchRequest request)
    {
        return Ok(await productsService.GetAllProducts(request));
    }

    /// <summary>
    /// 賣家查看自己的所有商品
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    [HttpGet]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> SellerGetAllProducts([FromQuery] ProductsSearchRequest request)
    {
        request.sellerId = CurrentUserId;
        return Ok(await productsService.SellerGetAllProducts(request));
    }

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="productsInsertRequest">新增商品資訊</param>
    /// <returns>審核表 ID </returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> CreateProducts([FromBody] ProductsInsertRequest productsInsertRequest)
    {
        productsInsertRequest.UserId = CurrentUserId;
        return Ok(await productsService.CreateProducts(productsInsertRequest));
    }

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="productsUpdateRequest">商品更新資訊</param>
    /// <returns>審核表 ID </returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateProducts(ProductsUpdateRequest productsUpdateRequest)
    {
        productsUpdateRequest.UserId = CurrentUserId;
        return Ok(await productsService.UpdateProducts(productsUpdateRequest));
    }

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateProductsDeleteStatus([FromBody] IEnumerable<int> productId)
    {
        return Ok(await productsService.UpdateProductsDeleteStatus(CurrentUserId, productId));
    }

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <returns>影響列數</returns>
    [HttpDelete]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> DeleteProducts([FromQuery] int productsId)
    {
        return Ok(await productsService.DeleteProducts(productsId, CurrentUserId));
    }

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="reviewId">審核表 ID</param>
    /// <returns>新增成功的圖片</returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgUpload([FromForm] IFormFile productsImgsFiles, [FromForm] int reviewId)
    {
        return Ok(await productsService.ProductsImgUpload(productsImgsFiles, reviewId));
    }

    /// <summary>
    /// 商品描述的圖片上傳
    /// </summary>
    /// <param name="productsDescriptionImgsFiles">商品描述圖片檔案</param>
    /// <returns>上傳是否成功</returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> ProductsDescriptionImgUpload([FromForm] IFormFile productsDescriptionImgsFiles)
    {
        return Ok(await productsService.ProductsDescriptionImgUpload(productsDescriptionImgsFiles));
    }

    /// <summary>
    /// 刪除商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>刪除的圖片</returns>
    [HttpDelete]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgDelete([FromQuery] int productsImgId)
    {
        return Ok(await productsService.DeleteProductsImg(productsImgId));
    }

    /// <summary>
    /// 查看單一訂單評價
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>商品評價資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RateResponse>))]
    public async Task<IActionResult> GetOrderRate([FromQuery] int orderId)
    {
        return Ok(await productsService.GetOrderRate(orderId));
    }

    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="request">商品評價資訊</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> CreateProductRate([FromBody] ProductsRateRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await productsService.CreateProductRate(request));
    }
}
