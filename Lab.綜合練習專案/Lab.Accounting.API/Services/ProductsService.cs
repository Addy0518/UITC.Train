using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using NPOI.HPSF;
using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Services;

public class ProductsService(
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
    public async Task<ApiResponse<Product>> GetProducts(int productId)
    {
        var target = await productsRepository.GetProducts(productId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<Product>();
        }
        target.ProductsImgs = await productsImgRepository.GetProductsAllImg(productId);

        target.ProductsAVGRate = await productsRateRepositories.CountAVGProductRate(productId) ?? 0;

        target.ProductsAllRates = await productsRateRepositories.GetProductRate(productId);

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    public async Task<ApiResponse<ProductsResponse>> GetAllProducts(ProductsSearchRequest request)
    {
        var products = await productsRepository.GetAllProducts(request);
        var totalCount = await productsRepository.CountProducts(request);
        if (!products.Any())
        {
            return ApiResponseHelper.NotFound<ProductsResponse>();
        }

        // 開兩條執行緒同時查詢
        var tasks = products.Select(async product =>
        {
            product.ProductsAVGRate = await productsRateRepositories.CountAVGProductRate(product.ProductsId) ?? 0;
            product.ProductsImgs = await productsImgRepository.GetProductsAllImg(product.ProductsId);
        });
        await Task.WhenAll(tasks);

        var result = new ProductsResponse { Products = products, TotalCount = totalCount };

        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 賣家查看自己的所有商品
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    public async Task<ApiResponse<ProductsResponse>> SellerGetAllProducts(ProductsSearchRequest request)
    {
        var products = await productsRepository.SellerGetAllProducts(request);

        if (!products.Any())
        {
            return ApiResponseHelper.NotFound<ProductsResponse>();
        }

        // 開兩條執行緒同時查詢
        var tasks = products.Select(async product =>
        {
            product.ProductsAVGRate = await productsRateRepositories.CountAVGProductRate(product.ProductsId) ?? 0;
            product.ProductsImgs = await productsImgRepository.GetProductsAllImg(product.ProductsId);
        });
        await Task.WhenAll(tasks);

        var result = new ProductsResponse { Products = products };
        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 查看商品類別
    /// </summary>
    /// <param name="productcategoryId">商品類別 ID</param>
    /// <returns>商品類別</returns>
    public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetCategory(int? productcategoryId = null)
    {
        var target = await productsRepository.GetCategory(productcategoryId);
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
            ProductsDescription = productsInsertRequest.ProductsDescription,
            ProductCategoryId = productsInsertRequest.ProductCategoryId,
            UserId = productsInsertRequest.UserId,
            IsDelete = IsDeleteStatusEnum.Normal,
        };
        var exists = await productsRepository.ExistsProductsName(
            productsInsertRequest.ProductsName,
            productsInsertRequest.UserId
        );

        if (exists)
        {
            var errors = new Dictionary<string, string[]> { { "ProductsName", new[] { "已有相同名稱的商品!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var target = await productsRepository.CreateProducts(product);
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
            ProductsDescription = productsUpdateRequest.ProductsDescription,
            ProductCategoryId = productsUpdateRequest.ProductCategoryId,
        };
        var exists = await productsRepository.ExistsProductsName(
            productsUpdateRequest.ProductsName,
            productsUpdateRequest.UserId,
            productsUpdateRequest.ProductsId
        );

        if (exists)
        {
            var errors = new Dictionary<string, string[]> { { "ProductsName", new[] { "已有相同名稱的商品!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }
        var target = await productsRepository.GetProducts(productsUpdateRequest.ProductsId);
        if (target == null || target.UserId != productsUpdateRequest.UserId)
        {
            return ApiResponseHelper.NotFound<int>();
        }

        // 比對商品描述的圖片有沒有在資料夾
        var oldImgs = ParseDescriptionImgs(target.ProductsDescription);
        var newImgs = ParseDescriptionImgs(productsUpdateRequest.ProductsDescription);

        // Except 差集 => 顯示舊的有但新的沒有的 , 這裡抓 oldImgs 有的但 newImgs 沒有的
        var deletedImgs = oldImgs.Except(newImgs);

        // 抓出來刪除
        foreach (var img in deletedImgs)
        {
            FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsDescriptionImg", img);
        }

        var result = await productsRepository.UpdateProducts(updateTarget);

        return ApiResponseHelper.Success(result);
    }

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> UpdateProductsDeleteStatus(int sellerId, IEnumerable<int> productId)
    {
        var target = await productsRepository.UpdateProductsDeleteStatus(sellerId, productId);
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
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>影響列數</returns>
    public async Task<ApiResponse<int>> DeleteProducts(int productsId, int sellerId)
    {
        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var target = await productsRepository.GetProducts(productsId);
            if (target == null || target.UserId != sellerId)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            IEnumerable<MallProductImg> imgs = new List<MallProductImg>();
            if (target.IsDelete == IsDeleteStatusEnum.Deleted)
            {
                imgs = await productsImgRepository.GetProductsAllImg(productsId);

                var descImgs = ParseDescriptionImgs(target.ProductsDescription);
                foreach (var img in descImgs)
                {
                    FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsDescriptionImg", img);
                }
            }

            var deletetarget = await productsRepository.DeleteProducts(productsId, target.IsDelete, sellerId);

            if (deletetarget == 0)
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
        var product = await productsRepository.GetProducts(productId);
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
    /// 商品描述的圖片上傳
    /// </summary>
    /// <param name="productsDescriptionImgsFiles">商品描述圖片檔案</param>
    /// <returns>上傳是否成功</returns>
    public async Task<ApiResponse<string>> ProductsDescriptionImgUpload(IFormFile productsDescriptionImgsFiles)
    {
        if (productsDescriptionImgsFiles == null)
            return ApiResponseHelper.NotFound<string>();

        var fileName = await FileUploadHelper.SaveFileAsync(
            productsDescriptionImgsFiles,
            env.WebRootPath,
            "ProductsDescriptionImg"
        );

        if (fileName == null)
        {
            return ApiResponseHelper.InternalException<string>("圖片上傳失敗");
        }

        return ApiResponseHelper.Success(fileName);
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
    /// 查看單一訂單評價
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>商品評價資訊</returns>
    public async Task<ApiResponse<RateResponse>> GetOrderRate(int orderId)
    {
        var rate = await productsRateRepositories.GetOrderRate(orderId);
        if (rate == null)
            return ApiResponseHelper.NotFound<RateResponse>();
        return ApiResponseHelper.Success(rate);
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
        if (product == null)
        {
            var errors = new Dictionary<string, string[]> { { "MallProducts", new[] { "商品不存在了!" } } };

            return ApiResponseHelper.RequestError<int>(errors);
        }

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

    /// <summary>
    /// 從 html 解析出所有商品描述圖片的檔名
    /// </summary>
    /// <param name="html">描述內容</param>
    /// <returns>檔案名稱清單</returns>
    private IEnumerable<string> ParseDescriptionImgs(string html)
    {
        // 空的就回傳空清單
        if (string.IsNullOrEmpty(html))
            return Enumerable.Empty<string>();

        // 用正規表達式在 HTML 字串裡找所有符合 /ProductsDescriptionImg/檔名 的地方
        // [^"]+ 的意思是「一直匹配到碰到 " 為止」，用來擷取到 src="" 結尾前的檔名 , [^""] 等於 [^"]
        // 例如：<img src="http://xxx/ProductsDescriptionImg/abc123.jpg"> → 抓到 abc123.jpg
        var matches = Regex.Matches(html, @"/ProductsDescriptionImg/([^""]+)");

        // Group 是正規表達式的內容部分
        // Groups[0] = /ProductsDescriptionImg/abc123.jpg => 整個字串
        // Groups[1] = abc123.jpg => () 裡的部分
        return matches.Select(m => m.Groups[1].Value);
    }
}
