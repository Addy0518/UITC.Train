namespace Lab.Accounting.API.Repositories;

public class ProductsRepository(DBConnecting connecting) : IProductsRepository
{
    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <param name="sellerId">賣家 Id</param>
    /// <param name="isDelete">是否為刪除狀態</param>
    /// <returns>商品列表</returns>
    public async Task<IEnumerable<ProductsResponse>> GetAllProducts(
        int pageIndex,
        int pageSize,
        int? sellerId = null,
        IsDeleteStatusEnum? isDelete = IsDeleteStatusEnum.Normal
    )
    {
        using var conn = connecting.CreateConnecting();

        int offset = pageIndex * pageSize;
        // Offset 代表要跳過的行數，Fetch Next 代表要取得的行數
        var sql =
            @"SELECT   m.productsid,
                                 m.userid,
                                 m.productsname,
                                 m.productsprice,
                                 m.ProductsStock,
                                 m.ProductCategoryId,
                                 m.isDelete,
                                 c.productcategoryname
                        FROM     mallproducts m
                        JOIN     mallproductcategory c
                        ON       c.productcategoryid= m.ProductCategoryId
                        Where (@UserId is null or m.userId=@UserId) 
                        and  (@isDelete is null or m.isDelete=@isDelete)
                        and  m.ProductsStock > 0
                        ORDER BY productsid offset @offset rows FETCH next @pageSize rows only";

        var result = await conn.QueryAsync<ProductsResponse>(
            sql,
            new
            {
                offset = offset,
                pageSize = pageSize,
                UserId = sellerId,
                isDelete = isDelete,
            }
        );
        return result;
    }

    /// <summary>
    /// 查看單一商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    public async Task<ProductsResponse> GetProducts(int productId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT m.productsid,
                               m.userid,
                               m.productsname,
                               m.productsprice,
                               m.ProductsStock,
                               m.ProductsDescription,
                               m.ProductCategoryId,
                               m.isDelete,
                               c.productcategoryname,
                               c.productcategoryid,        
                               c.ProductParentId 
                        FROM   mallproducts m
                               left JOIN mallproductcategory c
                                 ON c.productcategoryid =  m.ProductCategoryId
                        WHERE  m.ProductsId = @ProductsId";

        var result = await conn.QueryFirstOrDefaultAsync<ProductsResponse>(sql, new { ProductsId = productId });

        return result;
    }

    /// <summary>
    /// 查看商品類別
    /// </summary>
    /// <param name="productcategoryId">商品類別 ID</param>
    /// <returns>商品類別</returns>
    public async Task<IEnumerable<MallProductCategory>> GetCategory(int? productcategoryId = null)
    {
        using var conn = connecting.CreateConnecting();

        //  第一層 ProductCategoryId 跟 ProductParentId 為 null 的是最頂層的類別
        //  第二層 ( 子 ) ProductParentId = ( 父 ) ProductCategoryId  的就是往下一層的類別
        //  (衣服 => 短袖 , 長袖 => 男士短袖 , 女士短袖 ...)
        var sql =
            @"Select ProductCategoryId,ProductCategoryName,ProductParentId 
                From mallproductcategory 
                Where (@ProductCategoryId is null and ProductParentId is null) 
                OR ProductParentId = @ProductCategoryId";
        return await conn.QueryAsync<MallProductCategory>(sql, new { ProductCategoryId = productcategoryId });
    }

    /// <summary>
    /// 新增單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>影響列數</returns>
    public async Task<int> CreateProducts(MallProducts products)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"INSERT INTO mallproducts
                                    (userid,
                                     productsname,
                                     productsprice,
                                     ProductsStock,
                                     ProductsDescription,
                                     ProductCategoryId,
                                     CreateTime,
                                     UpdateTime,
                                     IsDelete
                                     )
                        VALUES      (@UserId,
                                     @ProductsName,
                                     @ProductsPrice,
                                     @ProductsStock,
                                     @ProductsDescription,
                                     @ProductCategoryId,
                                     GetDate(),
                                     GetDate(),
                                     @IsDelete
                                     ) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
        return await conn.QuerySingleAsync<int>(sql, products);
    }

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="products">商品資訊</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateProducts(MallProducts products)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE mallproducts
                        SET      
                                 productsname = COALESCE(@ProductsName, productsname),
                                 productsprice = COALESCE(@ProductsPrice, productsprice),
                                 ProductsStock = COALESCE(@ProductsStock, ProductsStock),
                                 ProductsDescription = COALESCE(@ProductsDescription, ProductsDescription),
                                 ProductCategoryId = COALESCE(@ProductCategoryId, ProductCategoryId),
                                 UpdateTime    = GetDate()
                        WHERE    productsid = @ProductsId and userId=@UserId;";
        return await conn.ExecuteAsync(sql, products);
    }

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateProductsDeleteStatus(int sellerId, IEnumerable<int> productId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"UPDATE mallproducts
                        SET     IsDelete = 0 ,
                                UpdateTime   = GetDate()
                        WHERE   UserId = @UserId
                        And     ProductsId in @ProductsId";
        return await conn.ExecuteAsync(sql, new { ProductsId = productId, UserId = sellerId });
    }

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <param name="isDelete">刪除狀態</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteProducts(int productsId, IsDeleteStatusEnum isDelete, int sellerId)
    {
        using var conn = connecting.CreateConnecting();
        //用 true 跟 false 判斷是否執行硬刪除或是軟刪除
        var deletesql =
            isDelete == IsDeleteStatusEnum.Deleted
                ? @"
                    Delete From ProductImg Where ProductsID=@productsId;
                    Delete From MallProductsRate Where ProductsID=@productsId;
                    Delete From MallShoppingCar Where ProductsID=@productsId;
                    Delete From MallProducts Where ProductsID=@productsId and UserId=@userId;"
                : @"Update MallProducts Set IsDelete=1,UpdateTime=GetDate() Where ProductsID=@productsId and UserId=@userId;
                    ";

        return await conn.ExecuteAsync(deletesql, new { productsId = productsId, userId = sellerId });
    }

    /// <summary>
    /// 設定商品庫存
    /// </summary>
    /// <param name="productsId">商品 Id</param>
    /// <param name="purchaseQuantity">購買數量</param>
    /// <returns>影響列數</returns>
    public async Task<int> SetStock(int productsId, int purchaseQuantity)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Update mallproducts
                    SET 
                          ProductsStock = @purchaseQuantity,
                          UpdateTime   = GetDate()
                    WHERE ProductsId = @productsId ";

        return await conn.ExecuteAsync(sql, new { productsId, purchaseQuantity });
    }

    /// <summary>
    /// 計算賣家所有商品數量
    /// </summary>
    /// <param name="sellerId">賣家 Id</param>
    /// <returns>影響列數</returns>
    public async Task<int> CountSellerProducts(int sellerId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT COUNT(*) FROM mallproducts 
              WHERE UserId = @SellerId AND isDelete = 0";

        return await conn.ExecuteAsync(sql, new { SellerId = sellerId });
    }
}
