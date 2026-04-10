using System.IdentityModel.Tokens.Jwt;
using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Microsoft.AspNetCore.Identity.Data;
using Serilog.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab.Accounting.API.Services
{
    public class UserService(
        IUserRepositories userrepo,
        TokenHelper tokenHelper,
        PasswordSecureHelper passwordSecureHelper,
        ITokenBlacklistRepositories tokenBlacklistRepositories,
        IWebHostEnvironment env
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
            var user = new User { UserAccount = loginRequest.UserAccount };

            var dbuser = await userrepo.Login(user);

            if (dbuser == null)
            {
                return ApiResponseHelper.NotFound<UserResponse>();
            }

            bool isValid = passwordSecureHelper.VerifyPassword(
                loginRequest.UserPassword,
                dbuser.UserPassword
            );

            if (isValid == false)
                return ApiResponseHelper.NotFound<UserResponse>();

            dbuser.UserPassword = null;

            var token = tokenHelper.GeneratedToken(dbuser.UserId, dbuser.UserName);

            var userresponse = new UserResponse
            {
                Token = token,
                UserId = dbuser.UserId,
                UserName = dbuser.UserName,
            };

            return ApiResponseHelper.Success(userresponse, "成功");
        }

        /// <summary>
        /// 使用者登出
        /// </summary>
        /// <param name="Token">登出的 Token</param>
        /// <returns>是否成功登出</returns>
        public async Task<ApiResponse<string>> Logout(string Token)
        {
            // 新增 JwtSecurityTokenHandler 物件
            var tokenHandler = new JwtSecurityTokenHandler();

            // 如果解析不了就退回
            if (!tokenHandler.CanReadToken(Token))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Token", new[] { "無效的 Token !" } },
                };

                return ApiResponseHelper.RequestError<string>(errors);
            }

            // 解析 JWT Token，拿到 Jti 和過期時間 ( 這裡只是拆開解析 , 還沒驗證 )
            var jwt = tokenHandler.ReadJwtToken(Token);

            // Id 是 Guid 字串
            var jit = jwt.Id;

            // ValidTo 則是 JWT　標準名稱　exp (expiration) 過期時間的值
            var expiresAt = jwt.ValidTo;

            // 先檢查有沒有登出過了 , 有登出過就會在黑名單
            if (await tokenBlacklistRepositories.isBlackList(jit))
            {
                return ApiResponseHelper.Success<string>("已登出");
            }

            // 沒有就登出並加入黑名單
            await tokenBlacklistRepositories.AddToken(jit, expiresAt);

            return ApiResponseHelper.Success<string>("登出成功,以新增至黑名單");
        }

        /// <summary>
        /// 使用者大頭照上傳
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <param name="userFile">使用者大頭照檔案 </param>
        /// <returns>使用者資訊</returns>
        public async Task<ApiResponse<UserResponse>> UserHeadShotUpload(
            IFormFile userFile,
            int userId
        )
        {
            var target = await userrepo.GetUser(userId);
            var existFile = await ExistFile(userFile, target.UserHeadshot, "UserHeadShot");

            var result = await userrepo.UserHeadShotUpload(existFile, userId);
            var lastresult = await userrepo.GetUser(userId);
            return ApiResponseHelper.Success<UserResponse>(lastresult, "成功");
        }

        /// <summary>
        /// 私有方法判斷文件是否存在
        /// </summary>
        /// <param name="newFile">新的檔案</param>
        /// <param name="oldPath">舊的檔案路徑</param>
        /// <param name="folder">檔案存放的資料夾</param>
        /// <returns>檔案路徑</returns>
        private async Task<string?> ExistFile(IFormFile? newFile, string? oldPath, string folder)
        {
            //沒更新就回傳舊檔案路徑
            if (newFile == null)
                return oldPath;

            //更新的話刪除舊檔案
            if (!string.IsNullOrEmpty(oldPath))
            {
                FileUploadHelper.DeleteFile(env.WebRootPath, folder, oldPath);
            }
            //不管怎樣都要儲存檔案
            return await FileUploadHelper.SaveFileAsync(newFile, env.WebRootPath, folder);
        }
    }
}
