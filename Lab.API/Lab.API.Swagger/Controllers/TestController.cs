using System.ComponentModel;
using System.Text.Json.Serialization;
using Lab.API.Swagger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(TestContext context) : ControllerBase
    {
        [HttpGet]
        // Summary 概括
        [EndpointSummary("這是Summary")]
        // Description 詳細描述這支 API
        [EndpointDescription("這是Description")]
        // EndpointName 設定端點名稱
        [EndpointName("FromAttributes")]
        // Tags 是給這個 API 分類 , 讓她分到這個 Tag 的區塊
        [Tags("todos", "projects")]
        // Description 給參數掛上標籤
        public IResult Test([Description("This is a description.")] string name)
        {
            return Results.Ok("Hello");
        }

        [HttpPost]
        // 告訴使用者成功的話會是回傳長什麼樣的狀態與資料類型
        [ProducesResponseType<UserDTO>(
            StatusCodes.Status200OK,
            // 回傳格式
            "application/json",
            // 詳細介紹
            Description = "Returns the requested User item."
        )]
        // 這則是失敗 , 沒規定回傳什麼資料型態的話就會照 Task<ActionResult<User>> 裡的模型當預設
        [ProducesResponseType(
            StatusCodes.Status404NotFound,
            Description = "Requested User item not found."
        )]
        // 預設資料類型
        [ProducesDefaultResponseType(Description = "Undocumented status code.")]
        public async Task<ActionResult<User>> InserUser(UserDTO user)
        {
            var user1 = new User
            {
                Name = user.Name,
                Email = user.Email,
                Role = "User",
                Password = user.Password,
            };
            // 使用 Ef Core Add 新增物件 , 這時候再用 SaveChangesAsync 儲存物件的變更
            context.Add(user1);
            await context.SaveChangesAsync();
            return Ok(user1);
        }

        /// <summary>
        /// 查看列舉值
        /// </summary>
        /// <param name="num">列舉值參數</param>
        /// <returns>是否正確跟在列舉裡的排名</returns>
        /// /// <remarks>
        /// 範例請求 :
        ///
        ///     Get / Test / enum
        ///     {
        ///        "num" : 1
        ///     }
        ///
        /// </remarks>
        /// <response code="200">回傳查到的物件</response>
        /// <response code="400">如果物件是空的</response>
        [HttpGet("enum")]
        public string GetEnum([FromQuery] DayOfTheWeekAsString num)
        {
            // 看外面丟的是否等於列舉裡的 Moday
            bool number = num.HasFlag(DayOfTheWeekAsString.Monday);
            // 回傳是否正確跟在列舉裡的排名
            return $"{number},{(int)num}";
        }

        [JsonConverter(typeof(JsonStringEnumConverter<DayOfTheWeekAsString>))]
        public enum DayOfTheWeekAsString
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
        }
    }
}
