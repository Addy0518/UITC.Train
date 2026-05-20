namespace Lab.Accounting.API.Repositories;

public class UserRepository(DBConnecting connecting) : IUserRepository
{
    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="userInformation">使用者註冊資訊</param>
    /// <returns>使用者資訊</returns>
    public async Task<UserResponse> Register(User userInformation)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"Insert Into [User] (
                  UserName, UserAccount, UserPassword, UserPhone,UserAddress,CreateTime,UpdateTime,IsDelete
                ) 
                values 
                  (
                    @UserName, @UserAccount, @UserPassword, 
                    @UserPhone,@UserAddress,GetDate(),GetDate(),@IsDelete
                  );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

        return await conn.QuerySingleAsync<UserResponse>(sql, userInformation);
    }

    /// <summary>
    /// 檢查使用者是否註冊過
    /// </summary>
    /// <param name="userInformation">使用者註冊資訊</param>
    /// <returns>是否註冊過</returns>
    public async Task<bool> ExistRegister(User userInformation)
    {
        using var conn = connecting.CreateConnecting();
        var sql = @"Select Top 1 1 From [User] Where @UserAccount=UserAccount";

        var result = await conn.ExecuteScalarAsync<int?>(sql, userInformation);
        // 有值就存在 , 沒值就不存在
        return result.HasValue;
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="userInformation">使用者登入資訊</param>
    /// <returns>使用者資訊</returns>
    public async Task<User> Login(User userInformation)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select UserId,UserName,UserRole,UserPassword,UserAddress From [User] Where UserAccount=@UserAccount";

        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { UserAccount = userInformation.UserAccount });
    }

    /// <summary>
    /// 使用者大頭照上傳
    /// </summary>
    /// <param name="userHeadShot">使用者大頭照</param>
    /// <param name="userId">使用者 ID </param>
    /// <returns>影響列數</returns>
    public async Task<int> UserHeadShotUpload(string userHeadShot, int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Update 
                  [User] 
                Set 
                  UserHeadShot = COALESCE(@UserHeadShot, UserHeadShot),
                  UpdateTime    = GetDate()
                where 
                  UserId = @UserId";

        return await conn.ExecuteAsync(sql, new { UserHeadShot = userHeadShot, UserId = userId });
    }

    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者資訊</returns>
    public async Task<UserResponse> GetUser(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select UserAccount,UserId,UserName,UserHeadShot,UserPhone,UserBirthDate,UserGender,UserAddress,UserRole From [User]
                where 
                  UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<UserResponse>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <returns>使用者資訊列表</returns>
    public async Task<IEnumerable<UserResponse>> GetAllUser()
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select UserId,UserName,UserAccount,UserPhone,UserHeadShot,UserRole,UserAddress From [User]
             ";

        return await conn.QueryAsync<UserResponse>(sql);
    }

    /// <summary>
    /// 編輯使用者資訊
    /// </summary>
    /// <param name="request">使用者更新資訊</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateUser(UserUpdateRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE [User]
                  SET
                       UserName      = COALESCE(@UserName, UserName),
                       UserAddress   = COALESCE(@UserAddress, UserAddress),
                       UserPhone     = COALESCE(@UserPhone, UserPhone),
                       UserBirthDate = COALESCE(@UserBirthDate, UserBirthDate),
                       UserGender    = COALESCE(@UserGender, UserGender),
                       UpdateTime    = GetDate()
                  WHERE UserId = @UserId";

        return await conn.ExecuteAsync(sql, request);
    }

    /// <summary>
    /// 查看使用者密碼
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>影響列數</returns>
    public async Task<User> GetUserPassword(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select UserPassword From [User]
                  WHERE UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdatePassword(UserUpdatePasswordRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE [User]
                  SET
                  UserPassword = @NewUserPassword,
                  UpdateTime    = GetDate()
                  WHERE UserId = @UserId";

        return await conn.ExecuteAsync(sql, request);
    }

    /// <summary>
    /// 改變權限變賣家
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <param name="userRole">使用者權限 </param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateRole(int userId, string userRole)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Update [User] Set 
                    UserRole    = COALESCE(@UserRole, UserRole),
                    UpdateTime  = COALESCE(@UpdateTime, UpdateTime)
                  WHERE UserId = @UserId";
        return await conn.ExecuteAsync(
            sql,
            new
            {
                UserId = userId,
                UserRole = userRole,
                UpdateTime = DateTime.Now,
            }
        );
    }
}
