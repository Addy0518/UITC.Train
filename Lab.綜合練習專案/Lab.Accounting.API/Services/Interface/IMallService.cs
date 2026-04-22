using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Services.Interface
{
    public interface IMallService
    {
        /// <summary>
        /// 查看商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <returns>商品資訊</returns>
        Task<ApiResponse<ProductsResponse>> GetProducts(int productId);

        /// <summary>
        /// 查看所有商品 ( 分頁 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>商品列表</returns>
        Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(
            int pageIndex,
            int pageSize,
            int? userId = null
        );

        /// <summary>
        /// 新增單一商品 + 類別
        /// </summary>
        /// <param name="productsInsertRequest">商品資訊</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> CreateProducts(ProductsInsertRequest productsInsertRequest);

        /// <summary>
        /// 軟刪除單一商品
        /// </summary>
        /// <param name="productsId">商品 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> DeleteProducts(int productsId, int userId);

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
        //Task<ApiResponse<int>> ProductsImgDelete(int productsImgId);

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

        /// <summary>
        /// 新增單一商品到購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> AddProductsInShoppingCar(int productsId, int userId);

        /// <summary>
        /// 刪除單一商品從購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> DeleteProductsInShoppingCar(int productsId, int userId);
    }
}
