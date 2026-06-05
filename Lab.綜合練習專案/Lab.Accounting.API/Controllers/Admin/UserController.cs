namespace Lab.Accounting.API.Controllers.Admin;

[Tags("Admin-User")]
[Route("api/admin/[controller]/[action]")]
[ApiController]
[Authorize(Roles = RolesAuth.管理者)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class UserController(IUserService userserivce) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <param name="request">搜尋使用者請求 </param>
    /// <returns>使用者資訊列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserResponse>>))]
    public async Task<IActionResult> GetAllUser([FromQuery] UserSearchRequest request)
    {
        return Ok(await userserivce.GetAllUser(request));
    }

    /// <summary>
    /// 取得使用者詳細資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> GetUserDetails([FromQuery] int userId)
    {
        return Ok(await userserivce.GetUserDetails(userId));
    }

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="deleteReason">停用原因</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> DeleteUser([FromQuery] int userId, [FromQuery] string deleteReason)
    {
        return Ok(await userserivce.DeleteUser(userId, CurrentUserId, deleteReason));
    }

    /// <summary>
    /// 復原已選取的用戶刪除狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateUserDeleteStatus([FromQuery] int userId)
    {
        return Ok(await userserivce.UpdateUserDeleteStatus(userId));
    }
}
