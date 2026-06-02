namespace Lab.Accounting.API.Repositories
{
    public class ProductsReviewRepository(DBConnecting connecting) : IProductsReviewRepository
    {
        /// <summary>
        /// 新增審核
        /// </summary>
        /// <param name="request">賣家商品資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> CreateProductsReview(ProductsInsertRequest request)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"Insert Into [ProductsReview] (
                  UserName, UserAccount, UserPassword, UserPhone,UserAddress,CreateTime,UpdateTime,IsDelete
                ) 
                values 
                  (
                    @UserName, @UserAccount, @UserPassword, 
                    @UserPhone,@UserAddress,GetDate(),GetDate(),@IsDelete
                  );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

            return await conn.QuerySingleAsync<UserResponse>(sql, userInformation);
        }
    }
}
