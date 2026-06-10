namespace Lab.Accounting.API.Services;

public interface IShoppingCarService
{
    /// <summary>
    /// 查看購物車中的所有商品
    /// </summary>
    /// <param name="userId">使用者 Id</param>
    /// <returns>購物車中的所有商品</returns>
    Task<ApiResponse<IEnumerable<ProductDetails>>> GetAllProductsInShoppingCar(int userId);

    /// <summary>
    /// 新增單一商品到購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <param name="boughtquantity">購買數量</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> AddProductsInShoppingCar(int productsId, int userId, int boughtquantity);

    /// <summary>
    /// 刪除單一商品從購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> DeleteProductsInShoppingCar(int productsId, int userId);
}
