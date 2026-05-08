namespace Lab.Accounting.API.Repositories.Interface;

public interface IProductsRepository
{
    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <param name="userId">使用者 Id</param>
    /// <param name="isDelete">是否為刪除狀態</param>
    /// <returns>商品列表</returns>
    Task<IEnumerable<ProductsResponse>> GetAllProducts(
        int pageIndex,
        int pageSize,
        int? userId = null,
        IsDeleteStatusEnum? isDelete = IsDeleteStatusEnum.Normal
    );

    /// <summary>
    /// 查看單一商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    Task<ProductsResponse> GetProducts(int productId);

    /// <summary>
    /// 查看商品類別
    /// </summary>
    /// <param name="productcategoryId">商品類別 ID</param>
    /// <returns>商品類別</returns>
    Task<IEnumerable<MallProductCategory>> GetCategory(int? productcategoryId = null);

    /// <summary>
    /// 新增單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>影響列數</returns>
    Task<int> CreateProducts(MallProducts products);

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateProducts(MallProducts products);

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateProductsDeleteStatus(int userId, IEnumerable<int> productId);

    /// <summary>
    /// 設定商品庫存
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="purchaseQuantity">購買數量</param>
    /// <returns>影響列數</returns>
    Task<int> SetStock(int productsId, int purchaseQuantity);

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <param name="isDelete">刪除狀態</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<int> DeleteProducts(int productsId, IsDeleteStatusEnum isDelete, int userId);
}
