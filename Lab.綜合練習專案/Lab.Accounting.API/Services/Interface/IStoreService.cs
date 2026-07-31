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
        /// 查看所有追蹤賣場的用戶數量
        /// </summary>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>用戶追蹤數量</returns>
        Task<ApiResponse<int>> GetStoreFollowers(int storeId);

        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="request">註冊資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> StoreRegister(StoreRegisterRequest request);

        /// <summary>
        /// 賣場升級成公司帳號
        /// </summary>
        /// <param name="request">公司資訊</param>
        /// <returns>審核表 ID</returns>
        Task<ApiResponse<int>> StoreUpdateToCompany(StoreUpdateToCompanyRequest request);

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UpdateStore(StoreUpdateRequest request);

        /// <summary>
        /// 用戶追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> FollowStore(int userId, int storeId);

        /// <summary>
        /// 用戶取消追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UnfollowStore(int userId, int storeId);

        /// <summary>
        /// 查看用戶是否已追蹤某賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>是否已追蹤</returns>
        Task<ApiResponse<bool>> IsFollowingStore(int userId, int storeId);
    }
}
