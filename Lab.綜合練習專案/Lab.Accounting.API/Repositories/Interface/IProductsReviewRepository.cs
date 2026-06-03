namespace Lab.Accounting.API.Repositories.Interface;

public interface IProductsReviewRepository
{
    /// <summary>
    /// 查看商品審核
    /// </summary>
    /// <param name="reviewId">審核表 ID </param>
    /// <returns>審核資訊</returns>
    Task<Review> GetProductsReview(int reviewId);

    /// <summary>
    /// 查看所有商品審核
    /// </summary>
    /// <param name="request">審核表搜尋請求</param>
    /// <returns>審核資訊</returns>
    Task<IEnumerable<Review>> GetAllProductsReview(ProductsRiviewSearchRequest request);

    /// <summary>
    /// 新增商品審核
    /// </summary>
    /// <param name="productsReview">賣家商品資訊</param>
    /// <returns>審核表 ID </returns>
    Task<int> CreateInsertProductsReview(ProductsReview productsReview);

    /// <summary>
    /// 審核通過或駁回
    /// </summary>
    /// <param name="request">商品審核請求</param>
    /// <returns>影響列數</returns>
    Task<int> ApproveOrRejectProductsReview(ProductsRivewRequest request);
}
