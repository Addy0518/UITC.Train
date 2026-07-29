using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
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
    /// 賣場升級成公司帳號
    /// </summary>
    /// <param name="request">公司資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> StoreUpdateToCompany([FromForm] StoreUpdateToCompanyRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await sellerservice.StoreUpdateToCompany(request));
    }

    /// <summary>
    /// 編輯賣場資訊
    /// </summary>
    /// <param name="request">編輯資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateStore([FromBody] StoreUpdateRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await sellerservice.UpdateStore(request));
    }

    /// <summary>
    /// 用戶追蹤賣場
    /// </summary>
    /// <param name="storeId">賣場 ID</param>
    /// <returns>影響列數</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> FollowStore([FromQuery] int storeId)
    {
        return Ok(await sellerservice.FollowStore(CurrentUserId, storeId));
    }

    /// <summary>
    /// 用戶取消追蹤賣場
    /// </summary>
    /// <param name="storeId">賣場 ID</param>
    /// <returns>影響列數</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UnfollowStore([FromQuery] int storeId)
    {
        return Ok(await sellerservice.UnfollowStore(CurrentUserId, storeId));
    }

    /// <summary>
    /// 查看用戶是否已追蹤某賣場
    /// </summary>
    /// <param name="storeId">賣場 ID</param>
    /// <returns>是否已追蹤</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> IsFollowingStore([FromQuery] int storeId)
    {
        return Ok(await sellerservice.IsFollowingStore(CurrentUserId, storeId));
    }
}
