using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Services
{
    public class CategoryService(IPoductsCategoryRepository poductsCategoryRepository) : ICategoryService
    {
        /// <summary>
        /// 查看指定類別底下的所有層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetSonCategories(int fatherCategoryId)
        {
            var target = await poductsCategoryRepository.GetSonCategories(fatherCategoryId);

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看指定類別往上的所有層級類別
        /// </summary>
        /// <param name="sonCategoryId">商品子類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetFatherCategories(int sonCategoryId)
        {
            var target = await poductsCategoryRepository.GetFatherCategories(sonCategoryId);

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看最頂層一層的父類別
        /// </summary>
        /// <returns>商品類別</returns>
        public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetOneFatherCategory()
        {
            var target = await poductsCategoryRepository.GetOneFatherCategory();

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetOneSonCategory(int fatherCategoryId)
        {
            var target = await poductsCategoryRepository.GetOneSonCategory(fatherCategoryId);

            if (!target.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name = "request" > 類別新增資訊 </param >
        /// <returns>新增的類別 ID </returns>
        public async Task<ApiResponse<int>> AddCategory(CategoryInsertRequest request)
        {
            var target = await poductsCategoryRepository.AddCategory(request);

            if (target <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        public async Task<ApiResponse<int>> DeleteCategory(int categoryId)
        {
            var target = await poductsCategoryRepository.DeleteCategory(categoryId);

            if (target <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            return ApiResponseHelper.Success(target);
        }
    }
}
