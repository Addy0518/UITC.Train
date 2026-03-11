using Microsoft.Data.SqlClient;

namespace Lab.API.TODO.Infrastructures.Data;

public class TestConnection(IConfiguration configuration)
{
    // 管理連線
    public SqlConnection CreateConnection() =>
        new SqlConnection(configuration.GetConnectionString("TestConne"));
}
