using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Services
{
    public interface IStoreService
    {
        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        Task<ApiResponse<StoreResponse>> GetStore(int sellerId);

        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="request">註冊資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> StoreRegister(StoreRegisterRequest request);

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UpdateStore(StoreUpdateRequest request);
    }
}
