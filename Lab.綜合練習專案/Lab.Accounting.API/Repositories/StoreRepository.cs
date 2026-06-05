using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public class StoreRepository(DBConnecting connecting) : IStoreRepository
    {
        /// <summary>
        /// 賣場註冊
        /// </summary>
        /// <param name="seller">註冊資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> StoreRegister(Store seller)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO [dbo].[seller]
                            (userid,
                             sellername,
                             sellerunifiednumber,
                             sellercompanyname,
                             createtime,
                             updatetime,
                             IsDelete)
                VALUES      (@userid,
                             @SellerName,
                             @SellerUnifiedNumber,
                             @SellerCompanyName,
                             GetDate(),
                             GetDate(),
                             @IsDelete)

                SELECT Cast(@@ROWCOUNT AS INT) ";

            return await conn.ExecuteAsync(sql, seller);
        }

        /// <summary>
        /// 取得賣場資訊
        /// </summary>
        /// <param name="sellerId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        public async Task<Store> GetStore(int sellerId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select * From Store
                where UserId = @UserId";

            return await conn.QueryFirstOrDefaultAsync<Store>(sql, new { UserId = sellerId });
        }

        /// <summary>
        /// 編輯賣場資訊
        /// </summary>
        /// <param name="request">編輯資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> UpdateStore(StoreUpdateRequest request)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"UPDATE [dbo].[seller]
                  SET    sellername          = COALESCE(@SellerName, SellerName),
                         sellerunifiednumber = COALESCE(@SellerUnifiedNumber, SellerUnifiedNumber),
                         sellercompanyname   = COALESCE(@SellerCompanyName, SellerCompanyName),
                         updatetime          = GetDate()
                  WHERE  userid = @UserId";

            return await conn.ExecuteAsync(sql, request);
        }
    }
}
