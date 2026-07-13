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
            if (string.IsNullOrEmpty(user.UserAddress) || string.IsNullOrEmpty(user.UserZipCode))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "UserAddress", new[] { "開店前請先完善您的地址資訊，供日後出貨使用!" } },
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
                    StoreCompanyName = request.StoreCompanyName,
                    StoreUnifiedNumber = request.StoreUnifiedNumber,
                    StoreName = request.StoreName,
                    IsDelete = IsDeleteStatusEnum.Normal,
                };
                var result = await sellerRepository.StoreRegister(seller);

                if (result == null)
                    return ApiResponseHelper.InternalException<int>();

                var role = await userRepository.UpdateRole(request.UserId, RolesAuth.賣家);

                trxScope.Complete();
                return ApiResponseHelper.Success<int>(result, "成功!");
            }
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
                return ApiResponseHelper.InternalException<int>("賣家資訊更新失敗");
            return ApiResponseHelper.Success<int>(result);
        }
    }
}
