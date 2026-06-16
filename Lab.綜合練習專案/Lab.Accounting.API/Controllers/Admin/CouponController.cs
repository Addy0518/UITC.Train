using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Controllers.Admin;

[Tags("Admin-Coupon")]
[Route("api/admin/[controller]/[action]")]
[ApiController]
[Authorize(Roles = RolesAuth.管理者)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class CouponController(ICouponService couponService) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 查看所有優惠卷
    /// </summary>
    /// <returns>優惠卷資訊列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<CouponResponse>>))]
    public async Task<IActionResult> GetAllCoupons([FromQuery] CouponSearchRequest request)
    {
        return Ok(await couponService.GetAllCoupons(request));
    }

    /// <summary>
    /// 新增優惠卷
    /// </summary>
    /// <param name="request">優惠卷新增請求</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> CreateCoupons([FromBody] CouponInsertRequest request)
    {
        request.CreaterId = CurrentUserId;
        return Ok(await couponService.CreateCoupons(request));
    }

    /// <summary>
    /// 管理員編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> AdminUpdateCoupons([FromBody] CouponUpdateRequest request)
    {
        return Ok(await couponService.AdminUpdateCoupons(request));
    }
}
