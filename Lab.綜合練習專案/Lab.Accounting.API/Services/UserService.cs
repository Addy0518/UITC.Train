using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;

namespace Lab.Accounting.API.Services
{
    public class UserService(IUserRepositories userrepo) : IUserService
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
                UserPassword = registerRequest.UserPassword,
            };

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
            return null;
        }
    }
}
