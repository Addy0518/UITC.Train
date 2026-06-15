using Lab.Accounting.API.Common.Requests.Coupon;
using Lab.Accounting.API.Common.Requests.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using NPOI.HPSF;
using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Services
{
    public class CouponService(ICouponRepository couponRepository, IWebHostEnvironment env) : ICouponService
    {
        /// <summary>
        /// 查看用戶優惠卷
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>優惠卷資訊列表</returns>
        public async Task<ApiResponse<IEnumerable<CouponResponse>>> GetUserCoupon(int userId)
        {
            var target = await couponRepository.GetUserCoupon(userId);

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<CouponResponse>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看所有優惠卷
        /// </summary>
        /// <param name="request">優惠卷搜尋請求</param>
        /// <returns>優惠卷資訊列表</returns>
        public async Task<ApiResponse<IEnumerable<CouponResponse>>> GetAllCoupons(CouponSearchRequest request)
        {
            var target = await couponRepository.GetAllCoupons(request);

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<CouponResponse>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 新增優惠卷
        /// </summary>
        /// <param name="request">優惠卷新增請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> CreateCoupons(CouponInsertRequest request)
        {
            request.Code = Guid.NewGuid().ToString();
            var target = await couponRepository.CreateCoupons(request);

            if (target <= 0)
            {
                return ApiResponseHelper.InternalException<int>("新增優惠卷錯誤");
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateCoupons(CouponUpdateRequest request)
        {
            var target = await couponRepository.UpdateCoupons(request);

            if (target <= 0)
            {
                return ApiResponseHelper.InternalException<int>("新增優惠卷錯誤");
            }

            return ApiResponseHelper.Success(target);
        }
    }
}
