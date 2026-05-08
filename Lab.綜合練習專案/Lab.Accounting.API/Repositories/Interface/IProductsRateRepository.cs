namespace Lab.Accounting.API.Repositories.Interface;

public interface IProductsRateRepository
{
    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="productrate">商品評價資訊</param>
    /// <returns>影響列數</returns>
    Task<int> CreateProductRate(MallProductsRate productrate);

    /// <summary>
    /// 刪除單一商品評價
    /// </summary>
    /// <param name="productRateId">商品評價 ID</param>
    /// <returns>影響列數</returns>
    Task<int> DeleteProductRate(int productRateId);

    /// <summary>
    /// 查看單一商品評價
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>商品評價資訊</returns>
    Task<IEnumerable<MallProductsRate>> GetProductRate(int productId);

    /// <summary>
    /// 計算商品評分平均值
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>評分平均值</returns>
    Task<decimal> CountAVGProductRate(int productId);
}
