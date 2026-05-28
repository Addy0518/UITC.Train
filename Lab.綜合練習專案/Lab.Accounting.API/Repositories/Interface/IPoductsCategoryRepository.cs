namespace Lab.Accounting.API.Repositories
{
    public interface IPoductsCategoryRepository
    {
        /// <summary>
        /// 查看指定類別底下的所有層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        Task<IEnumerable<MallProductCategory>> GetSonCategories(int fatherCategoryId);

        /// <summary>
        /// 查看指定類別往上的所有層級類別
        /// </summary>
        /// <param name="sonCategoryId">商品子類別 ID</param>
        /// <returns>商品類別</returns>
        Task<IEnumerable<MallProductCategory>> GetFatherCategories(int sonCategoryId);

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        Task<IEnumerable<MallProductCategory>> GetOneSonCategory(int fatherCategoryId);

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryName">類別名稱</param>
        /// <param name="parentId">父類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        Task<int> AddCategory(string categoryName, int? parentId);

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        Task<int> DeleteCategory(int categoryId);
    }
}
