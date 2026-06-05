using Lab.Accounting.API.Common.Requests.Order;

namespace Lab.Accounting.API.Controllers.Admin;

[Tags("Admin-Order")]
[Route("api/admin/[controller]/[action]")]
[ApiController]
[Authorize(Roles = RolesAuth.管理者)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
public class OrderController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// 查看所有訂單
    /// </summary>
    /// <param name="request">訂單搜尋請求</param>
    /// <returns>所有訂單資訊</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<OrderResponse>>))]
    public async Task<IActionResult> GetAllOrder([FromQuery] OrderSearchRequest request)
    {
        return Ok(await orderService.GetAllOrder(request));
    }
}
