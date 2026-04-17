using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsRepositories
    {
        /// <summary>
        /// 查看所有商品 ( 分頁 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <returns>商品列表</returns>
        Task<IEnumerable<ProductsResponse>> GetAllProducts(int pageIndex, int pageSize);

        /// <summary>
        /// 查看單一商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>商品資訊</returns>
        Task<ProductsResponse> GetProducts(int productId, int userId);

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
        /// 刪除單一商品
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        Task<int> DeleteProducts(int productsId, int userId);

        /// <summary>
        /// 設定商品庫存
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="purchaseQuantity">購買數量</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        Task<int> SetStock(int productsId, int purchaseQuantity, int userId);

        /// <summary>
        /// 購買商品
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        Task<int> BuyProducts(int productsId, int userId);
    }
}
