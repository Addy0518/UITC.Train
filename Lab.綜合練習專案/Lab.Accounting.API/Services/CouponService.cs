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
        /// 查看優惠卷
        /// </summary>
        /// <param name="couponId">優惠卷 ID </param>
        /// <returns>優惠卷資訊</returns>
        public async Task<ApiResponse<CouponResponse>> GetCoupon(int couponId)
        {
            var target = await couponRepository.GetCoupon(couponId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<CouponResponse>();
            }

            return ApiResponseHelper.Success(target);
        }

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
        /// 查看用戶可領取的優惠卷
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>可領取優惠卷資訊列表</returns>
        public async Task<ApiResponse<IEnumerable<CouponResponse>>> GetCanReceiveCoupon(int userId)
        {
            var target = await couponRepository.GetCanReceiveCoupon(userId);

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
            if (request.Type == CouponTypeEnum.百分比折扣)
            {
                if (request.Discount > 100)
                {
                    var errors = new Dictionary<string, string[]> { { "Discount", new[] { "折扣不能大於100!" } } };

                    return ApiResponseHelper.RequestError<int>(errors);
                }
            }

            if (request.EndTime <= request.StartTime)
            {
                var errors = new Dictionary<string, string[]> { { "EndTime", new[] { "結束時間必須晚於開始時間!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            request.Code = Guid.NewGuid().ToString();

            var target = await couponRepository.CreateCoupons(request);

            if (target <= 0)
            {
                return ApiResponseHelper.InternalException<int>("新增優惠卷錯誤");
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 管理員編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> AdminUpdateCoupons(CouponUpdateRequest request)
        {
            if (request.EndTime <= request.StartTime)
            {
                var errors = new Dictionary<string, string[]> { { "EndTime", new[] { "結束時間必須晚於開始時間!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var coupon = await couponRepository.GetCoupon(request.CouponId);

            if (coupon == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var target = await couponRepository.AdminUpdateCoupons(request);

            if (target <= 0)
            {
                return ApiResponseHelper.InternalException<int>("編輯優惠卷錯誤");
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 賣家編輯優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> SellerUpdateCoupons(CouponUpdateRequest request)
        {
            if (request.EndTime <= request.StartTime)
            {
                var errors = new Dictionary<string, string[]> { { "EndTime", new[] { "結束時間必須晚於開始時間!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var coupon = await couponRepository.GetCoupon(request.CouponId);

            if (coupon == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            if (coupon.CreaterId != request.CreaterId)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "CreaterId", new[] { "你沒有權限編輯這張優惠卷!" } },
                };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            var target = await couponRepository.SellerUpdateCoupons(request);

            if (target <= 0)
            {
                return ApiResponseHelper.InternalException<int>("編輯優惠卷錯誤");
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 用戶領取優惠卷
        /// </summary>
        /// <param name="request">優惠卷編輯請求</param>
        /// <returns>優惠卷 ID</returns>
        public async Task<ApiResponse<int>> CreateUserCoupon(UserCouponInsertRequest request)
        {
            var coupon = await couponRepository.GetCoupon(request.CouponId);

            if (coupon == null || !coupon.IsActive || DateTime.Now > coupon.EndTime)
            {
                var errors = new Dictionary<string, string[]> { { "Coupon", new[] { "優惠卷已過期或不存在!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            var userCoupon = await couponRepository.GetUserCoupon(request.UserId);

            if (userCoupon.Any(c => c.CouponId == request.CouponId))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "UserCoupon", new[] { "你已經領取過這張優惠卷了!" } },
                };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var receive = await couponRepository.SetCouponStock(request.CouponId);

                if (receive <= 0)
                {
                    var errors = new Dictionary<string, string[]> { { "Coupon", new[] { "優惠卷已被領取完畢!" } } };
                    return ApiResponseHelper.RequestError<int>(errors);
                }

                var target = await couponRepository.CreateUserCoupon(request);

                if (target <= 0)
                {
                    return ApiResponseHelper.InternalException<int>("領取優惠卷錯誤");
                }

                trxScope.Complete();
                return ApiResponseHelper.Success(target);
            }
        }
    }
}
