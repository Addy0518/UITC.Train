using Microsoft.Data.SqlClient;

namespace Lab.API.Dapper.Models
{
    // 使用 Dapper 方式的注入連線 , 這樣寫是注入的簡寫
    public class UserConnection(IConfiguration configuration)
    {
        // 新增一個連線方法拿到 Setting 連線
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
