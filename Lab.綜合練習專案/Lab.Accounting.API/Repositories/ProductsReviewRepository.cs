namespace Lab.Accounting.API.Repositories
{
    public class ProductsReviewRepository(DBConnecting connecting) : IProductsReviewRepository
    {
        /// <summary>
        /// 查看商品審核
        /// </summary>
        /// <param name="reviewId">審核表 ID </param>
        /// <returns>審核資訊</returns>
        public async Task<Review> GetProductsReview(int reviewId)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"Select *,u.UserName From dbo.ProductsReview r
                  Left Join [User] u on r.SellerId = u.UserId
                  Where r.ProductsReviewId = @ProductsReviewId  
                 ";

            return await conn.QuerySingleAsync<Review>(sql, new { ProductsReviewId = reviewId });
        }

        /// <summary>
        /// 查看所有商品審核
        /// </summary>
        /// <param name="request">審核表搜尋請求</param>
        /// <returns>審核資訊</returns>
        public async Task<IEnumerable<Review>> GetAllProductsReview(ProductsRiviewSearchRequest request)
        {
            using var conn = connecting.CreateConnecting();
            int offset = request.pageIndex * request.pageSize;

            var sql =
                @"Select *,u.UserName, Count(*) over() as TotalCount From dbo.ProductsReview r
                  Left Join [User] u on r.SellerId = u.UserId
                  Where 
                  (@ReviewStatus is null or r.ReviewStatus = @reviewStatus)
                  and (@sellerId is null or r.SellerId = @sellerId)
                  and  (@keyWords is null 
                         or  u.UserName like '%' + @keyWords + '%'                         
                         or  r.ProductsName like '%' + @keyWords + '%')

                  Order By 
                  case when @sortBy='CreateTime' and @sortOrder='asc' then r.CreateTime end asc,
                  case when @sortBy='CreateTime' and @sortOrder='desc' then r.CreateTime end desc,
                  case when @sortBy='ReviewTime' and @sortOrder='asc' then r.ReviewTime end asc,
                  case when @sortBy='ReviewTime' and @sortOrder='desc' then r.ReviewTime end desc,
                  r.ProductsReviewId
                  Offset @offset Rows Fetch Next @pageSize Rows Only";

            return await conn.QueryAsync<Review>(
                sql,
                new
                {
                    offset = offset,
                    pageSize = request.pageSize,
                    SellerId = request.sellerId,
                    keyWords = request.keyWords,
                    reviewStatus = request.ReviewStatus,
                    sortBy = request.sortBy,
                    sortOrder = request.sortOrder,
                }
            );
        }

        /// <summary>
        /// 新增商品審核
        /// </summary>
        /// <param name="productsReview">賣家商品資訊</param>
        /// <returns>審核表 ID </returns>
        public async Task<int> CreateInsertProductsReview(ProductsReview productsReview)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"INSERT INTO dbo.ProductsReview 
                (
                    ProductsId, 
                    SellerId,
                    ProductsName,
                    ProductsPrice,
                    ProductsStock,
                    ProductsDescription,
                    ProductCategoryId,
                    ReviewStatus,
                    CreateTime
                )
                VALUES 
                (          
                    @ProductsId,
                    @SellerId,                             
                    @ProductsName,
                    @ProductsPrice,
                    @ProductsStock,
                    @ProductsDescription,
                    @ProductCategoryId,
                    @ReviewStatus,                                     
                    GETDATE()            
             
                );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

            return await conn.QuerySingleAsync<int>(sql, productsReview);
        }

        /// <summary>
        /// 審核通過或駁回
        /// </summary>
        /// <param name="request">商品審核請求</param>
        /// <returns>影響列數</returns>
        public async Task<int> ApproveOrRejectProductsReview(ProductsRivewRequest request)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"Update dbo.ProductsReview
                  Set 
                      AdminId=@AdminId,
                      ReviewStatus = @ReviewStatus,
                      NotPassReson=Case when @ReviewStatus = 2 then @NotPassReson else null end,
                      ReviewTime = GETDATE()
                  Where ProductsReviewId = @ProductsReviewId";

            return await conn.ExecuteAsync(sql, request);
        }
    }
}
