using Lab.Accounting.API.Common.Requests.Store;
using Microsoft.AspNetCore.Identity.Data;

namespace Lab.Accounting.API.Services
{
    public class StoreService(
        IStoreRepository sellerRepository,
        IUserRepository userRepository,
        IProductsRepository productsRepository,
        IProductsRateRepository productsRateRepository
    ) : IStoreService
    {
        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        public async Task<ApiResponse<StoreResponse>> GetStore(int sellerId)
        {
            var target = await sellerRepository.GetStore(sellerId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<StoreResponse>();
            }
            var allRateCount = await productsRateRepository.GetAllProductsRateCount(sellerId);
            var allRateAVG = await productsRateRepository.CountAVGAllProductRate(sellerId);

            var productCount = await productsRepository.CountSellerProducts(sellerId);
            var result = new StoreResponse
            {
                StoreId = target.StoreId,
                UserId = target.UserId,
                StoreName = target.StoreName,
                StoreUnifiedNumber = target.StoreUnifiedNumber,
                StoreCompanyName = target.StoreCompanyName,
                AllProductsRateCount = allRateCount,
                CountAVGAllProductRate = allRateAVG,
                AllProductsCount = productCount,
                CreateTime = target.CreateTime,
            };
            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="request">註冊資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> StoreRegister(StoreRegisterRequest request)
        {
            var user = await userRepository.GetUser(request.UserId);
            var missingFields = new List<string>();
            if (string.IsNullOrWhiteSpace(user.UserPhone))
                missingFields.Add("電話");
            if (string.IsNullOrWhiteSpace(user.UserAddress))
                missingFields.Add("地址");
            if (string.IsNullOrWhiteSpace(user.UserZipCode))
                missingFields.Add("郵遞區號");

            if (missingFields.Count > 0)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "UserProfile", new[] { $"開店前請先完善以下資料：{string.Join("、", missingFields)}" } },
                };
                return ApiResponseHelper.RequestError<int>(errors);
            }
            var exist = await sellerRepository.GetStore(request.UserId);

            if (exist != null)
            {
                var errors = new Dictionary<string, string[]> { { "Seller", new[] { "該帳號已是賣家!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var seller = new Store
                {
                    UserId = request.UserId,
                    StoreName = request.StoreName,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsDelete = IsDeleteStatusEnum.Normal,
                };
                var result = await sellerRepository.StoreRegister(seller);

                if (result == null)
                    return ApiResponseHelper.InternalException<int>("註冊失敗");

                var role = await userRepository.UpdateRole(request.UserId, RolesAuth.賣家);
                if (role <= 0)
                    return ApiResponseHelper.InternalException<int>("更新使用者身份失敗");
                trxScope.Complete();
                return ApiResponseHelper.Success<int>(result, "成功!");
            }
        }

        /// <summary>
        /// 賣場升級成公司帳號
        /// </summary>
        /// <param name="request">公司資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> StoreUpdateToCompany(StoreUpdateCompanyRequest request)
        {
            if (request.UserId <= 0 || request.StoreId <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var exist = await sellerRepository.GetStore(request.UserId);

            if (exist == null)
            {
                var errors = new Dictionary<string, string[]> { { "Seller", new[] { "該帳號尚未註冊成為賣家!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            if (!string.IsNullOrWhiteSpace(exist.StoreUnifiedNumber))
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "此賣場已經是企業賣場!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var seller = new StoreCompanyReview
            {
                UserId = request.UserId,
                StoreId = request.StoreId,
                StoreCompanyName = request.StoreCompanyName,
                StoreUnifiedNumber = request.StoreUnifiedNumber,
                DocumentPath = request.DocumentPath,
                ReviewStatus = ReviewStatusEnum.Pending,
                CreateTime = DateTime.Now,
            };

            var result = await sellerRepository.StoreUpdateToCompanyReview(seller);
            if (result <= 0)
                return ApiResponseHelper.InternalException<int>("升級公司帳號失敗");
            return ApiResponseHelper.Success<int>(result);
        }

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateStore(StoreUpdateRequest request)
        {
            if (request.UserId <= 0 || request.StoreId <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            var result = await sellerRepository.UpdateStore(request);
            if (result <= 0)
                return ApiResponseHelper.InternalException<int>("送出審核申請失敗");
            return ApiResponseHelper.Success<int>(result);
        }
    }
}
