using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IStoreRepository
    {
        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="seller">註冊資訊</param>
        /// <returns>影響列數</returns>
        Task<int> StoreRegister(Store seller);

        /// <summary>
        /// 取得賣場資訊 ( 賣場 ID )
        /// </summary>
        /// <param name="storeId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        Task<Store> GetStorebyStoreId(int storeId);

        /// <summary>
        /// 取得賣場資訊 ( 賣家 ID )
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        Task<Store> GetStore(int sellerId);

        /// <summary>
        /// 通過審核正式成立帳號
        /// </summary>
        /// <param name="seller">公司資訊</param>
        /// <returns>影響列數</returns>
        Task<int> StoreUpdateToCompany(StoreCompanyReview seller);

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        Task<int> UpdateStore(StoreUpdateRequest request);

        /// <summary>
        /// 用戶追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        Task<int> FollowStore(int userId, int storeId);

        /// <summary>
        /// 用戶取消追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        Task<int> UnfollowStore(int userId, int storeId);

        /// <summary>
        /// 查看用戶是否已追蹤某賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>是否已追蹤</returns>
        Task<bool> IsFollowingStore(int userId, int storeId);
    }
}
