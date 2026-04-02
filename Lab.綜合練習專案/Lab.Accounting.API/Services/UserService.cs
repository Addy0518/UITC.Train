using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Serilog.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab.Accounting.API.Services
{
    public class UserService(
        IUserRepositories userrepo,
        TokenHelper tokenHelper,
        PasswordSecureHelper passwordSecureHelper,
        ILogger<UserService> logger
    ) : IUserService
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="registerRequest">使用者註冊資訊</param>
        /// <returns>註冊成功</returns>
        public async Task<ApiResponse<UserResponse>> Register(UserRegisterRequest registerRequest)
        {
            var user = new User
            {
                UserName = registerRequest.UserName,
                UserAccount = registerRequest.UserAccount,
                UserPhone = registerRequest.UserPhone,
                UserPassword = passwordSecureHelper.HashPassword(registerRequest.UserPassword),
            };
            var exist = await userrepo.ExistRegister(user);

            if (exist == true)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "UserAccount", new[] { "該帳號已被註冊!" } },
                };

                return ApiResponseHelper.RequestError<UserResponse>(errors);
            }

            var result = await userrepo.Register(user);

            var userresult = new UserResponse
            {
                UserId = result.UserId,
                UserName = result.UserName,
            };

            return ApiResponseHelper.Success<UserResponse>(userresult, "成功!");
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="loginRequest">使用者登入資訊</param>
        /// <returns>登入成功</returns>
        public async Task<ApiResponse<UserResponse>> Login(UserLoginRequest loginRequest)
        {
            var user = new User
            {
                UserAccount = loginRequest.UserAccount,
                UserPassword = loginRequest.UserPassword,
            };
            // 記錄資料
            logger.LogInformation("使用者帳號{account}", user.UserAccount);

            var dbuser = await userrepo.Login(user);

            if (dbuser == null)
            {
                return ApiResponseHelper.NotFound<UserResponse>();
            }

            var token = tokenHelper.GeneratedToken(dbuser.UserId, dbuser.UserName);

            dbuser.Token = token;
            return ApiResponseHelper.Success(dbuser, "成功");
        }
    }
}
