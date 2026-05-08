using UBOT_Domain.Models.Constants;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class MallController(IMallService mallService) : ControllerBase
{
    // 公開網址基底給綠界呼叫
    private string tuuneUrl = "https://veneering-bannister-outlook.ngrok-free.dev";

    // 前端網址基底
    private string fronturl = "http://localhost:5174";

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
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ProductsResponse>>))]
    public async Task<IActionResult> GetSellerAllProducts(
        [FromQuery] int? pageIndex,
        [FromQuery] int? pageSize,
        [FromQuery] IsDeleteStatusEnum? isDelete = IsDeleteStatusEnum.Normal
    )
    {
        return Ok(await mallService.GetAllProducts(pageIndex ?? 0, pageSize ?? 10, CurrentUserId, isDelete));
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
        return Ok(await mallService.GetCategory(productcategoryId));
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
        return Ok(await mallService.CreateProducts(productsInsertRequest));
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
        return Ok(await mallService.UpdateProducts(productsUpdateRequest));
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
        return Ok(await mallService.UpdateProductsDeleteStatus(CurrentUserId, productId));
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
        return Ok(await mallService.DeleteProducts(productsId, CurrentUserId));
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
        return Ok(await mallService.ProductsImgUpload(productsImgsFiles, productId));
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
        return Ok(await mallService.DeleteProductsImg(productsImgId));
    }

    /// <summary>
    /// 使用者購買商品並跳轉綠界界面
    /// </summary>
    /// <param name="Request">商品購買資訊 </param>
    /// <returns>訂單 ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UserBuyProduct([FromBody] ProductsBuyRequest Request)
    {
        Request.UserId = CurrentUserId;
        var target = await mallService.UserBuyProduct(Request);

        int orderId = target.ReturnData;

        var payment = await mallService.GetPaymentData(orderId, CurrentUserId, tuuneUrl);
        return Ok(payment);
    }

    /// <summary>
    /// 接收綠界回傳資料
    /// </summary>
    /// <param name="collection">綠界回傳的表單資料</param>
    /// <returns>訂單 ID</returns>
    [HttpPost]
    [AllowAnonymous]
    //綠界傳回來的表單是傳統的表單格式,用這串來確定能接收
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> EcPayBack([FromForm] IFormCollection collection)
    //IformCollection就是接收傳統表單資料的,formform是用來接收html的form提交資料
    //而formbody則是接收Json資料的
    {
        //用serivice的設定訂單方法
        var result = await mallService.SetPaymentData(collection);

        return Content(result);
    }

    /// <summary>
    /// 綠界回來之後再呼叫的API(這裡是中繼站)
    /// </summary>
    /// <param name="collection">綠界回傳的表單資料</param>
    /// <returns>訂單 ID</returns>
    [HttpPost]
    [AllowAnonymous]
    public IActionResult PaymentCallback([FromForm] IFormCollection collection)
    {
        var orderNo = collection["MerchantTradeNo"].ToString();

        // 然後使用 Redirect 導回 Vue 的路由（這會變成 GET 請求，Angular 就能接收了）
        return Content($"<script>window.location.href='{fronturl}/mall';</script>", "text/html");
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
