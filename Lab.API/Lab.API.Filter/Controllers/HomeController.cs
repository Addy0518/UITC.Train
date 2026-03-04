using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Filter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizationFilter]
    public class HomeController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public HomeController(TodoContext context)
        {
            _todoContext = context;
        }

        [HttpGet]
        [MyLogging("Andy")]
        public void Index()
        {
            Response.WriteAsync("Hello World! \r\n");
        }

        [HttpGet("Error")]
        public void Error()
        {
            throw new System.Exception("Error");
        }

        // Get : api/TodoItems/id
        [HttpGet("{id}")]
        [MyLogging("Andy")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            // Find找到指定的資料
            var item = await _todoContext.TodoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // Post : api/TodoItems
        [HttpPost("Todo")]
        [MyLogging("Andy")]
        public async Task<ActionResult<TodoItem>> PostTodoItem([FromBody] TodoItem item)
        {
            _todoContext.TodoItems.Add(item);
            // Add 標記完之後 SaveChangesAsync 非同步儲存所有改變
            await _todoContext.SaveChangesAsync();

            // CreatedAtAction:(string,object,object)=>(1 . URL的名稱 2 . ID  3 . 物件)
            // 創建完成呼叫 get 資料回傳
            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }
    }
}
