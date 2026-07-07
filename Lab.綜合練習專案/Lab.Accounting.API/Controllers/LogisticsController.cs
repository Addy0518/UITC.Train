using Lab.Accounting.API.Common.Requests.Logistics;
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
        /// 接收綠界物流狀態通知，更新對應物流單的狀態
        /// </summary>
        /// <param name="request">綠界回傳的物流狀態資料</param>
        /// <returns>是否處理成功</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> HandleLogisticsStatusNotify([FromForm] LogisticsStatusCallbackRequest request)
        {
            await logisticsService.HandleLogisticsStatusNotify(request);
            return Content("1|OK");
        }

        /// <summary>
        /// C2C 門市選店時發生異常會打這支
        /// </summary>
        /// <param name="collection">綠界回傳表單資訊</param>
        /// <returns>訊息</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> LogisticsC2CReply([FromForm] IFormCollection collection)
        {
            // 記錄下來，之後可以在賣家畫面提示「門市異常，需要重新選店」
            return Content("1|OK");
        }

        /// <summary>
        /// 接收綠界回傳的門市資料存進暫存表 ( 綠界回來後呼叫的 API , 這裡是中繼站)
        /// </summary>
        /// <param name="request">綠界回傳門市資料</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CvsStoreCallback([FromForm] CvsStoreCallbackRequest request)
        {
            await logisticsService.SaveCvsLogisticsTemp(request);

            var sessionKey = request.ExtraData;

            return Content(
                $"<script>window.location.href='{fronturl}/product-bought?sessionKey={sessionKey}';</script>",
                "text/html"
            );
        }

        /// <summary>
        /// 儲存物流暫存訂單收件人 ( 超商 )
        /// </summary>
        /// <param name="request">收件人資訊</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> SaveCvsReceiver([FromBody] CvsReceiverInsertRequest request)
        {
            return Ok(await logisticsService.SaveCvsReceiver(request));
        }

        /// <summary>
        /// 儲存物流暫存訂單資料 ( 宅配 )
        /// </summary>
        /// <param name="request">物流暫存表單資料</param>
        /// <returns>操作結果</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> SaveHomeLogisticsTemp([FromBody] LogisticsTempInsertRequest request)
        {
            return Ok(await logisticsService.SaveHomeLogisticsTemp(request));
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

        // 測試綠界呼叫用 , 可以刪
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public IActionResult GetCheckMacValueForTest([FromBody] Dictionary<string, string> parameters)
        {
            var checkMacValue = logisticsService.GetCheckMacValueForTest(parameters);
            return Ok(checkMacValue);
        }
    }
}
