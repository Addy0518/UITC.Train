using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class LogisticsController(ILogisticsService logisticsService, IConfiguration config) : ControllerBase
    {
        // 前端網址基底
        private string fronturl = config["FrontendUrl"];

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
        /// 接收綠界回傳的門市資料存進暫存表 ( 綠界回來後呼叫的 API , 這裡是中繼站)
        /// </summary>
        /// <param name="request">綠界回傳門市資料</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CvsStoreCallback([FromForm] CvsStoreCallbackRequest request)
        {
            await logisticsService.HandleCvsStoreCallback(request);

            var sessionKey = request.ExtraData;

            return Content(
                $"<script>window.location.href='{fronturl}/product-bought?sessionKey={sessionKey}';</script>",
                "text/html"
            );
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

        /// <summary>
        /// 查看物流暫存訂單資料
        /// </summary>
        /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
        /// <returns>物流暫存訂單資料</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderLogisticsTemp>))]
        public async Task<IActionResult> GetLogisticsTemp([FromQuery] string sessionKey)
        {
            return Ok(await logisticsService.GetLogisticsTemp(sessionKey));
        }
    }
}
