using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class LogisticsController(ILogisticsService logisticsService) : ControllerBase
    {
        /// <summary>
        /// 產生綠界超商門市地圖網址
        /// </summary>
        /// <param name="request">物流訂單資訊</param>
        /// <returns>地圖網址</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public IActionResult GetCvsMapUrl([FromQuery] GetCvsMapRequest request)
        {
            return Ok(logisticsService.GetCvsMapUrl(request));
        }

        /// <summary>
        /// 接收綠界回傳的門市資料存進暫存表
        /// </summary>
        /// <param name="request">綠界回傳門市資料</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CvsStoreCallback([FromForm] CvsStoreCallbackRequest request)
        {
            await logisticsService.HandleCvsStoreCallback(request);

            // 綠界要求回傳純文字 "1|OK"，不是 JSON
            return Content("1|OK", "text/plain");
        }

        /// <summary>
        /// 收件人資料存進暫存表
        /// </summary>
        /// <param name="request">物流暫存表單資料</param>
        /// <returns>操作結果</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> SaveLogisticsTemp([FromBody] LogisticsTempInsertRequest request)
        {
            return Ok(await logisticsService.SaveLogisticsTemp(request));
        }
    }
}
