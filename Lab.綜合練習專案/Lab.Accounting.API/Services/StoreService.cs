using Lab.Accounting.API.Common.Requests.Store;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;

namespace Lab.Accounting.API.Services
{
    public class StoreService(
        IStoreRepository storeRepository,
        IStoreReviewRepository storeReviewRepository,
        IUserRepository userRepository,
        IProductsRepository productsRepository,
        IProductsRateRepository productsRateRepository,
        INotificationService notificationService,
        IWebHostEnvironment env
    ) : IStoreService
    {
        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        public async Task<ApiResponse<StoreResponse>> GetStore(int sellerId)
        {
            var target = await storeRepository.GetStore(sellerId);

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
            var exist = await storeRepository.GetStore(request.UserId);

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
                var result = await storeRepository.StoreRegister(seller);

                if (result == null)
                    return ApiResponseHelper.InternalException<int>("註冊失敗");

                var role = await userRepository.UpdateRole(request.UserId, RolesAuth.賣家);
                if (role <= 0)
                    return ApiResponseHelper.InternalException<int>("更新使用者身份失敗");

                await notificationService.CreateNotification(
                    request.UserId,
                    NotificationTypeEnum.StoreCompanyApproved,
                    "賣場審核通過",
                    $"您的賣場通過審核，已升級為公司帳號。"
                );
                trxScope.Complete();

                return ApiResponseHelper.Success<int>(result, "成功!");
            }
        }

        /// <summary>
        /// 賣場升級成公司帳號
        /// </summary>
        /// <param name="request">公司資訊</param>
        /// <returns>審核表 ID</returns>
        public async Task<ApiResponse<int>> StoreUpdateToCompany(StoreUpdateToCompanyRequest request)
        {
            if (request.UserId <= 0 || request.StoreId <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var exist = await storeRepository.GetStore(request.UserId);

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
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var seller = new StoreUpdateToCompanyRequest
                {
                    UserId = request.UserId,
                    StoreId = request.StoreId,
                    StoreCompanyName = request.StoreCompanyName,
                    StoreUnifiedNumber = request.StoreUnifiedNumber,
                    ReviewStatus = ReviewStatusEnum.Pending,
                    CreateTime = DateTime.Now,
                };

                var reviewId = await storeReviewRepository.StoreUpdateToCompanyReview(seller);
                if (reviewId <= 0)
                {
                    return ApiResponseHelper.InternalException<int>("升級公司帳號失敗");
                }

                if (request.Document != null)
                {
                    var imgUpload = await StoreDocumentUpload(request.Document, reviewId);
                    if (imgUpload.CodeStatus != CodeStatusEnum.Success)
                    {
                        var errors = new Dictionary<string, string[]>
                        {
                            { "Document", new[] { "文件上傳失敗 , 請重新申請 !" } },
                        };

                        return ApiResponseHelper.RequestError<int>(errors);
                    }
                }

                trxScope.Complete();
                return ApiResponseHelper.Success<int>(reviewId);
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

            var result = await storeRepository.UpdateStore(request);
            if (result <= 0)
                return ApiResponseHelper.InternalException<int>("送出審核申請失敗");
            return ApiResponseHelper.Success<int>(result);
        }

        /// <summary>
        /// 賣場公司文件上傳
        /// </summary>
        /// <param name="storeFiles">賣場公司文件檔案</param>
        /// <param name="reviewId">審核表 ID</param>
        /// <returns>文件路徑</returns>
        private async Task<ApiResponse<string>> StoreDocumentUpload(IFormFile storeFiles, int reviewId)
        {
            var path = await FileUploadHelper.SaveFileAsync(storeFiles, env.WebRootPath, "StoreUpdateDocument");
            var imgUpload = await storeReviewRepository.StoreDocumentUpload(reviewId, path);

            if (imgUpload <= 0)
            {
                // DB 失敗，把剛存的實體檔案清掉，避免孤兒檔案
                FileUploadHelper.DeleteFile(env.WebRootPath, "StoreUpdateDocument", path);
                return ApiResponseHelper.InternalException<string>("圖片上傳失敗");
            }

            return ApiResponseHelper.Success(path);
        }

        /// <summary>
        /// 用戶追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> FollowStore(int userId, int storeId)
        {
            var store = await storeRepository.GetStorebyStoreId(storeId);
            if (store == null)
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "查無賣場" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            if (store.UserId == userId)
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "無法追蹤自己的賣場" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var follow = await storeRepository.FollowStore(userId, storeId);

            if (follow <= 0)
                return ApiResponseHelper.InternalException<int>("追蹤失敗 ! ");
            return ApiResponseHelper.Success<int>(follow);
        }

        /// <summary>
        /// 用戶取消追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UnfollowStore(int userId, int storeId)
        {
            var target = await storeRepository.GetStorebyStoreId(storeId);
            if (target == null)
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "查無賣場" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            if (target.UserId == userId)
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "無法取消追蹤自己的賣場" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var delete = await storeRepository.UnfollowStore(userId, storeId);

            if (delete <= 0)
                return ApiResponseHelper.InternalException<int>("取消追蹤失敗 ! ");
            return ApiResponseHelper.Success<int>(delete);
        }

        /// <summary>
        /// 查看用戶是否已追蹤某賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>是否已追蹤</returns>
        public async Task<ApiResponse<bool>> IsFollowingStore(int userId, int storeId)
        {
            var target = await storeRepository.GetStorebyStoreId(storeId);
            if (target == null)
            {
                var errors = new Dictionary<string, string[]> { { "Store", new[] { "查無賣場" } } };

                return ApiResponseHelper.RequestError<bool>(errors);
            }

            var isfollow = await storeRepository.IsFollowingStore(userId, storeId);

            return ApiResponseHelper.Success<bool>(isfollow);
        }
    }
}
