using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Services;

public class ProductsService(
    IProductsRepository productsRepositories,
    IProductsImgRepository productsImgRepository,
    IProductsRateRepository productsRateRepositories,
    IUserRepository userRepository,
    IProductsOrderRepository productsOrderRepository,
    IProductsRepository productsRepository,
    IWebHostEnvironment env
) : IProductsService
{
    /// <summary>
    /// 賣家查看商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    public async Task<ApiResponse<ProductsResponse>> GetProducts(int productId)
    {
        var target = await productsRepositories.GetProducts(productId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<ProductsResponse>();
        }
        target.ProductsImgs = await productsImgRepository.GetProductsAllImg(productId);

        target.ProductsAVGRate = await productsRateRepositories.CountAVGProductRate(productId) ?? 0;

        target.ProductsAllRates = await productsRateRepositories.GetProductRate(productId);

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="pageIndex">頁碼</param>
    /// <param name="pageSize">每頁顯示數量</param>
    /// <param name="userId">使用者 Id</param>
    /// <param name="isDelete">是否為刪除狀態</param>
    /// <returns>商品列表</returns>
    public async Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(
        int pageIndex,
        int pageSize,
        int? userId = null,
        IsDeleteStatusEnum? isDelete = IsDeleteStatusEnum.Normal
    )
    {
        var products = await productsRepositories.GetAllProducts(pageIndex, pageSize, userId, isDelete);

        if (products == null)
        {
            return ApiResponseHelper.NotFound<IEnumerable<ProductsResponse>>();
        }

        // 開兩條執行緒同時查詢
        var tasks = products.Select(async product =>
        {
            product.ProductsAVGRate = await productsRateRepositories.CountAVGProductRate(product.ProductsId) ?? 0;
            product.ProductsImgs = await productsImgRepository.GetProductsAllImg(product.ProductsId);
        });
        await Task.WhenAll(tasks);
        return ApiResponseHelper.Success(products);
    }

    /// <summary>
    /// 查看商品類別
    /// </summary>
    /// <param name="productcategoryId">商品類別 ID</param>
    /// <returns>商品類別</returns>
    public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetCategory(int? productcategoryId = null)
    {
        var target = await productsRepositories.GetCategory(productcategoryId);
        if (target == null)
        {
            return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="productsInsertRequest">商品資訊</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> CreateProducts(ProductsInsertRequest productsInsertRequest)
    {
        var product = new MallProducts
        {
            ProductsName = productsInsertRequest.ProductsName,
            ProductsPrice = productsInsertRequest.ProductsPrice,
            ProductsStock = productsInsertRequest.ProductsStock,
            ProductCategoryId = productsInsertRequest.ProductCategoryId,
            UserId = productsInsertRequest.UserId,
            IsDelete = IsDeleteStatusEnum.Normal,
        };
        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var target = await productsRepositories.CreateProducts(product);
            if (target <= 0)
                return ApiResponseHelper.InternalException<int>("商品新增失敗");

            trxScope.Complete();
            return ApiResponseHelper.Success(target);
        }
    }

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="productsUpdateRequest">商品更新資訊</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> UpdateProducts(ProductsUpdateRequest productsUpdateRequest)
    {
        var updateTarget = new MallProducts
        {
            UserId = productsUpdateRequest.UserId,
            ProductsId = productsUpdateRequest.ProductsId,
            ProductsName = productsUpdateRequest.ProductsName,
            ProductsPrice = productsUpdateRequest.ProductsPrice,
            ProductsStock = productsUpdateRequest.ProductsStock,
            ProductCategoryId = productsUpdateRequest.ProductCategoryId,
        };

        var target = await productsRepositories.GetProducts(productsUpdateRequest.ProductsId);
        if (target == null || target.UserId != productsUpdateRequest.UserId)
        {
            return ApiResponseHelper.NotFound<int>();
        }
        var result = await productsRepositories.UpdateProducts(updateTarget);

        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> UpdateProductsDeleteStatus(int userId, IEnumerable<int> productId)
    {
        var target = await productsRepositories.UpdateProductsDeleteStatus(userId, productId);
        if (target == 0)
        {
            return ApiResponseHelper.NotFound<int>();
        }
        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> DeleteProducts(int productsId, int userId)
    {
        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var target = await productsRepositories.GetProducts(productsId);
            if (target == null || target.UserId != userId)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            IEnumerable<MallProductImg> imgs = new List<MallProductImg>();
            if (target.IsDelete == IsDeleteStatusEnum.Deleted)
            {
                imgs = await productsImgRepository.GetProductsAllImg(productsId);
            }

            var deletetarget = await productsRepositories.DeleteProducts(productsId, target.IsDelete, userId);

            if (deletetarget == null)
                return ApiResponseHelper.InternalException<int>("刪除失敗");

            if (target.IsDelete == IsDeleteStatusEnum.Deleted)
            {
                foreach (var img in imgs)
                {
                    FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", img.ProductsImg);
                }
            }

            trxScope.Complete();
            return ApiResponseHelper.Success<int>(deletetarget);
        }
    }

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="productId">商品 ID</param>
    /// <returns>新增成功的圖片</returns>
    public async Task<ApiResponse<IEnumerable<MallProductImg>>> ProductsImgUpload(
        IFormFile productsImgsFiles,
        int productId
    )
    {
        var product = await productsRepositories.GetProducts(productId);
        if (product == null)
            return ApiResponseHelper.NotFound<IEnumerable<MallProductImg>>();

        var result = await FileUploadHelper.SaveFileAsync(productsImgsFiles, env.WebRootPath, "ProductsImg");
        var imgupload = await productsImgRepository.ProductsImgUpload(productId, result);

        if (imgupload <= 0)
        {
            // DB 失敗，把剛存的實體檔案清掉，避免孤兒檔案
            FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", result);
            return ApiResponseHelper.InternalException<IEnumerable<MallProductImg>>("圖片上傳失敗");
        }

        var newtarget = await productsImgRepository.GetProductsAllImg(productId);
        return ApiResponseHelper.Success(newtarget);
    }

    /// <summary>
    /// 刪除商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>刪除的圖片</returns>
    public async Task<ApiResponse<MallProductImg>> DeleteProductsImg(int productsImgId)
    {
        var result = await productsImgRepository.DeleteProductsImg(productsImgId);
        if (result == null)
        {
            return ApiResponseHelper.NotFound<MallProductImg>();
        }
        FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", result.ProductsImg);
        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="request">商品評價資訊</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> CreateProductRate(ProductsRateRequest request)
    {
        var order = await productsOrderRepository.GetUserOneOrder(request.OrderId, request.UserId);
        if (order == null)
            return ApiResponseHelper.NotFound<int>();

        if (order.ShippingStatus != ShippingStatusEnum.Arrived)
        {
            var errors = new Dictionary<string, string[]> { { "ShippingStatus", new[] { "商品尚未送達,無法評價!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

        var existRate = await productsRateRepositories.GetOrderRate(order.OrderId);

        if (existRate != null)
        {
            var errors = new Dictionary<string, string[]> { { "OrderId", new[] { "這筆訂單已經評價過了!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

        var product = await productsRepository.GetProducts(request.ProductsId);
        if (product.UserId == request.UserId)
        {
            var errors = new Dictionary<string, string[]> { { "UserId", new[] { "賣家無法評價自己的商品!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

        var rate = new MallProductsRate
        {
            UserId = request.UserId,
            ProductsId = request.ProductsId,
            OrderId = request.OrderId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreateTime = DateTime.Now,
        };

        var result = await productsRateRepositories.CreateProductRate(rate);
        return ApiResponseHelper.Success(result);
    }
}
