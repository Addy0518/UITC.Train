namespace Lab.Accounting.API.Repositories;

public class ProductsShoppingCarRepository(DBConnecting connecting) : IProductsShoppingCarRepository
{
    /// <summary>
    /// 查看購物車中的所有商品
    /// </summary>
    /// <param name="userId">使用者 Id</param>
    /// <returns>購物車中的所有商品</returns>
    public async Task<IEnumerable<ProductDetails>> GetAllProductsInShoppingCar(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT         m.productsid, 
                                 m.userid,
                                 m.productsname,
                                 m.productsprice,
                                 m.ProductCategoryId,
                                 m.ProductsStock,
                                 s.boughtquantity,
                                 STRING_AGG(c.productcategoryname, ',') as Productcategoryname
                        FROM     product m
                        JOIN     productcategory c
                        ON       c.productcategoryid=m.ProductCategoryId                       
                        JOIN     shoppingcar s
                        ON       m.productsid = s.productsid
                WHERE  s.userid = @UserId and m.IsDelete = 0
                GROUP BY 
                               m.productsid,
                               m.userid,
                               m.productsname,
                               m.ProductCategoryId,
                               m.productsprice,
                               m.ProductsStock,   
                               s.boughtquantity";

        return await conn.QueryAsync<ProductDetails>(sql, new { UserId = userId });
    }

    /// <summary>
    /// 新增單一商品到購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <param name="boughtquantity">購買數量</param>
    /// <returns>影響列數</returns>
    public async Task<int> AddProductsInShoppingCar(int productsId, int userId, int boughtquantity)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"IF EXISTS (
                    SELECT 1 FROM shoppingcar
                    WHERE productsid = @productsid AND userid = @userid
                )
                    UPDATE shoppingcar
                    SET boughtquantity = boughtquantity + @boughtquantity
                    WHERE productsid = @productsid AND userid = @userid
                ELSE
                    INSERT INTO shoppingcar (userid, productsid, boughtquantity)
                    VALUES (@userid, @productsid, @boughtquantity)

                SELECT CAST(@@ROWCOUNT AS INT)";

        return await conn.QuerySingleAsync<int>(
            sql,
            new
            {
                UserId = userId,
                ProductsId = productsId,
                boughtquantity = boughtquantity,
            }
        );
    }

    /// <summary>
    /// 刪除單一商品從購物車
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="userId">使用者 Id</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteProductsInShoppingCar(int productsId, int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Delete From shoppingcar
                Where productsid = @ProductsId and userid = @UserId";

        return await conn.ExecuteAsync(sql, new { ProductsId = productsId, UserId = userId });
    }
}
