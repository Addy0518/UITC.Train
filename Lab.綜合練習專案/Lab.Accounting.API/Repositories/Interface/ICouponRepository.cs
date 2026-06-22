using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Repositories.Interface;

public interface ICouponRepository
{
    /// <summary>
    /// 查看優惠卷
    /// </summary>
    /// <param name="couponId">優惠卷 ID </param>
    /// <returns>優惠卷資訊</returns>
    Task<CouponResponse> GetCoupon(int couponId);

    /// <summary>
    /// 查看用戶優惠卷
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>優惠卷資訊列表</returns>
    Task<IEnumerable<CouponResponse>> GetUserCoupon(int userId);

    /// <summary>
    /// 查看所有優惠卷
    /// </summary>
    /// <param name="request">優惠卷搜尋請求</param>
    /// <returns>優惠卷資訊列表</returns>
    Task<IEnumerable<CouponResponse>> GetAllCoupons(CouponSearchRequest request);

    /// <summary>
    /// 查看用戶可領取的優惠卷
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>可領取優惠卷資訊列表</returns>
    Task<IEnumerable<CouponResponse>> GetCanReceiveCoupon(int userId);

    /// <summary>
    /// 新增優惠卷
    /// </summary>
    /// <param name="request">優惠卷新增請求</param>
    /// <returns>影響列數</returns>
    Task<int> CreateCoupons(CouponInsertRequest request);

    /// <summary>
    /// 管理員編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    Task<int> AdminUpdateCoupons(CouponUpdateRequest request);

    /// <summary>
    /// 賣家編輯優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>影響列數</returns>
    Task<int> SellerUpdateCoupons(CouponUpdateRequest request);

    /// <summary>
    /// 用戶領取優惠卷
    /// </summary>
    /// <param name="request">優惠卷編輯請求</param>
    /// <returns>優惠卷 ID</returns>
    Task<int> CreateUserCoupon(UserCouponInsertRequest request);

    /// <summary>
    /// 訂單建立成功後使用優惠卷
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <param name="couponId">優惠卷 ID</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateUserCoupon(int orderId, int couponId);

    /// <summary>
    /// 完成優惠卷使用
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>影響列數</returns>
    Task<int> CompleteUserCoupon(string orderNumber);

    /// <summary>
    /// 扣除優惠卷數量
    /// </summary>
    /// <param name="couponId">優惠卷 ID</param>
    /// <returns>影響列數</returns>
    Task<int> SetCouponStock(int couponId);
}
