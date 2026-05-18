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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> GetProducts([FromQuery] int productId)
    {
        return Ok(await productsService.GetProducts(productId));
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
        return Ok(await productsService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10));
    }

    /// <summary>
    /// 查看賣家所有商品
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <param name="isDelete">是否為刪除狀態</param>
    /// <returns>商品列表</returns>
    [HttpGet]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ProductsResponse>>))]
    public async Task<IActionResult> GetSellerAllProducts(
        [FromQuery] int? pageIndex,
        [FromQuery] int? pageSize,
        [FromQuery] IsDeleteStatusEnum? isDelete = IsDeleteStatusEnum.Normal
    )
    {
        return Ok(await productsService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10, CurrentUserId, isDelete));
    }

    /// <summary>
    /// 查看商品類別
    /// </summary>
    /// <param name="productcategoryId">商品類別 ID</param>
    /// <returns>商品類別</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ProductsResponse>>))]
    public async Task<IActionResult> GetCategory([FromQuery] int? productcategoryId = null)
    {
        return Ok(await productsService.GetCategory(productcategoryId));
    }

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="productsInsertRequest">新增商品資訊</param>
    /// <returns>商品資訊</returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> CreateProducts([FromBody] ProductsInsertRequest productsInsertRequest)
    {
        productsInsertRequest.UserId = CurrentUserId;
        return Ok(await productsService.CreateProducts(productsInsertRequest));
    }

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="productsUpdateRequest">商品更新資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductsResponse>))]
    public async Task<IActionResult> DeleteProducts([FromQuery] int productsId)
    {
        return Ok(await productsService.DeleteProducts(productsId, CurrentUserId));
    }

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="productId">商品 Id</param>
    /// <returns>新增成功的圖片</returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> ProductsImgUpload([FromForm] IFormFile productsImgsFiles, [FromForm] int productId)
    {
        return Ok(await productsService.ProductsImgUpload(productsImgsFiles, productId));
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
