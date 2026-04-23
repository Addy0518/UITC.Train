using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsCategoryRepositories
    {
        /// <summary>
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategoryId">商品類別 ID</param>
        /// <returns>商品類別 ID</returns>
        Task<MallProductCategory> GetCategory(int? productcategoryId = null);

        /// <summary>
        /// 新增商品跟類別關聯
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="productcategoryId">商品類別 Id</param>
        /// <returns>影響列數</returns>
        Task<int> CreateProductsCategory(int productId, int productcategoryId);

        /// <summary>
        /// 刪除商品跟類別關聯
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <returns>影響列數</returns>
        Task<int> DeleteProductsCategory(int productId);
    }
}
