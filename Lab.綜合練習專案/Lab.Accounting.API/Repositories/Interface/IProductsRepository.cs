using Lab.Accounting.API.Common.Requests.Products;

namespace Lab.Accounting.API.Repositories.Interface;

public interface IProductsRepository
{
    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    Task<IEnumerable<ProductDetails>> GetAllProducts(ProductsSearchRequest request);

    /// <summary>
    /// 賣家查看自己的所有商品
    /// </summary>
    ///  <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    Task<IEnumerable<ProductDetails>> SellerGetAllProducts(ProductsSearchRequest request);

    /// <summary>
    /// 查看單一商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    Task<ProductDetails> GetProducts(int productId);

    /// <summary>
    /// 新增單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>商品 ID</returns>
    Task<int> CreateProducts(Product products);

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateProducts(Product products);

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateProductsDeleteStatus(int sellerId, IEnumerable<int> productId);

    /// <summary>
    /// 檢查商品名稱重複
    /// </summary>
    /// <param name="productsName">商品名稱</param>
    /// <param name="sellerId">賣家 ID </param>
    /// <param name="productId">商品 ID </param>
    /// <returns>影響列數</returns>
    Task<bool> ExistsProductsName(string productsName, int sellerId, int? productId = null);

    /// <summary>
    /// 設定商品庫存
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="purchaseQuantity">購買數量</param>
    /// <returns>影響列數</returns>
    Task<int> SetStock(int productsId, int purchaseQuantity);

    /// <summary>
    /// 計算賣家所有商品數量
    /// </summary>
    /// <param name="sellerId">賣家 Id</param>
    /// <returns>影響列數</returns>
    Task<int> CountSellerProducts(int sellerId);

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <param name="isDelete">刪除狀態</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    Task<int> DeleteProducts(int productsId, IsDeleteStatusEnum isDelete, int sellerId);

    /// <summary>
    /// 審核通過後新增審查表的商品 ID
    /// </summary>
    /// <param name="reviewId">審查表 ID </param>
    /// <param name="productsId">商品 ID </param>
    /// <returns>影響列數</returns>
    Task<int> UpdateReviewProductsId(int reviewId, int productsId);
}
