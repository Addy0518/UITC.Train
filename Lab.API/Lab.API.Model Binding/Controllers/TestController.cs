using Lab.API.Model_Binding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Lab.API.Model_Binding.Models.TodoItem;

namespace Lab.API.Model_Binding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public TestController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        // Get : api/Test/id

        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem([FromRoute] int id) // 從路由得值查詢
        {
            // Find找到指定的資料
            var item = await _todoContext.TodoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // Post : api/Test
        [HttpPost]
        public async Task<IActionResult> TodoItems(TodoItem item)
        {
            // 資料驗證未過則回傳錯誤訊息
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _todoContext.Add(item);
            await _todoContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }
    }
}
