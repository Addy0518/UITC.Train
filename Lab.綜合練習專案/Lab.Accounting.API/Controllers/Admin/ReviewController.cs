using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers.Admin
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = RolesAuth.管理者)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class ReviewController(IReviewService reviewService) : ControllerBase
    {
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        /// <summary>
        /// 查看商品審核
        /// </summary>
        /// <param name="reviewId">審核表 ID </param>
        /// <returns>審核資訊</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Review>))]
        public async Task<IActionResult> GetProductsReview([FromQuery] int reviewId)
        {
            return Ok(await reviewService.GetProductsReview(reviewId));
        }

        /// <summary>
        /// 查看所有商品審核
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>審核資訊</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ReviewResponse>))]
        public async Task<IActionResult> GetAllProductsReview([FromQuery] ProductsRiviewSearchRequest request)
        {
            return Ok(await reviewService.GetAllProductsReview(request));
        }

        /// <summary>
        /// 審核通過或駁回
        /// </summary>
        /// <param name="request">商品審核請求</param>
        /// <returns>影響列數</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> ApproveOrRejectProductsReview([FromBody] ProductsRivewRequest request)
        {
            request.AdminId = CurrentUserId;
            return Ok(await reviewService.ApproveOrRejectProductsReview(request));
        }
    }
}
