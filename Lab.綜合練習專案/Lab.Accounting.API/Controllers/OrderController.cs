using Microsoft.AspNetCore.Http.HttpResults;
using UBOT_Domain.Models.Constants;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class OrderController(IOrderService orderService) : ControllerBase
{
    // 公開網址基底給綠界呼叫
    private string tuuneUrl = "https://veneering-bannister-outlook.ngrok-free.dev";

    // 前端網址基底
    private string fronturl = "http://localhost:5174";

    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 查看使用者購買紀錄
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

        int orderId = target.ReturnData;

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

        // 然後使用 Redirect 導回 Vue 的路由（這會變成 GET 請求，Angular 就能接收了）
        return Content($"<script>window.location.href='{fronturl}/mall';</script>", "text/html");
    }
}
