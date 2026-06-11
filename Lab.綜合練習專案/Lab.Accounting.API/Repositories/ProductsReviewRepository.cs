using Lab.Accounting.API.Common.Requests.Products;

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
                @"Select r.*,
                         u.UserName as SellerName,
                         s.UserName as AdminName,
                         c.ProductCategoryName
                  From dbo.ProductReview r
                  Left Join [User] u on r.SellerId = u.UserId
                  Left Join [User] s on r.AdminId=s.UserId
                  LEFT JOIN ProductCategory c ON r.ProductCategoryId = c.ProductCategoryId
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
                @"Select r.*,
                         u.UserName as SellerName,
                         s.UserName as AdminName, 
                         Count(*) over() as TotalCount 
                  From dbo.ProductReview r
                  Left Join [User] u on r.SellerId = u.UserId
                  Left Join [User] s on r.AdminId=s.UserId

                  Where 
                  (@ReviewStatus is null or r.ReviewStatus = @reviewStatus)
                  and (@sellerId is null or r.SellerId = @sellerId)
                  and  (@keyWords is null 
                         or  (@searchType='SellerName' and u.UserName like '%' + @keyWords + '%')                         
                         or  (@searchType='ProductsName' and r.ProductsName like '%' + @keyWords + '%')
                         or  (@searchType='ProductsReviewId' and r.ProductsReviewId=TRY_CAST(@keyWords as int))
                  )

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
                    searchType = request.searchType,
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
        public async Task<int> CreateInsertProductsReview(ProductReview productsReview)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"INSERT INTO dbo.ProductReview 
                (
                    ProductsId, 
                    SellerId,
                    ProductsName,
                    ProductsPrice,
                    ProductsStock,
                    ProductsDescription,
                    ProductCategoryId,
                    Discount,
                    DiscountStart,
                    DiscountEnd,
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
                    @Discount,
                    @DiscountStart,
                    @DiscountEnd,
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
                @"Update dbo.ProductReview
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
