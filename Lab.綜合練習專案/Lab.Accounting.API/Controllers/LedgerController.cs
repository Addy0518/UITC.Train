using Lab.Accounting.API.Infrastructures.Data.Views;
using Lab.API.TODO.Common.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab.Accounting.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ApiResponse<ProblemDetails>)
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest,
        Type = typeof(ApiResponse<Dictionary<string, string[]>>)
    )]
    public class LedgerController(ILedgerService service) : ControllerBase
    {
        // 私有方法 : 從 Token 取出 UserId
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        /// <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="itemname">項目名稱</param>
        ///  <param name="isDelete">刪除狀態</param>
        /// <returns>單筆或多筆項目</returns>
        [HttpGet]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(ApiResponse<List<LedgerItemJoinCategoryView>>)
        )]
        public async Task<IActionResult> GetAllLedger(
            [FromQuery] List<int>? categoryId,
            [FromQuery] DateTime? date,
            [FromQuery] string? itemname,
            [FromQuery] bool? isDelete
        )
        {
            return Ok(
                await service.GetAllLedger(categoryId, date, itemname, isDelete, CurrentUserId)
            );
        }

        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        [HttpGet]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(ApiResponse<LedgerItemJoinCategoryView>)
        )]
        public async Task<IActionResult> GetLedger([FromQuery] int ledgerId)
        {
            return Ok(await service.GetLedger(ledgerId, CurrentUserId));
        }

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param>
        /// <returns>新增的帳本項目</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> CreateLedger([FromBody] LedgerInsertRequest insert)
        {
            insert.UserId = CurrentUserId;
            return Ok(await service.CreateLedger(insert));
        }

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <returns>影響列數</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> UpdateLedger([FromBody] LedgerUpdateRequest update)
        {
            update.UserId = CurrentUserId;
            return Ok(await service.UpdateLedger(update));
        }

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <returns>影響列數</returns>
        [HttpDelete("{ledgerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> DeleteLedger(int ledgerId)
        {
            return Ok(await service.DeleteLedger(ledgerId, CurrentUserId));
        }

        /// <summary>
        /// 刪除所有已軟刪除的帳本項目
        /// </summary>
        /// <returns>影響列數</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> DeleteAllSoftDeleteLedger()
        {
            return Ok(await service.DeleteAllSoftDeleteLedger(CurrentUserId));
        }
    }
}
