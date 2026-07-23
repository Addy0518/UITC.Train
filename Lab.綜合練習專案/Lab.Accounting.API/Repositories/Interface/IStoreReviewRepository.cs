using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IStoreReviewRepository
    {
        /// <summary>
        /// 取得單一賣場審核資訊
        /// </summary>
        /// <param name="reviewId">審核表 ID</param>
        /// <returns>單一賣場審核資訊</returns>
        Task<StoreReview> GetStoreReview(int reviewId);

        /// <summary>
        /// 取得賣場審核資訊
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>賣場審核資訊</returns>
        Task<IEnumerable<StoreReview>> GetAllStoreReview(StoreRiviewSearchRequest request);

        /// <summary>
        /// 賣場升級成公司帳號審核
        /// </summary>
        /// <param name="request">公司資訊</param>
        /// <returns>審核表 ID</returns>
        Task<int> StoreUpdateToCompanyReview(StoreUpdateToCompanyRequest request);

        /// <summary>
        /// 賣場審核通過或駁回
        /// </summary>
        /// <param name="request">賣場審核請求</param>
        /// <returns>影響列數</returns>
        Task<int> ApproveOrRejectStoreReview(StoreReviewRequest request);

        /// <summary>
        /// 上傳公司賣場文件路徑
        /// </summary>
        /// <param name="reviewId">審核表 ID</param>
        /// <param name="path">文件路徑</param>
        /// <returns>影響列數</returns>
        Task<int> StoreDocumentUpload(int reviewId, string path);
    }
}
