using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Lab.Accounting.API.Repositories;

public class ProductsRepository(DBConnecting connecting) : IProductsRepository
{
    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    public async Task<IEnumerable<Product>> GetAllProducts(ProductsSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();

        int offset = request.pageIndex * request.pageSize;
        // Offset 代表要跳過的行數，Fetch Next 代表要取得的行數
        var sql =
            @"SELECT   m.productsid,
                                 m.userid,
                                 m.productsname,
                                 m.productsprice,
                                 m.ProductsStock,
                                 m.ProductCategoryId,
                                 c.productcategoryname,
                                 parent.ProductCategoryId as ProductParentId,
                                 parent.productcategoryname as ParentCategoryName
                        FROM     mallproducts m
                        JOIN     mallproductcategory c
                        ON       c.productcategoryid= m.ProductCategoryId
                        LEFT JOIN mallproductcategory parent    
                        ON       parent.productcategoryid = c.ProductParentId
                        Where (@UserId is null or m.userId=@UserId) 
                        and  m.isDelete=0
                        and  m.ProductsStock > 0
                        and  (@productCategoryId is null or c.ProductParentId=@productCategoryId or m.ProductCategoryId=@productCategoryId)
                        and  (@keyWords is null or  m.productsname like '%' + @keyWords + '%')
                        ORDER BY productsid offset @offset rows FETCH next @pageSize rows only";

        var result = await conn.QueryAsync<Product>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                UserId = request.sellerId,
                productCategoryId = request.productCategoryId,
                keyWords = request.keyWords,
            }
        );
        return result;
    }

    /// <summary>
    /// 賣家查看自己的所有商品
    /// </summary>
    ///  <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    public async Task<IEnumerable<Product>> SellerGetAllProducts(ProductsSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();

        int offset = request.pageIndex * request.pageSize;
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
                        and  (@IsDelete is null or m.IsDelete=@IsDelete) 
                        ORDER BY productsid offset @offset rows FETCH next @pageSize rows only";

        var result = await conn.QueryAsync<Product>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                UserId = request.sellerId,
                IsDelete = request.isDelete,
            }
        );
        return result;
    }

    /// <summary>
    /// 查看單一商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    public async Task<Product> GetProducts(int productId)
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
                               c.ProductParentId,
                               parent.productcategoryname as parentcategoryname
                        FROM   mallproducts m
                        left JOIN mallproductcategory c
                            ON c.productcategoryid =  m.ProductCategoryId
                        left JOIN mallproductcategory parent
                            ON parent.productcategoryid =  c.ProductParentId
                        WHERE  m.ProductsId = @ProductsId";

        var result = await conn.QueryFirstOrDefaultAsync<Product>(sql, new { ProductsId = productId });

        return result;
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
    /// 檢查商品名稱重複
    /// </summary>
    /// <param name="productsName">商品名稱</param>
    /// <param name="sellerId">賣家 ID </param>
    /// <param name="productId">商品 ID </param>
    /// <returns>影響列數</returns>
    public async Task<bool> ExistsProductsName(string productsName, int sellerId, int? productId = null)
    {
        using var conn = connecting.CreateConnecting();
        // 檢查資料表內有無重複的資料 , 有就回傳 true , 無就 false
        // 如果是更新商品就要把 productid 是自己的排除在外 , 不然沒更新商品名稱的話就會跟原本的自己重複
        var sql =
            @"Select Case When Exists 
              ( 
                Select 1 From MallProducts 
                Where  ProductsName=@ProductsName
                AND    UserId=@SellerId
                AND    ( @ProductId is Null or ProductsId != @ProductId )
              ) Then 1 Else 0
              End ";

        return await conn.ExecuteScalarAsync<bool>(
            sql,
            new
            {
                ProductsName = productsName,
                SellerId = sellerId,
                ProductId = productId,
            }
        );
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
    /// 計算商品數量
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>影響列數</returns>
    public async Task<int> CountProducts(ProductsSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"SELECT COUNT(*) FROM mallproducts m
            JOIN mallproductcategory c ON c.productcategoryid = m.ProductCategoryId
            WHERE m.isDelete = 0 AND m.ProductsStock > 0
            AND (@productCategoryId is null 
                    or c.ProductParentId = @productCategoryId 
                    or m.ProductCategoryId = @productCategoryId)";
        return await conn.ExecuteScalarAsync<int>(sql, new { productCategoryId = request.productCategoryId });
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

        return await conn.ExecuteScalarAsync<int>(sql, new { SellerId = sellerId });
    }
}
