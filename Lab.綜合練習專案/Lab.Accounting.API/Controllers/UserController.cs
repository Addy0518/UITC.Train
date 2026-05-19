namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class UserController(IUserService userserivce) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="registerRequest">使用者註冊資訊</param>
    /// <returns>註冊成功</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest registerRequest)
    {
        return Ok(await userserivce.Register(registerRequest));
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="loginRequest">使用者登入資訊</param>
    /// <returns>登入成功</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
    {
        return Ok(await userserivce.Login(loginRequest));
    }

    /// <summary>
    /// 使用者登出
    /// </summary>
    /// <returns>是否成功登出</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> Logout()
    {
        var Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        return Ok(await userserivce.Logout(Token));
    }

    /// <summary>
    /// 使用者大頭照上傳
    /// </summary>
    /// <param name="userFile">使用者大頭照檔案 </param>
    /// <returns>使用者資訊</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> UserHeadShotUpload(IFormFile userFile)
    {
        return Ok(await userserivce.UserHeadShotUpload(userFile, CurrentUserId));
    }

    /// <summary>
    /// 取得登入者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> GetUser()
    {
        return Ok(await userserivce.GetUser(CurrentUserId));
    }

    /// <summary>
    /// 取得指定使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserResponse>))]
    public async Task<IActionResult> GetOneUser([FromQuery] int userId)
    {
        return Ok(await userserivce.GetUser(userId));
    }

    /// <summary>
    /// 編輯使用者資訊
    /// </summary>
    /// <param name="request">使用者更新資訊</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await userserivce.UpdateUser(request));
    }

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> UpdatePassword([FromBody] UserUpdatePasswordRequest request)
    {
        request.UserId = CurrentUserId;
        return Ok(await userserivce.UpdatePassword(request));
    }
}
