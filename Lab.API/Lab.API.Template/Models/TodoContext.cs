using Microsoft.EntityFrameworkCore;

namespace Lab.API.Template.Models
{
    public class TodoContext : DbContext
    {
        // 定義跟資料庫溝通的行為
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options) { }

        // DbSet則是用來查詢跟儲存,把LINQ查詢傳成資料庫查詢
        public DbSet<TodoItem> TodoItems { get; set; } = null;
    }
}
