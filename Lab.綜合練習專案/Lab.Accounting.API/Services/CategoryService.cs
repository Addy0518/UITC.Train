using NPOI.HPSF;
using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Services
{
    public class CategoryService(IPoductsCategoryRepository poductsCategoryRepository, IWebHostEnvironment env)
        : ICategoryService
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
            if (request.ProductParentId == 0)
            {
                request.ProductParentId = null;
            }

            if (request.ProductParentId == null && request.ProductCategoryImgFile == null)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "ProductCategoryImgFile", new[] { "頂層類別必須上傳圖片 !" } },
                };

                return ApiResponseHelper.RequestError<int>(errors);
            }
            var target = await poductsCategoryRepository.AddCategory(request);

            if (target <= 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            if (request.ProductCategoryImgFile != null)
            {
                var filename = await FileUploadHelper.SaveFileAsync(
                    request.ProductCategoryImgFile,
                    env.WebRootPath,
                    "CategoryImg"
                );
                if (filename == null)
                {
                    return ApiResponseHelper.InternalException<int>("圖片上傳失敗");
                }
                var updateResult = await poductsCategoryRepository.UploadCategoryImg(target, filename);
                if (updateResult <= 0)
                {
                    // DB 失敗，把剛存的實體檔案清掉，避免孤兒檔案
                    FileUploadHelper.DeleteFile(env.WebRootPath, "CategoryImg", filename);
                    return ApiResponseHelper.InternalException<int>("圖片上傳失敗");
                }
            }

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>刪除了幾筆</returns>
        public async Task<ApiResponse<int>> DeleteCategory(int categoryId)
        {
            IEnumerable<MallProductCategory> targets = await poductsCategoryRepository.DeleteCategory(categoryId);

            if (!targets.Any())
            {
                return ApiResponseHelper.NotFound<int>();
            }

            foreach (var target in targets)
            {
                FileUploadHelper.DeleteFile(env.WebRootPath, "CategoryImg", target.ProductCategoryImg);
            }

            return ApiResponseHelper.Success(targets.Count());
        }
    }
}
