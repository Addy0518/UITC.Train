using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab.API.Template.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : Controller
    {
        private readonly TodoContext _context;

        // DI注入,在建構時決定要注入什麼,因為是剛開始注入而以所以可以用readonly防止被改動
        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // Get : api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems(int page,int pagenumber)
        {

            return await _context.TodoItems
                         //  toDTO是我寫的私有方法,我放在下面
                         .Select(x =>ToDTO(x))
                         // 用Skip跟Take控制顯示數量(分頁)
                         .Skip(pagenumber)
                         .Take(page)
                         .ToListAsync();

        }




        // Get : api/TodoItems/id
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(int id)
        {
            // Find找到指定的資料
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return ToDTO(item);
        }

        // Post : api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItem item)
        {
           
            _context.TodoItems.Add(item);   
            // Add 標記完之後 SaveChangesAsync 非同步儲存所有改變
            await _context.SaveChangesAsync();

            // CreatedAtAction:(string,object,object)=>(1 . URL的名稱 2 . ID  3 . 物件)
            // 創建完成呼叫 get 資料回傳
            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);

        }


        // Put : api/TodoItems/
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItemDTO>> PutTodoItem(int id, TodoItemDTO dto)
        {
           

            // 確認有沒有這筆資料
            var item = await _context.TodoItems.FindAsync(id);
            if (!ItemExists(id) || item==null)
            {
                return NotFound();
            }


            item.Name = dto.Name;
            item.isComplete = dto.isComplete;


            try
            {
                await _context.SaveChangesAsync();

            }
            // 抓取所有例外錯誤
            catch (Exception ex)
            {

                throw new Exception($"錯誤 : {ex.Message}");

            }

            return Ok(item);


        }


        // Delete : api/TodoItems/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var items = await _context.TodoItems.FindAsync(id);
            if (items == null)
            {

                return NotFound();
            }
            // 用Remove刪除記憶體追蹤的物件
            _context.Remove(items);
            // 再儲存
            await _context.SaveChangesAsync();

            // 不用回傳任何東西
            return NoContent();

        }


        // 確認資料是否存在的私有方法
        private bool ItemExists(int id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        // 投影DTO的私有方法
        private static TodoItemDTO ToDTO(TodoItem item) =>
        new TodoItemDTO
        {
            Id = item.Id,
            Name = item.Name,
            isComplete = item.isComplete,
        };




    }
}
