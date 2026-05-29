using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        /// <summary>
        /// 查看指定類別底下的所有層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallProductCategory>>))]
        public async Task<IActionResult> GetSonCategories([FromQuery] int fatherCategoryId)
        {
            var target = await categoryService.GetSonCategories(fatherCategoryId);
            return Ok(target);
        }

        /// <summary>
        /// 查看指定類別往上的所有層級類別
        /// </summary>
        /// <param name="sonCategoryId">商品子類別 ID</param>
        /// <returns>商品類別</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallProductCategory>>))]
        public async Task<IActionResult> GetFatherCategories([FromQuery] int sonCategoryId)
        {
            var target = await categoryService.GetFatherCategories(sonCategoryId);
            return Ok(target);
        }

        /// <summary>
        /// 查看最頂層一層的父類別
        /// </summary>
        /// <returns>商品類別</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallProductCategory>>))]
        public async Task<IActionResult> GetOneFatherCategory()
        {
            var target = await categoryService.GetOneFatherCategory();
            return Ok(target);
        }

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallProductCategory>>))]
        public async Task<IActionResult> GetOneSonCategory([FromQuery] int fatherCategoryId)
        {
            var target = await categoryService.GetOneSonCategory(fatherCategoryId);
            return Ok(target);
        }

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name="request">類別新增資訊</param>
        /// <returns>新增的類別 ID </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> AddCategory([FromBody] CategoryInsertRequest request)
        {
            var target = await categoryService.AddCategory(request);
            return Ok(target);
        }

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId)
        {
            var target = await categoryService.DeleteCategory(categoryId);
            return Ok(target);
        }
    }
}
