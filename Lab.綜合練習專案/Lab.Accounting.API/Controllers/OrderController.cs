using Microsoft.AspNetCore.Http.HttpResults;
using UBOT_Domain.Models.Constants;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class OrderController(IOrderService orderService, IConfiguration config) : ControllerBase
{
    // 公開網址基底給綠界呼叫
    private string tuuneUrl = config["TuuneUrl"];

    // 前端網址基底
    private string fronturl = config["FrontendUrl"];

    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 買家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <returns>訂單資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse>))]
    public async Task<IActionResult> GetUserOneOrder([FromQuery] int orderId)
    {
        var target = await orderService.GetUserOneOrder(orderId, CurrentUserId);
        return Ok(target);
    }

    /// <summary>
    /// 買家查看所有訂單
    /// </summary>
    /// <returns>訂單 ID</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<OrderResponse>>))]
    public async Task<IActionResult> GetUserOrder()
    {
        var target = await orderService.GetUserOrder(CurrentUserId);
        return Ok(target);
    }

    /// <summary>
    /// 賣家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <returns>訂單資訊</returns>
    [HttpGet]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse>))]
    public async Task<IActionResult> GetSellerOneOrder([FromQuery] int orderId)
    {
        var target = await orderService.GetSellerOneOrder(orderId, CurrentUserId);
        return Ok(target);
    }

    /// <summary>
    /// 賣家查看所有訂單
    /// </summary>
    /// <returns>所有訂單資訊</returns>
    [HttpGet]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<OrderResponse>>))]
    public async Task<IActionResult> GetSellerOrder()
    {
        var target = await orderService.GetSellerOrder(CurrentUserId);
        return Ok(target);
    }

    /// <summary>
    /// 改變運輸狀態
    /// </summary>s
    /// <param name="orderId">訂單 ID</param>
    /// <returns>影響行數</returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateShippingStatus(
        [FromQuery] int orderId,
        [FromQuery] ShippingStatusEnum shippingStatus
    )
    {
        var target = await orderService.UpdateShippingStatus(orderId, shippingStatus);
        return Ok(target);
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
        var target = await orderService.UserBuyProduct(Request);

        if (target.CodeStatus != CodeStatusEnum.Success)
        {
            return Ok(target); // 直接回傳錯誤結果
        }

        List<int> orderId = target.ReturnData;

        var payment = await orderService.GetPaymentData(orderId, CurrentUserId, tuuneUrl);
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
        var result = await orderService.SetPaymentData(collection);

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
        var rtnCode = collection["RtnCode"].ToString(); // 1=成功, 其他=失敗

        // 使用 Redirect 導回 Vue 的路由（這會變成 GET 請求，Angular 就能接收了）
        if (rtnCode == "1")
        {
            return Content(
                $"<script>window.location.href='{fronturl}/user-centre/purchase-orders?orderNo={orderNo}&status=success';</script>",
                "text/html"
            );
        }
        else
        {
            return Content(
                $"<script>window.location.href='{fronturl}/user-centre/purchase-orders?status=fail';</script>",
                "text/html"
            );
        }
    }

    /// <summary>
    /// 綠界訂單創建( 重新付款 )
    /// </summary>
    /// <param name="orderIds">多筆訂單 ID </param>
    /// <returns>跳轉綠界訂單</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GreenPayResponse>))]
    public async Task<IActionResult> GetRetryPaymentData([FromBody] List<int> orderIds)
    {
        var result = await orderService.GetRetryPaymentData(orderIds, CurrentUserId, tuuneUrl);

        return Ok(result);
    }
}
