using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Services
{
    public interface IReviewService
    {
        /// <summary>
        /// 查看商品審核
        /// </summary>
        /// <param name="reviewId">審核表 ID </param>
        /// <returns>審核資訊</returns>
        Task<ApiResponse<Review>> GetProductsReview(int reviewId);

        /// <summary>
        /// 查看所有商品審核
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>審核資訊</returns>
        Task<ApiResponse<ReviewResponse>> GetAllProductsReview(ProductsRiviewSearchRequest request);

        /// <summary>
        /// 查看審查表所有圖片
        /// </summary>
        /// <param name="reviewId">審查表 ID </param>
        /// <returns>商品圖片 URL</returns>
        Task<ApiResponse<IEnumerable<ProductImg>>> GetReviewAllImg(int reviewId);

        /// <summary>
        /// 商品審核通過或駁回
        /// </summary>
        /// <param name="request">商品審核請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> ApproveOrRejectProductsReview(ProductsRivewRequest request);

        /// <summary>
        /// 取得單一賣場審核資訊
        /// </summary>
        /// <param name="reviewId">審核表 ID</param>
        /// <returns>單一賣場審核資訊</returns>
        Task<ApiResponse<StoreReview>> GetStoreReview(int reviewId);

        /// <summary>
        /// 取得賣場審核資訊
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>賣場審核資訊</returns>
        Task<ApiResponse<StoreReviewResponse>> GetAllStoreReview(StoreRiviewSearchRequest request);

        /// <summary>
        /// 賣場審核通過或駁回
        /// </summary>
        /// <param name="request">賣場審核請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> ApproveOrRejectStoreReview(StoreReviewRequest request);
    }
}
