using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Services
{
    public interface ICouponService
    {
        /// <summary>
        /// 查看優惠卷
        /// </summary>
        /// <param name="couponId">優惠卷 ID </param>
        /// <returns>優惠卷資訊</returns>
        Task<ApiResponse<CouponResponse>> GetCoupon(int couponId);

        /// <summary>
        /// 查看用戶優惠卷
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>優惠卷資訊列表</returns>
        Task<ApiResponse<IEnumerable<CouponResponse>>> GetUserCoupon(int userId);

        /// <summary>
        /// 查看所有優惠卷
        /// </summary>
        /// <param name="request">優惠卷搜尋請求</param>
        /// <returns>優惠卷資訊列表</returns>
        Task<ApiResponse<IEnumerable<CouponResponse>>> GetAllCoupons(CouponSearchRequest request);

        /// <summary>
        /// 查看用戶可領取的優惠卷
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>可領取優惠卷資訊列表</returns>
        Task<ApiResponse<IEnumerable<CouponResponse>>> GetCanReceiveCoupon(int userId);

        /// <summary>
        /// 新增優惠卷
        /// </summary>
        /// <param name="request">優惠卷新增請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> CreateCoupons(CouponInsertRequest request);

        /// <summary>
        /// 管理員編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> AdminUpdateCoupons(CouponUpdateRequest request);

        /// <summary>
        /// 賣家編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> SellerUpdateCoupons(CouponUpdateRequest request);

        /// <summary>
        /// 用戶領取優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>優惠卷 ID</returns>
        Task<ApiResponse<int>> CreateUserCoupon(UserCouponInsertRequest request);
    }
}
