namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsCategoryRepositories
    {
        /// <summary>
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategory">商品類別</param>
        /// <returns>影響列數</returns>
        Task<int> GetCategory(string productcategory);

        /// <summary>
        /// 新增單一商品類別
        /// </summary>
        /// <param name="productcategory">商品類別</param>
        /// <returns>影響列數</returns>
        Task<int> CreateCategory(string productcategory);

        /// <summary>
        /// 新增商品跟類別關聯
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="productcategoryId">商品類別 Id</param>
        /// <returns>影響列數</returns>
        Task<int> CreateProductsCategory(int productId, int productcategoryId);
    }
}
