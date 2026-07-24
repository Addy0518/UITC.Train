using Lab.Accounting.API.Common.Requests.Notification;
using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;
using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        /// <summary>
        /// 查看所有通知紀錄
        /// </summary>
        /// <param name="request">通知搜尋請求</param>
        /// <returns>所有通知訊息</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<NotificationResponse>))]
        public async Task<IActionResult> GetAllNotifications([FromBody] NotificationSearchRequest request)
        {
            request.UserId = CurrentUserId;
            return Ok(await notificationService.GetAllNotifications(request));
        }

        /// <summary>
        /// 查看單一通知紀錄
        /// </summary>
        /// <param name="notificationId">通知 ID</param>
        /// <returns>單一通知訊息</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> GetNotification([FromQuery] int notificationId)
        {
            return Ok(await notificationService.GetNotification(notificationId, CurrentUserId));
        }
    }
}
