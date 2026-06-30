using Lab.Accounting.API.Common.Requests.Category;

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
                  UserName, UserAccount, UserPassword, UserPhone,UserAddress,UserRegisterMethod,CreateTime,UpdateTime,IsDelete
                ) 
                values 
                  (
                    @UserName, @UserAccount, @UserPassword, 
                    @UserPhone,@UserAddress,@UserRegisterMethod,GetDate(),GetDate(),@IsDelete
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
    /// Google 登入註冊
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="password">密碼</param>
    /// <param name="userName">使用者名稱</param>
    /// <param name="pic">使用者 Google 頭貼照片</param>
    /// <returns>使用者 ID</returns>
    public async Task<int> GoogleUserLogin(string email, string password, string userName, string pic)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"Insert Into [User] (
                  UserName, UserAccount, UserPassword,UserHeadshot,UserRegisterMethod,CreateTime,UpdateTime,IsDelete
                ) 
                values 
                  (
                    @UserName, @UserAccount, @UserPassword,@UserHeadshot,@UserRegisterMethod,GetDate(),GetDate(),@IsDelete
                  );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

        return await conn.ExecuteScalarAsync<int>(
            sql,
            new
            {
                UserName = userName,
                UserAccount = email,
                UserPassword = password,
                UserHeadshot = pic,
                UserRegisterMethod = (int)RegisterMethodEnum.Google登入,
                IsDelete = 0,
            }
        );
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
    /// 依據帳號取得使用者資訊
    /// </summary>
    /// <param name="userAccount">使用者帳號</param>
    /// <returns>使用者資訊</returns>
    public async Task<UserResponse> GetUserByAccount(string userAccount)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select UserAccount,UserId,UserName,UserRegisterMethod  From [User]
              Where  UserAccount = @UserAccount";

        return await conn.QueryFirstOrDefaultAsync<UserResponse>(sql, new { UserAccount = userAccount });
    }

    /// <summary>
    /// 取得使用者詳細資訊 ( 管理員 )
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者詳細資訊</returns>
    public async Task<UserResponse> GetUserDetails(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT       u.useraccount,
                           u.userid,
                           u.username,
                           u.userAddress,
                           u.userBirthDate,
                           u.userPhone,
                           u.userheadshot,
                           u.usergender,
                           u.userrole,
                           u.createtime,
                           u.updateTime,
                           u.isdelete,
                           u.DeleteAdminId,
                           u.DeleteReason,

                    -- 統計資料
                    Count(DISTINCT o.OrderId) as TotalOrders,
                    Count(DISTINCT p.ProductsId) as TotalProducts,
                    IsNull(Sum(o.AccountAmount),0) as TotalSpent

                    FROM   [user] u
                    Left Join [Order] o on o.UserId = u.UserId
                    Left Join Product p on p.UserId = u.UserId
                    Where u.UserId = @UserId
                    GROUP BY u.UserAccount,
                             u.UserId,
                             u.UserName,
                             u.UserAddress,
                             u.UserBirthDate,
                             u.UserPhone,
                             u.UserHeadShot,
                             u.UserGender,
                             u.UserRole,
                             u.CreateTime,
                             u.UpdateTime,
                             u.IsDelete,
                             u.DeleteAdminId,
                             u.DeleteReason";

        return await conn.QueryFirstOrDefaultAsync<UserResponse>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <param name="request">搜尋使用者請求 </param>
    /// <returns>使用者資訊列表</returns>
    public async Task<IEnumerable<UserResponse>> GetAllUser(UserSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();
        int offset = request.pageIndex * request.pageSize;
        var sql =
            @"SELECT useraccount,
                           userid,
                           username,
                           userheadshot,
                           usergender,
                           userrole,
                           createtime,
                           isdelete,
                           Count(*) OVER() AS TotalCount
                    FROM   [user] 
                    Where 
                           (@IsDelete is null or IsDelete=@IsDelete)
                    and    (@UserGender is null or UserGender=@UserGender)
                    and    (@UserRole is null or UserRole=@UserRole)
                    and    (@keyWords is null 
                    or     UserName like '%' + @keyWords + '%')
                    

                    Order by 
                    case when @sortBy='CreateTime' and @sortOrder='asc' then CreateTime end asc,
                    case when @sortBy='CreateTime' and @sortOrder='desc' then CreateTime end desc,
                    userId
                    offset @offset rows FETCH next @pageSize rows only";

        return await conn.QueryAsync<UserResponse>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                keyWords = request.keyWords,
                sortBy = request.sortBy,
                sortOrder = request.sortOrder,
                IsDelete = request.IsDelete,
                UserGender = request.UserGender,
                UserRole = request.UserRole,
            }
        );
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
            @"Select UserAccount,UserPassword From [User]
                  WHERE UserId = @UserId";
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 根據 ID 更新使用者密碼
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="NewUserPassword">新密碼</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdatePasswordById(int userId, string NewUserPassword)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE [User]
                  SET
                  UserPassword = @NewUserPassword,
                  UpdateTime    = GetDate()
                  WHERE UserId = @UserId";

        return await conn.ExecuteAsync(sql, new { UserId = userId, NewUserPassword = NewUserPassword });
    }

    /// <summary>
    /// 根據帳號更新使用者密碼
    /// </summary>
    /// <param name="userAccount">使用者帳號</param>
    /// <param name="NewUserPassword">新密碼</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdatePasswordByAccount(string userAccount, string NewUserPassword)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE [User]
                  SET
                  UserPassword = @NewUserPassword,
                  UpdateTime    = GetDate()
                  WHERE UserAccount = @UserAccount";

        return await conn.ExecuteAsync(sql, new { UserAccount = userAccount, NewUserPassword = NewUserPassword });
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

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="adminId">管理員 ID</param>
    /// <param name="deleteReason">停用原因</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteUser(int userId, int adminId, string deleteReason)
    {
        using var conn = connecting.CreateConnecting();

        var deletesql =
            @"Update [User] 
                          Set 
                                 IsDelete=1,
                                 DeleteAdminId=@DeleteAdminId,
                                 DeleteReason=@DeleteReason,
                                 UpdateTime=GetDate()
                          Where UserId=@UserId;";

        return await conn.ExecuteAsync(
            deletesql,
            new
            {
                UserId = userId,
                DeleteAdminId = adminId,
                DeleteReason = deleteReason,
            }
        );
    }

    /// <summary>
    /// 復原已選取的用戶刪除狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateUserDeleteStatus(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE [User]
                        SET     IsDelete = 0 ,
                                DeleteAdminId=NULL,
                                DeleteReason=NULL,
                                UpdateTime   = GetDate()
                        WHERE   UserId = @UserId";
        return await conn.ExecuteAsync(sql, new { UserId = userId });
    }
}
