using Lab.API.Dapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lab.API.Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(TestContext context) : ControllerBase
    {
        // 新增( SaveChanges )
        [HttpPost("SaveChanges")]
        public async Task<IActionResult> InsertBook([FromBody] BookDTO bookdto)
        {
            using var tran = await context.Database.BeginTransactionAsync();
            try
            {
                // 先把 DTO 的投影到 Model
                var book = new Book
                {
                    UserId = bookdto.UserId,
                    BookName = bookdto.BookName,
                    BookPrice = bookdto.BookPrice,
                };
                // 使用 Ef Core Add 新增物件 , 這時候再用 SaveChangesAsync 儲存物件的變更
                context.Add(book);
                await context.SaveChangesAsync();

                // 可以在交易過程設置儲存點 , 這樣當交易失敗時可以不用返回一開始而是返回指定點
                await tran.CreateSavepointAsync("原點!");

                await tran.CommitAsync();
                return Ok(book);
            }
            catch (Exception ex)
            {
                await tran.RollbackToSavepointAsync("原點!");
                throw;
            }
        }

        // 更新( Update )
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateBook([FromBody] BookDTO bookdto)
        {
            // 一般的寫法跟新增差不多 , 改成追蹤 Id 更新原有物件
            var targetbook = await context.Books.FindAsync(bookdto.BookId);
            if (targetbook == null)
                return NotFound();
            targetbook.UserId = bookdto.UserId;
            targetbook.BookName = bookdto.BookName;
            targetbook.BookPrice = bookdto.BookPrice;
            // 使用 Attach 標記為 Unchanged , 這時候還沒更新
            context.Attach(targetbook);
            // 變更追蹤狀態為更新
            context.Entry(targetbook).State = EntityState.Modified;
            // 儲存
            await context.SaveChangesAsync();

            return Ok(targetbook);
        }

        // 更新( ExecuteUpdate )
        [HttpPut("ExecuteUpdate")]
        public async Task<IActionResult> ExecuteUpdateBook()
        {
            using var tran = await context.Database.BeginTransactionAsync();
            try
            {
                // 改成用 ExecuteUpdate , 這能更快速的批次更新 , 因為它直接跳過記憶體追蹤 , 必須要確定要更新什麼物體
                // 所以用交易包起來 , 失敗就直接返回原點
                var result = await context
                    .Books.Where(x => x.UserId == 60012)
                    // SetProperty 就能更新指定屬性
                    .ExecuteUpdateAsync(x => x.SetProperty(x => x.BookName, x => "阿陳的書"));

                await tran.CommitAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        // 查看單筆
        [HttpGet("One")]
        public async Task<Book> GetBook([FromQuery] int id)
        {
            return await context.Books.SingleAsync(x => x.BookId == id);
        }

        //查看多筆
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks(int page, int pagenumber)
        {
            return await context
                .Books.Select(x => new BookDTO
                {
                    BookId = x.BookId,
                    BookName = x.BookName,
                    BookPrice = x.BookPrice,
                })
                // 用Skip跟Take控制顯示數量(分頁)
                .Skip(pagenumber - 1)
                .Take(page)
                .ToListAsync();
        }

        // 刪除 ( Remove )
        [HttpDelete("Remove")]
        public async Task<bool> DeleteBookRemove([FromQuery] int id)
        {
            var book = await context.Books.FindAsync(id);
            if (book != null)
            {
                // Remove 刪除之後 Save 儲存追蹤變更
                context.Books.Remove(book);
                context.SaveChanges();
                return true;
            }

            return false;
        }

        // 刪除 ( ExecuteDelete )
        [HttpDelete("ExecuteDelete")]
        public async Task<bool> DeleteBookExecuteDelete()
        {
            using var tran = await context.Database.BeginTransactionAsync();
            try
            {
                // 跟 ExecuteUpdate 一樣能高效刪除但會跳過追蹤 , 所以交易包起來
                await context.Books.Where(x => x.UserId == 60012).ExecuteDeleteAsync();
                await tran.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        // 刪除使用者跟書
        [HttpDelete("RemoveUserAndBooks")]
        public async Task<bool> DeleteBookAndUser([FromQuery] int id)
        {
            await using var tran = await context.Database.BeginTransactionAsync();
            try
            {
                await context.Books.Where(x => x.UserId == id).ExecuteDeleteAsync();
                var user = await context.Users.FindAsync(id);
                if (user != null)
                {
                    context.Users.Remove(user);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }
            return false;
        }

        // 更新用戶 ( Attach 測試 )
        [HttpPut("UpdateUser")]
        public async Task UpdateUser(int userId)
        {
            // 只給一個 Pk 值
            var user = new User() { Id = userId };

            context.Attach(user);
            // 設定其他屬性
            user.Name = "ANdyyyyy";
            // 這樣就可以騙過 Ef , 跳過查詢步驟了
            await context.SaveChangesAsync();
        }

        private class Reviewer_BankTrace
        {
            public int bookId { get; set; }

            public int userId { get; set; }
        }
    }
}
