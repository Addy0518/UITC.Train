using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface
{
    public class StoreReviewRepository(DBConnecting connecting) : IStoreReviewRepository
    {
        /// <summary>
        /// 取得單一賣場審核資訊
        /// </summary>
        /// <param name="reviewId">審核表 ID</param>
        /// <returns>單一賣場審核資訊</returns>
        public async Task<StoreReview> GetStoreReview(int reviewId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select r.*,
                         u.UserName as SellerName,
                         s.UserName as AdminName
                  From dbo.StoreCompanyReview r
                  Left Join [User] u on r.UserId = u.UserId
                  Left Join [User] s on r.AdminId=s.UserId
                  Where r.StoreCompanyReviewId = @StoreCompanyReviewId  
                 ";

            return await conn.QueryFirstOrDefaultAsync<StoreReview>(sql, new { StoreCompanyReviewId = reviewId });
        }

        /// <summary>
        /// 取得賣場審核資訊
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>賣場審核資訊</returns>
        public async Task<IEnumerable<StoreReview>> GetAllStoreReview(StoreRiviewSearchRequest request)
        {
            using var conn = connecting.CreateConnecting();
            int offset = request.pageIndex * request.pageSize;

            var sql =
                @"Select r.*,
                         u.UserName as SellerName,
                         s.UserName as AdminName, 
                         Count(*) over() as TotalCount 
                  From dbo.StoreCompanyReview r
                  Left Join [User] u on r.UserId = u.UserId
                  Left Join [User] s on r.AdminId=s.UserId

                  Where 
                  (@ReviewStatus is null or r.ReviewStatus = @reviewStatus)
                  and (@sellerId is null or r.UserId = @sellerId)
                  and  (@keyWords is null 
                         or  (@searchType='SellerName' and u.UserName like '%' + @keyWords + '%')                         
                         or  (@searchType='StoreCompanyName' and r.StoreCompanyName like '%' + @keyWords + '%')
                         or  (@searchType='StoreUnifiedNumber' and r.StoreUnifiedNumber like '%' + @keyWords + '%')
                  )

                  Order By 
                  case when @sortBy='CreateTime' and @sortOrder='asc' then r.CreateTime end asc,
                  case when @sortBy='CreateTime' and @sortOrder='desc' then r.CreateTime end desc,
                  r.StoreCompanyReviewId
                  Offset @offset Rows Fetch Next @pageSize Rows Only";

            return await conn.QueryAsync<StoreReview>(
                sql,
                new
                {
                    offset = offset,
                    pageSize = request.pageSize,
                    sellerId = request.sellerId,
                    keyWords = request.keyWords,
                    searchType = request.searchType,
                    reviewStatus = request.ReviewStatus,
                    sortBy = request.sortBy,
                    sortOrder = request.sortOrder,
                }
            );
        }

        /// <summary>
        /// 賣場升級成公司帳號審核
        /// </summary>
        /// <param name="request">公司資訊</param>
        /// <returns>審核表 ID</returns>
        public async Task<int> StoreUpdateToCompanyReview(StoreUpdateToCompanyRequest request)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO StoreCompanyReview
                        (StoreId,
                         UserId,
                         StoreCompanyName,
                         StoreUnifiedNumber,
                         ReviewStatus,
                         CreateTime)
                VALUES      
                        (@StoreId,
                        @UserId,
                        @StoreCompanyName,
                        @StoreUnifiedNumber,
                        @ReviewStatus,
                        @CreateTime);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await conn.ExecuteScalarAsync<int>(sql, request);
        }

        /// <summary>
        /// 賣場審核通過或駁回
        /// </summary>
        /// <param name="request">賣場審核請求</param>
        /// <returns>影響列數</returns>
        public async Task<int> ApproveOrRejectStoreReview(StoreReviewRequest request)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"Update dbo.StoreCompanyReview
                  Set 
                      AdminId=@AdminId,
                      ReviewStatus = @ReviewStatus,
                      NotPassReson=Case when @ReviewStatus = 2 then @NotPassReson else null end,
                      ReviewTime = @ReviewTime
                  Where StoreCompanyReviewId = @StoreCompanyReviewId";

            return await conn.ExecuteAsync(sql, request);
        }

        /// <summary>
        /// 上傳公司賣場文件路徑
        /// </summary>
        /// <param name="reviewId">審核表 ID</param>
        /// <param name="path">文件路徑</param>
        /// <returns>影響列數</returns>
        public async Task<int> StoreDocumentUpload(int reviewId, string path)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"UPDATE dbo.StoreCompanyReview
                  SET    DocumentPath        = @DocumentPath
                  WHERE  StoreCompanyReviewId = @StoreCompanyReviewId";

            return await conn.ExecuteAsync(sql, new { DocumentPath = path, StoreCompanyReviewId = reviewId });
        }
    }
}
