using Lab.Accounting.API.Common.Requests.Products;

namespace Lab.Accounting.API.Services;

public interface IProductsService
{
    /// <summary>
    /// 賣家查看商品
    /// </summary>
    /// <param name="productId">商品 Id</param>
    /// <returns>商品資訊</returns>
    Task<ApiResponse<ProductDetails>> GetProducts(int productId);

    /// <summary>
    /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    Task<ApiResponse<ProductsResponse>> GetAllProducts(ProductsSearchRequest request);

    /// <summary>
    /// 賣家查看自己的所有商品
    /// </summary>
    /// <param name="request">搜尋條件</param>
    /// <returns>商品列表</returns>
    Task<ApiResponse<ProductsResponse>> SellerGetAllProducts(ProductsSearchRequest request);

    /// <summary>
    /// 新增單一商品 + 類別
    /// </summary>
    /// <param name="productsInsertRequest">商品資訊</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> CreateProducts(ProductsInsertRequest productsInsertRequest);

    /// <summary>
    /// 更新單一商品
    /// </summary>
    /// <param name="productsUpdateRequest">商品更新資訊</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> UpdateProducts(ProductsUpdateRequest productsUpdateRequest);

    /// <summary>
    /// 復原已選取的商品刪除狀態
    /// </summary>
    /// <param name="productId">選取的所有商品 Id</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> UpdateProductsDeleteStatus(int userId, IEnumerable<int> productId);

    /// <summary>
    /// 軟刪除或硬刪除單一商品
    /// </summary>
    /// <param name="productsId">商品 ID</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> DeleteProducts(int productsId, int userId);

    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgsFiles">商品圖片檔案</param>
    /// <param name="reviewId">審核表 ID</param>
    /// <returns>新增成功的圖片</returns>
    Task<ApiResponse<IEnumerable<ProductImg>>> ProductsImgUpload(IFormFile productsImgsFiles, int reviewId);

    /// <summary>
    /// 商品描述的圖片上傳
    /// </summary>
    /// <param name="productsDescriptionImgsFiles">商品描述圖片檔案</param>
    /// <returns>上傳是否成功</returns>
    Task<ApiResponse<string>> ProductsDescriptionImgUpload(IFormFile productsDescriptionImgsFiles);

    /// <summary>
    /// 刪除商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>刪除的圖片</returns>
    Task<ApiResponse<ProductImg>> DeleteProductsImg(int productsImgId);

    /// <summary>
    /// 查看單一訂單評價
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>商品評價資訊</returns>
    Task<ApiResponse<RateResponse>> GetOrderRate(int orderId);

    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="request">商品評價資訊</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> CreateProductRate(ProductsRateRequest request);
}
