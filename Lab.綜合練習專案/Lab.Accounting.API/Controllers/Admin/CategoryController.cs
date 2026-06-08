using Lab.Accounting.API.Common.Requests.Category;
using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers.Admin
{
    [Tags("Admin-Category")]
    [Route("api/admin/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = RolesAuth.管理者)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        /// <summary>
        /// 查看所有類別
        /// </summary>
        /// <param name="request">商品類別搜尋請求</param>
        /// <returns>所有商品類別</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MallProductCategory>>))]
        public async Task<IActionResult> GetAllCategories([FromQuery] CategorySearchRequest request)
        {
            return Ok(await categoryService.GetAllCategories(request));
        }

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name="request">類別新增資訊</param>
        /// <returns>新增的類別 ID </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> AddCategory([FromForm] CategoryInsertRequest request)
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
