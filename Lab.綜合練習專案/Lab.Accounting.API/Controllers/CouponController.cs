using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class CouponController(ICouponService couponService) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 查看優惠卷
    /// </summary>
    /// <param name="couponId">優惠卷 ID </param>
    /// <returns>優惠卷資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CouponResponse>))]
    public async Task<IActionResult> GetCoupon([FromQuery] int couponId)
    {
        return Ok(await couponService.GetCoupon(couponId));
    }

    /// <summary>
    /// 查看用戶優惠卷
    /// </summary>
    /// <returns>優惠卷資訊列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<CouponResponse>>))]
    public async Task<IActionResult> GetUserCoupon()
    {
        return Ok(await couponService.GetUserCoupon(CurrentUserId));
    }

    /// <summary>
    /// 查看用戶可領取的優惠卷
    /// </summary>
    /// <returns>可領取優惠卷資訊列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<CouponResponse>>))]
    public async Task<IActionResult> GetCanReceiveCoupon()
    {
        return Ok(await couponService.GetCanReceiveCoupon(CurrentUserId));
    }

    /// <summary>
    /// 賣家查看所有優惠卷
    /// </summary>
    /// <returns>優惠卷資訊列表</returns>
    [HttpGet]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<CouponResponse>>))]
    public async Task<IActionResult> GetSellerAllCoupons([FromQuery] CouponSearchRequest request)
    {
        request.CreaterId = CurrentUserId;
        return Ok(await couponService.GetAllCoupons(request));
    }

    /// <summary>
    /// 賣家新增優惠卷
    /// </summary>
    /// <param name="request">優惠卷新增請求</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> SellerCreateCoupons([FromBody] CouponInsertRequest request)
    {
        request.CreaterId = CurrentUserId;
        return Ok(await couponService.CreateCoupons(request));
    }

    /// <summary>
    /// 賣家編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> SellerUpdateCoupons([FromBody] CouponUpdateRequest request)
    {
        request.CreaterId = CurrentUserId;
        return Ok(await couponService.SellerUpdateCoupons(request));
    }

    /// <summary>
    /// 用戶領取優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>優惠卷 ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> CreateUserCoupon([FromBody] UserCouponInsertRequest request)
    {
        request.UserId = CurrentUserId;
        request.CreateTime = DateTime.Now;
        return Ok(await couponService.CreateUserCoupon(request));
    }
}
