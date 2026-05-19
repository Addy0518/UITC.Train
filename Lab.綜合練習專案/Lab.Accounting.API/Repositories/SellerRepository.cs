namespace Lab.Accounting.API.Repositories.Interface
{
    public class SellerRepository(DBConnecting connecting) : ISellerRepository
    {
        /// <summary>
        /// 賣家註冊
        /// </summary>
        /// <param name="seller">註冊資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> SellerRegister(Seller seller)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO [dbo].[seller]
                            (userid,
                             sellername,
                             sellerunifiednumber,
                             sellercompanyname,
                             createtime,
                             updatetime)
                VALUES      (@userid,
                             @SellerName,
                             @SellerUnifiedNumber,
                             @SellerCompanyName,
                             @CreateTime,
                             @UpdateTime)

                SELECT Cast(@@ROWCOUNT AS INT) ";

            return await conn.ExecuteAsync(sql, seller);
        }

        /// <summary>
        /// 取得賣家資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣家資訊</returns>
        public async Task<Seller> GetSeller(int sellerId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select * From Seller
                where SellerId = @SellerId";

            return await conn.QueryFirstOrDefaultAsync<Seller>(sql, new { SellerId = sellerId });
        }
    }
}
