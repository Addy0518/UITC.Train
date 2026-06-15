using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Repositories;

public class CouponRepository(DBConnecting connecting) : ICouponRepository
{
    /// <summary>
    /// 查看用戶優惠卷
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>優惠卷資訊列表</returns>
    public async Task<IEnumerable<CouponResponse>> GetUserCoupon(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select * From Coupon c
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
            WHERE  c.CreaterId = @CreaterId
            AND    (@keyWords IS NULL OR c.Name LIKE '%' + @keyWords + '%')
            AND    (@IsActive IS NULL OR c.IsActive = @IsActive)
            AND    (@Type IS NULL OR c.Type = @Type)
            ORDER BY c.CreateTime DESC 
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
    /// 編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateCoupons(CouponUpdateRequest request)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              UPDATE Coupon 
              SET 
                  Name =  COALESCE(@Name, Name),
                  Type =   COALESCE(@Type, Type),
                  Discount = @Discount,  COALESCE(@Discount, Discount),
                  MinimunSpend =   COALESCE(@MinimunSpend, MinimunSpend),
                  StartTime =  COALESCE(@StartTime, StartTime),
                  EndTime =  COALESCE(@EndTime, EndTime),
                  IsActive= COALESCE(@IsActive, IsActive)
              WHERE 
                  CouponId = @CouponId;";
        return await conn.ExecuteAsync(sql, request);
    }
}
