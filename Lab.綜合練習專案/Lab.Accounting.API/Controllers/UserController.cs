using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ApiResponse<ProblemDetails>)
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest,
        Type = typeof(ApiResponse<Dictionary<string, string[]>>)
    )]
    public class UserController(IUserService userserivce) : ControllerBase
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="registerRequest">使用者註冊資訊</param>
        /// <returns>註冊成功</returns>
        [HttpPost]
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
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            return Ok(await userserivce.Login(loginRequest));
        }

        /// <summary>
        /// 使用者登出
        /// </summary>
        /// <param name="Token">登出的 Token</param>
        /// <returns>是否成功登出</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            return Ok(await userserivce.Logout(Token));
        }
    }
}
