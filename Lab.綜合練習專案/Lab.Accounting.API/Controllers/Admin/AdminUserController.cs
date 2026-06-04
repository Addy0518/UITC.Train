namespace Lab.Accounting.API.Controllers.Admin;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = RolesAuth.管理者)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class AdminUserController(IUserService userserivce) : ControllerBase
{
    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <returns>使用者資訊列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserResponse>>))]
    public async Task<IActionResult> GetAllUser()
    {
        return Ok(await userserivce.GetAllUser());
    }

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserResponse>>))]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        return Ok(await userserivce.DeleteUser(userId));
    }
}
