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
        /// 取得賣場資訊 ( 賣場 ID )
        /// </summary>
        /// <param name="storeId">賣家 ID </param>
        /// <returns>賣場資訊</returns>
        public async Task<Store> GetStorebyStoreId(int storeId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select * From Store
                where StoreId = @StoreId";

            return await conn.QueryFirstOrDefaultAsync<Store>(sql, new { StoreId = storeId });
        }

        /// <summary>
        /// 取得賣場資訊 ( 賣家 ID )
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
                         updatetime         = @UpdateTime
                WHERE    userid = @UserId and StoreId=@StoreId";

            return await conn.ExecuteAsync(
                sql,
                new
                {
                    StoreUnifiedNumber = seller.StoreUnifiedNumber,
                    StoreCompanyName = seller.StoreCompanyName,
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

        /// <summary>
        /// 用戶追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> FollowStore(int userId, int storeId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Insert Into [UserFollowStore] (
                  UserId, StoreId
                ) 
                values 
                  (
                    @UserId, @StoreId
                  );";
            return await conn.ExecuteAsync(sql, new { UserId = userId, StoreId = storeId });
        }

        /// <summary>
        /// 用戶取消追蹤賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> UnfollowStore(int userId, int storeId)
        {
            using var conn = connecting.CreateConnecting();

            var sql = @"Delete From UserFollowStore Where UserId = @UserId And StoreId = @StoreId";
            return await conn.ExecuteAsync(sql, new { UserId = userId, StoreId = storeId });
        }

        /// <summary>
        /// 查看用戶是否已追蹤某賣場
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="storeId">賣場 ID</param>
        /// <returns>是否已追蹤</returns>
        public async Task<bool> IsFollowingStore(int userId, int storeId)
        {
            using var conn = connecting.CreateConnecting();

            var sql = @"Select Count(1) From UserFollowStore Where UserId = @UserId And StoreId = @StoreId";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, StoreId = storeId });
            return count > 0;
        }
    }
}
