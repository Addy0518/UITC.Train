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
                @"INSERT INTO [dbo].Store
                            (userid,
                             Storename,
                             createtime,
                             updatetime,
                             IsDelete)
                VALUES      (@userid,
                             @StoreName,
                             @CreateTime,
                             @UpdateTime,
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
        /// 通過審核正式成立帳號
        /// </summary>
        /// <param name="seller">公司資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> StoreUpdateToCompany(StoreCompanyReview seller)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Update Store
                   SET   StoreUnifiedNumber = @StoreUnifiedNumber,
                         StoreCompanyName   = @StoreCompanyName,
                         DocumentPath       = @DocumentPath,
                         updatetime         = @UpdateTime
                WHERE    userid = @UserId and StoreId=@StoreId";

            return await conn.ExecuteAsync(
                sql,
                new
                {
                    StoreUnifiedNumber = seller.StoreUnifiedNumber,
                    StoreCompanyName = seller.StoreCompanyName,
                    DocumentPath = seller.DocumentPath,
                    UpdateTime = DateTime.Now,
                    UserId = seller.UserId,
                    StoreId = seller.StoreId,
                }
            );
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
                @"UPDATE [dbo].[Store]
                  SET    StoreName          = COALESCE(@StoreName, StoreName),
                         StoreCompanyName   = COALESCE(@StoreCompanyName, StoreCompanyName),
                         updatetime          = GetDate()
                  WHERE  userid = @UserId and StoreId=@StoreId";

            return await conn.ExecuteAsync(sql, request);
        }
    }
}
