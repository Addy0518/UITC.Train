using NPOI.POIFS.Properties;
using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = RolesAuth.賣家)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<ProblemDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Dictionary<string, string[]>>))]
    public class DashBoardController(IDashBoradService dashBoradService) : ControllerBase
    {
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        /// <summary>
        /// 查看賣家所有數據
        /// </summary>
        /// <returns>賣家數據</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DashBoardResponse>))]
        public async Task<IActionResult> GetDashboard()
        {
            return Ok(await dashBoradService.GetDashboard(CurrentUserId));
        }
    }
}
