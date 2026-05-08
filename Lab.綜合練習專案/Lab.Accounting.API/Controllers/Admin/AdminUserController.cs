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
    [Authorize(Roles = RolesAuth.管理者)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserResponse>>))]
    public async Task<IActionResult> GetAllUser()
    {
        return Ok(await userserivce.GetAllUser());
    }
}
