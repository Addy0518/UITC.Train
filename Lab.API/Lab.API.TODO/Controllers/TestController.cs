using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.TODO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(ITestService testservice) : ControllerBase
    {
        /// <summary>
        /// 單筆取得 Users
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>單個 Users </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<User>))]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            return Ok(await testservice.GetUserAsync(id));
        }

        /// <summary>
        /// 多筆取得 Users
        /// </summary>
        /// <returns>所有 Users </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<User>>))]
        public async Task<IActionResult> GetAGetAllUsersAsync()
        {
            return Ok(await testservice.GetAllUsersAsync());
        }

        /// <summary>
        /// 更新 User
        /// </summary>
        /// <param name="update">更新 User 請求</param>
        /// <returns>影響列數</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateRequest update)
        {
            return Ok(await testservice.UpdateUserAsync(update));
        }

        /// <summary>
        /// 新增 User
        /// </summary>
        /// <param name="request">新增 User 請求</param>
        /// <returns>新增 User</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> InsertUserAsync([FromBody] InsertRequest request)
        {
            return Ok(await testservice.InsertUserAsync(request));
        }

        /// <summary>
        /// 刪除 User
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>影響列數</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            return Ok(await testservice.DeleteUserAsync(id));
        }
    }
}
