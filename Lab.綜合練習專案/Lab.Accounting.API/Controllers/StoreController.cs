using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = RolesAuth.賣家)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class StoreController(IStoreService sellerservice) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 取得賣場資訊
    /// </summary>
    /// <param name="sellerId">賣家 ID </param>
    /// <returns>賣家資訊</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<StoreResponse>))]
    public async Task<IActionResult> GetStore([FromQuery] int sellerId)
    {
        return Ok(await sellerservice.GetStore(sellerId));
    }

    /// <summary>
    /// 賣場註冊
    /// </summary>
    /// <param name="request">賣家註冊資訊</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> Register([FromBody] StoreRegisterRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await sellerservice.StoreRegister(request));
    }

    /// <summary>
    /// 編輯賣場資訊
    /// </summary>
    /// <param name="request">編輯資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateSeller(StoreUpdateRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await sellerservice.UpdateStore(request));
    }
}
