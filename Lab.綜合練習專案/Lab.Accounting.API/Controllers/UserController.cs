using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ApiResponse<UserResponse>> Register(UserRegisterRequest registerRequest)
        {
            return null;
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="loginRequest">使用者登入資訊</param>
        /// <returns>登入成功</returns>
        public async Task<ApiResponse<UserResponse>> Login(UserLoginRequest loginRequest)
        {
            return null;
        }
    }
}
