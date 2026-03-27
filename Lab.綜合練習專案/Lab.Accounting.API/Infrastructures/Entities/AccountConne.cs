namespace Lab.Accounting.API.Infrastructures.Entities
{
    public class AccountConne(IConfiguration configuration)
    {
        //管理連線
        public SqlConnection CreateConnec() =>
            new SqlConnection(configuration.GetConnectionString("AccountConne"));
    }
}
