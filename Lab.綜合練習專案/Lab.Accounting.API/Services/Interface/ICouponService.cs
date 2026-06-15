using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Services
{
    public interface ICouponService
    {
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
        /// 新增優惠卷
        /// </summary>
        /// <param name="request">優惠卷新增請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> CreateCoupons(CouponInsertRequest request);

        /// <summary>
        /// 編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UpdateCoupons(CouponUpdateRequest request);
    }
}
