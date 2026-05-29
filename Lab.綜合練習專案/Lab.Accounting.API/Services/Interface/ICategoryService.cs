namespace Lab.Accounting.API.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// 查看指定類別底下的所有層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        Task<ApiResponse<IEnumerable<MallProductCategory>>> GetSonCategories(int fatherCategoryId);

        /// <summary>
        /// 查看指定類別往上的所有層級類別
        /// </summary>
        /// <param name="sonCategoryId">商品子類別 ID</param>
        /// <returns>商品類別</returns>
        Task<ApiResponse<IEnumerable<MallProductCategory>>> GetFatherCategories(int sonCategoryId);

        /// <summary>
        /// 查看最頂層一層的父類別
        /// </summary>
        /// <returns>商品類別</returns>
        Task<ApiResponse<IEnumerable<MallProductCategory>>> GetOneFatherCategory();

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        Task<ApiResponse<IEnumerable<MallProductCategory>>> GetOneSonCategory(int fatherCategoryId);

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name="request">類別新增資訊</param>
        /// <returns>新增的類別 ID </returns>
        Task<ApiResponse<int>> AddCategory(CategoryInsertRequest request);

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        Task<ApiResponse<int>> DeleteCategory(int categoryId);
    }
}
