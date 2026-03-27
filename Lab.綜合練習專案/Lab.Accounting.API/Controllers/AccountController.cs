using Lab.API.TODO.Common.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ApiResponse<ProblemDetails>)
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest,
        Type = typeof(ApiResponse<Dictionary<string, string[]>>)
    )]
    public class AccountController(IAccountService service) : ControllerBase
    {
        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        /// <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="categoryname">項目名稱</param>
        /// <returns>單筆或多筆項目</returns>
        [HttpGet]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(ApiResponse<List<LedgerItem>>)
        )]
        public async Task<IActionResult> GetAllLedger(
            [FromQuery] List<int>? categoryId,
            [FromQuery] DateTime? date,
            [FromQuery] string? itemname
        )
        {
            return Ok(await service.GetAllLedger(categoryId, date, itemname));
        }

        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<LedgerItem>))]
        public async Task<IActionResult> GetLedger([FromQuery] int ledgerId)
        {
            return Ok(await service.GetLedger(ledgerId));
        }

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>新增的帳本項目</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> CreateLedger(
            LedgerInsertRequest insert,
            string categoryname
        )
        {
            return Ok(await service.CreateLedger(insert, categoryname));
        }

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>影響列數</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> UpdateLedger(
            LedgerUpdateRequest update,
            string? categoryname
        )
        {
            return Ok(await service.UpdateLedger(update, categoryname));
        }

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <param name="isDelete">刪除狀態</param>
        /// <returns>影響列數</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> DeleteLedger(int ledgerId)
        {
            return Ok(await service.DeleteLedger(ledgerId));
        }
    }
}
