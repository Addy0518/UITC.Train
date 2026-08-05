using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class ChatController(IChatRepository chatRepository) : ControllerBase
{
    // 私有方法 : 從 Token 取出 UserId
    private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

    /// <summary>
    /// 取得聊天對象列表
    /// </summary>
    /// <returns>聊天過的用戶 ID 列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<int>>))]
    public async Task<IActionResult> GetChatUserList()
    {
        var result = await chatRepository.GetChatUserList(CurrentUserId);
        return Ok(ApiResponseHelper.Success(result));
    }

    /// <summary>
    /// 取得與某用戶的歷史訊息
    /// </summary>
    /// <param name="targetUserId">聊天對象的 UserId</param>
    /// <returns>歷史訊息列表</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ChatMessage>>))]
    public async Task<IActionResult> GetMessageHistory([FromQuery] int targetUserId)
    {
        var result = await chatRepository.GetMessageHistory(CurrentUserId, targetUserId);
        return Ok(ApiResponseHelper.Success(result));
    }
}
