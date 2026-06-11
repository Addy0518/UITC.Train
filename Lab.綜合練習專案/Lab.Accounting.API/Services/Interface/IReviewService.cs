using Lab.Accounting.API.Common.Requests.Products;

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
        /// 審核通過或駁回
        /// </summary>
        /// <param name="request">商品審核請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> ApproveOrRejectProductsReview(ProductsRivewRequest request);
    }
}
