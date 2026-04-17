using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Services.Interface
{
    public interface IMallService
    {
        /// <summary>
        /// 查看單一商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>商品資訊</returns>
        Task<ApiResponse<ProductsResponse>> GetProducts(int productId, int userId);

        /// <summary>
        /// 查看所有商品 ( 分頁 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <returns>商品列表</returns>
        Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(int pageIndex, int pageSize);

        /// <summary>
        /// 新增單一商品 + 類別
        /// </summary>
        /// <param name="productsInsertRequest">商品資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> CreateProducts(ProductsInsertRequest productsInsertRequest);

        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productsImgsFiles">商品圖片檔案</param>
        /// <param name="productId">商品 Id</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<IEnumerable<MallProductImg>>> ProductsImgUpload(IFormFile productsImgsFiles, int productId);

        /// <summary>
        /// 商品圖片刪除
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> ProductsImgDelete(int productsImgId);

        /// <summary>
        /// 使用者購買商品並評分
        /// </summary>
        /// <param name="Request">商品購買資訊 </param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UserBuyProductAndRate(ProductsBuyRequest Request);

        /// <summary>
        /// 查看購物車中的所有商品
        /// </summary>
        /// <param name="userId">使用者 Id</param>
        /// <returns>購物車中的所有商品</returns>
        Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProductsInShoppingCar(int userId);
    }
}
