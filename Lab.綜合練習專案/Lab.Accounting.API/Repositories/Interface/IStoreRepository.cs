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
        /// 賣場升級成公司帳號
        /// </summary>
        /// <param name="seller">公司資訊</param>
        /// <returns>審核表 ID</returns>
        Task<int> StoreUpdateToCompanyReview(StoreCompanyReview seller);

        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        Task<Store> GetStore(int sellerId);

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        Task<int> UpdateStore(StoreUpdateRequest request);
    }
}
