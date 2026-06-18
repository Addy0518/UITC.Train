using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Repositories;

public class CouponRepository(DBConnecting connecting) : ICouponRepository
{
    /// <summary>
    /// 查看優惠卷
    /// </summary>
    /// <param name="couponId">優惠卷 ID </param>
    /// <returns>優惠卷資訊</returns>
    public async Task<CouponResponse> GetCoupon(int couponId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select * From Coupon 
              WHERE CouponId = @CouponId";
        return await conn.QueryFirstOrDefaultAsync<CouponResponse>(sql, new { CouponId = couponId });
    }

    /// <summary>
    /// 查看用戶優惠卷
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>優惠卷資訊列表</returns>
    public async Task<IEnumerable<CouponResponse>> GetUserCoupon(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select c.*,uc.UserCouponId,uc.CreateTime,uc.UsedTime From Coupon c
              Join UserCoupon uc On c.CouponId = uc.CouponId
                  WHERE UserId = @UserId";
        return await conn.QueryAsync<CouponResponse>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 查看所有優惠卷
    /// </summary>
    /// <param name="request">優惠卷搜尋請求</param>
    /// <returns>優惠卷資訊列表</returns>
    public async Task<IEnumerable<CouponResponse>> GetAllCoupons(CouponSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();
        int offset = request.pageIndex * request.pageSize;
        var sql =
            @"SELECT c.*,
                   Count(*) OVER() AS TotalCount
            FROM   Coupon c
            WHERE  (@keyWords IS NULL OR c.Name LIKE '%' + @keyWords + '%')
            AND    (@CreaterId IS NULL OR c.CreaterId = @CreaterId)
            AND    (@IsActive IS NULL OR c.IsActive = @IsActive)
            AND    (@Type IS NULL OR c.Type = @Type)
            ORDER BY 
                    case when @sortBy='StartTime' and @sortOrder='asc' then c.StartTime end asc,
                    case when @sortBy='StartTime' and @sortOrder='desc' then c.StartTime end desc,
                    couponId
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        return await conn.QueryAsync<CouponResponse>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                CreaterId = request.CreaterId,
                keyWords = request.keyWords,
                IsActive = request.IsActive,
                Type = request.Type,
                sortBy = request.sortBy,
                sortOrder = request.sortOrder,
            }
        );
    }

    /// <summary>
    /// 新增優惠卷
    /// </summary>
    /// <param name="request">優惠卷新增請求</param>
    /// <returns>影響列數</returns>
    public async Task<int> CreateCoupons(CouponInsertRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
                      INSERT INTO Coupon (
                          CreaterId,
                          Code, 
                          Name, 
                          Type, 
                          Discount, 
                          MinimunSpend, 
                          StartTime, 
                          EndTime,
                          IsActive
                      ) 
                      VALUES (
                          @CreaterId,
                          @Code, 
                          @Name, 
                          @Type, 
                          @Discount, 
                          @MinimunSpend, 
                          @StartTime, 
                          @EndTime,
                          @IsActive
                      );";
        return await conn.ExecuteAsync(sql, request);
    }

    /// <summary>
    /// 管理員編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    public async Task<int> AdminUpdateCoupons(CouponUpdateRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              UPDATE Coupon 
              SET 
                  Name =  COALESCE(@Name, Name),
                  Type =   COALESCE(@Type, Type),
                  Discount = COALESCE(@Discount, Discount),
                  MinimunSpend = COALESCE(@MinimunSpend, MinimunSpend),
                  StartTime =  COALESCE(@StartTime, StartTime),
                  EndTime =  COALESCE(@EndTime, EndTime),
                  IsActive= COALESCE(@IsActive, IsActive)
              WHERE 
                  CouponId = @CouponId;";
        return await conn.ExecuteAsync(sql, request);
    }

    /// <summary>
    /// 賣家編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    public async Task<int> SellerUpdateCoupons(CouponUpdateRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              UPDATE Coupon 
              SET 
                  Name =  COALESCE(@Name, Name),
                  Type =   COALESCE(@Type, Type),
                  Discount =    COALESCE(@Discount, Discount),
                  MinimunSpend =   COALESCE(@MinimunSpend, MinimunSpend),
                  StartTime =  COALESCE(@StartTime, StartTime),
                  EndTime =  COALESCE(@EndTime, EndTime),
                  IsActive= COALESCE(@IsActive, IsActive)
              WHERE 
                  CouponId = @CouponId
              AND CreaterId = @CreaterId;";
        return await conn.ExecuteAsync(sql, request);
    }

    /// <summary>
    /// 用戶領取優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>優惠卷 ID</returns>
    public async Task<int> CreateUserCoupon(UserCouponInsertRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
               INSERT INTO UserCoupon 
                      (
                          UserId,
                          CouponId,
                          CreateTime
                      ) 
                      VALUES (
                          @UserId,
                          @CouponId,
                          @CreateTime
                      );
              SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.ExecuteScalarAsync<int>(sql, request);
    }

    /// <summary>
    /// 訂單建立成功後連結優惠卷
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <param name="userCouponId">用戶優惠卷 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateUserCoupon(int orderId, int userCouponId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              UPDATE UserCoupon 
              SET 
                  OrderId =  @OrderId
              WHERE 
                  UserCouponId = @UserCouponId";
        return await conn.ExecuteAsync(sql, new { OrderId = orderId, UserCouponId = userCouponId });
    }

    /// <summary>
    /// 完成優惠卷使用
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>影響列數</returns>
    public async Task<int> CompleteUserCoupon(string orderNumber)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              UPDATE UserCoupon 
              SET 
                  UsedTime=GetDate()
              WHERE 
                  OrderId in (SELECT OrderId FROM [Order] WHERE OrderNumber = @OrderNumber)";
        return await conn.ExecuteAsync(sql, new { OrderNumber = orderNumber });
    }
}
