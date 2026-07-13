using Google.Apis.Auth;
using Lab.Accounting.API.Common.Requests.Category;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;

namespace Lab.Accounting.API.Services;

public class UserService(
    IUserRepository userrepo,
    TokenHelper tokenHelper,
    PasswordSecureHelper passwordSecureHelper,
    SendEmailHelper sendEmailHelper,
    VerifyCodeHelper verifyCodelHelper,
    ITokenBlacklistRepository tokenBlacklistRepositories,
    IOptions<GoogleAuthSetting> googleAuthOptions,
    IWebHostEnvironment env
) : IUserService
{
    private readonly GoogleAuthSetting googleAuthSettings = googleAuthOptions.Value;

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
            UserAddress = registerRequest.UserAddress,
            UserZipCode = registerRequest.UserZipCode,
            UserRegisterMethod = (int)RegisterMethodEnum.本網站註冊,
            IsDelete = (int)IsDeleteStatusEnum.Normal,
        };
        var exist = await userrepo.ExistRegister(user);

        if (exist == true)
        {
            var errors = new Dictionary<string, string[]> { { "UserAccount", new[] { "該帳號已被註冊!" } } };

            return ApiResponseHelper.RequestError<UserResponse>(errors);
        }

        var result = await userrepo.Register(user);

        if (result == null)
            return ApiResponseHelper.InternalException<UserResponse>();

        var userresult = new UserResponse { UserId = result.UserId, UserName = result.UserName };

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

        bool isValid = passwordSecureHelper.VerifyPassword(loginRequest.UserPassword, dbuser.UserPassword);

        if (isValid == false)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "UserPassword", new[] { "密碼驗證失敗 , 請重新輸入!" } },
            };

            return ApiResponseHelper.RequestError<UserResponse>(errors);
        }

        dbuser.UserPassword = null;

        string? role = null;
        if (!string.IsNullOrEmpty(dbuser.UserRole))
        {
            role = dbuser.UserRole;
        }

        var token = tokenHelper.GeneratedToken(dbuser.UserId, dbuser.UserName, role, dbuser.UserAddress ?? "");

        var userheadshot = await userrepo.GetUser(dbuser.UserId);

        var userresponse = new UserResponse
        {
            Token = token,
            UserId = dbuser.UserId,
            UserName = dbuser.UserName,
            UserHeadshot = userheadshot?.UserHeadshot,
            UserRole = dbuser.UserRole,
            UserPhone = dbuser.UserPhone,
            UserAddress = dbuser?.UserAddress,
            UserZipCode = dbuser?.UserZipCode,
        };

        return ApiResponseHelper.Success(userresponse, "成功");
    }

    /// <summary>
    /// Google 第三方登入
    /// </summary>
    /// <param name="request">Google Id_Token</param>
    /// <returns>登入成功</returns>
    public async Task<ApiResponse<UserResponse>> GoogleLogin(GoogleLoginRequest request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { googleAuthSettings.ClientId },
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch
        {
            var errors = new Dictionary<string, string[]> { { "IdToken", new[] { "Google 驗證失敗，請重新登入!" } } };
            return ApiResponseHelper.RequestError<UserResponse>(errors);
        }

        // 這裡都是 Google 帳號自帶的資訊
        var email = payload.Email;
        var googleName = payload.Name;
        var pic = payload.Picture;

        var dbUser = await userrepo.GetUserByAccount(email);

        // 第一次用 Google 登入，自動建立帳號
        if (dbUser == null)
        {
            var password = passwordSecureHelper.HashPassword(Guid.NewGuid().ToString());
            await userrepo.GoogleUserLogin(email, password, googleName, pic, dbUser.UserRole = null);
            dbUser = await userrepo.GetUserByAccount(email);
        }
        else if (dbUser.UserRegisterMethod != RegisterMethodEnum.Google登入)
        {
            // 這個 email 已經存在，但是是用「一般帳密」註冊的
            // 不能直接讓 Google 登入直接進去這個帳號！
            var errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "此信箱已使用一般帳號註冊，請改用帳號密碼登入!" } },
            };
            return ApiResponseHelper.RequestError<UserResponse>(errors);
        }
        else
        {
            // 判斷本來有沒有頭貼 , 隨時同步跟 Google 一樣的頭貼圖片
            // 不用存實體檔案 , google 頭貼是用公開網址
            if (dbUser.UserHeadshot != pic)
            {
                await userrepo.UserHeadShotUpload(pic, dbUser.UserId);

                dbUser.UserHeadshot = pic;
            }
        }

        // 發 JWT Token，後續流程跟一般登入完全一樣
        var token = tokenHelper.GeneratedToken(
            dbUser.UserId,
            dbUser.UserName,
            dbUser.UserRole,
            dbUser.UserAddress ?? ""
        );

        var userheadshot = await userrepo.GetUser(dbUser.UserId);

        var userresponse = new UserResponse
        {
            Token = token,
            UserId = dbUser.UserId,
            UserName = dbUser.UserName,
            UserHeadshot = userheadshot?.UserHeadshot,
            UserRole = dbUser.UserRole,
            UserAddress = dbUser?.UserAddress,
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
            var errors = new Dictionary<string, string[]> { { "Token", new[] { "無效的 Token !" } } };

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
    public async Task<ApiResponse<UserResponse>> UserHeadShotUpload(IFormFile userFile, int userId)
    {
        var target = await userrepo.GetUser(userId);
        if (target == null)
        {
            return ApiResponseHelper.NotFound<UserResponse>();
        }
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

    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者資訊</returns>
    public async Task<ApiResponse<UserResponse>> GetUser(int userId)
    {
        var result = await userrepo.GetUser(userId);
        if (result == null)
        {
            return ApiResponseHelper.NotFound<UserResponse>();
        }

        return ApiResponseHelper.Success<UserResponse>(result);
    }

    /// <summary>
    /// 取得使用者詳細資訊 ( 管理員 )
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者詳細資訊</returns>
    public async Task<ApiResponse<UserResponse>> GetUserDetails(int userId)
    {
        var result = await userrepo.GetUserDetails(userId);

        if (result == null)
        {
            return ApiResponseHelper.NotFound<UserResponse>();
        }

        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <param name="request">搜尋使用者請求 </param>
    /// <returns>使用者資訊列表</returns>
    public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUser(UserSearchRequest request)
    {
        var result = await userrepo.GetAllUser(request);

        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 編輯使用者資訊
    /// </summary>
    /// <param name="request">使用者更新資訊</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> UpdateUser(UserUpdateRequest request)
    {
        if (request.UserId <= 0)
        {
            return ApiResponseHelper.NotFound<int>();
        }

        var result = await userrepo.UpdateUser(request);
        if (result <= 0)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "UpdateUser", new[] { "更新失敗 , 請確認資料後重新送出!" } },
            };

            return ApiResponseHelper.RequestError<int>(errors);
        }
        return ApiResponseHelper.Success<int>(result);
    }

    /// <summary>
    /// 更新使用者密碼 ( 已登入 )
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<string>> UpdatePassword(UserUpdatePasswordRequest request)
    {
        var dbuser = await userrepo.GetUserPassword(request.UserId);
        if (dbuser == null)
        {
            return ApiResponseHelper.NotFound<string>();
        }

        bool isValid = passwordSecureHelper.VerifyPassword(request.OldUserPassword, dbuser.UserPassword);

        if (isValid == false)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "UserPassword", new[] { "密碼驗證失敗 , 請重新輸入!" } },
            };

            return ApiResponseHelper.RequestError<string>(errors);
        }

        dbuser.UserPassword = null;
        request.NewUserPassword = passwordSecureHelper.HashPassword(request.NewUserPassword);
        await userrepo.UpdatePasswordById(request.UserId, request.NewUserPassword);

        return ApiResponseHelper.Success<string>("更新成功 !");
    }

    /// <summary>
    /// 寄送忘記密碼的驗證碼
    /// </summary>
    /// <param name="request">使用者帳號</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<string>> SendVerfiyCode(SendVerifyCodeRequest request)
    {
        var dbuser = await userrepo.GetUserByAccount(request.UserAccount);

        if (dbuser == null)
        {
            return ApiResponseHelper.NotFound<string>();
        }

        // 生成六位數隨機碼當驗證碼
        var code = Random.Shared.Next(100000, 999999).ToString();

        // 儲存驗證碼到快取 , 設定過期時間
        verifyCodelHelper.SetCode(request.UserAccount, code, TimeSpan.FromMinutes(10));

        // 生成隨機驗證碼,然後寄給使用者
        await sendEmailHelper.SendEmail(dbuser.UserAccount, code);

        return ApiResponseHelper.Success<string>("驗證碼已寄送至您的信箱,請注意查收 !");
    }

    /// <summary>
    /// 更新使用者密碼 ( 忘記密碼 )
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<string>> ForgetUpdatePassword(UserForgetPasswordRequest request)
    {
        if (!verifyCodelHelper.TryGetCode(request.UserAccount, out var code) || code != request.code)
        {
            var errors = new Dictionary<string, string[]> { { "Code", new[] { "驗證碼錯誤或已過期 , 請重新申請 !" } } };

            return ApiResponseHelper.RequestError<string>(errors);
        }

        request.NewUserPassword = passwordSecureHelper.HashPassword(request.NewUserPassword);
        await userrepo.UpdatePasswordByAccount(request.UserAccount, request.NewUserPassword);

        verifyCodelHelper.RemoveCode(request.UserAccount);

        return ApiResponseHelper.Success<string>("密碼設定成功 !");
    }

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="adminId">管理員 ID</param>
    /// <param name="deleteReason">停用原因</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> DeleteUser(int userId, int adminId, string deleteReason)
    {
        var target = await userrepo.GetUser(userId);
        if (target == null)
        {
            var errors = new Dictionary<string, string[]> { { "User", new[] { "查無用戶" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }
        var delete = await userrepo.DeleteUser(userId, adminId, deleteReason);

        if (delete <= 0)
            return ApiResponseHelper.InternalException<int>("刪除失敗 ! ");
        return ApiResponseHelper.Success<int>(delete);
    }

    /// <summary>
    /// 復原已選取的用戶刪除狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> UpdateUserDeleteStatus(int userId)
    {
        var target = await userrepo.GetUser(userId);
        if (target == null)
        {
            var errors = new Dictionary<string, string[]> { { "User", new[] { "查無用戶" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }
        var delete = await userrepo.UpdateUserDeleteStatus(userId);

        if (delete <= 0)
            return ApiResponseHelper.InternalException<int>("復原失敗 ! ");
        return ApiResponseHelper.Success<int>(delete);
    }
}
