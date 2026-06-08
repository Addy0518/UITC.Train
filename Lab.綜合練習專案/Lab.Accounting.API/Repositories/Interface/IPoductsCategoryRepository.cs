using Lab.Accounting.API.Common.Requests.Category;

namespace Lab.Accounting.API.Repositories
{
    public interface IPoductsCategoryRepository
    {
        /// <summary>
        /// 查看指定類別
        /// </summary>
        /// <param name="categoryId">商品類別 ID</param>
        /// <returns>商品類別</returns>
        Task<MallProductCategory> GetCategories(int categoryId);

        /// <summary>
        /// 查看所有類別
        /// </summary>
        /// <param name="request">商品類別搜尋請求</param>
        /// <returns>所有商品類別</returns>
        Task<IEnumerable<CategoryResponse>> GetAllCategories(CategorySearchRequest request);

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
        /// 查看最頂層一層的父類別
        /// </summary>
        /// <returns>商品類別</returns>
        Task<IEnumerable<MallProductCategory>> GetOneFatherCategory();

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        Task<IEnumerable<MallProductCategory>> GetOneSonCategory(int fatherCategoryId);

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        ///  <param name = "request" > 類別新增資訊 </param >
        /// <returns>新增的類別 ID </returns>
        Task<int> AddCategory(CategoryInsertRequest request);

        /// <summary>
        /// 新增類別圖片
        /// </summary>
        /// <param name = "categoryId" > 商品類別 ID </param >
        /// <param name = "fileName" > 檔案名稱 </param >
        /// <returns>影響列數 </returns>
        Task<int> UploadCategoryImg(int categoryId, string fileName);

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>刪除的類別資訊 </returns>
        Task<IEnumerable<MallProductCategory>> DeleteCategory(int categoryId);
    }
}
