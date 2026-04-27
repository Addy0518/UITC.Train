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
        /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <param name="userId">使用者 Id</param>
        /// <param name="isDelete">是否為刪除狀態</param>
        /// <returns>商品列表</returns>
        Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(
            int pageIndex,
            int pageSize,
            int? userId = null,
            bool? isDelete = false
        );

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
        /// 軟刪除單一商品
        /// </summary>
        /// <param name="productsId">商品 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> DeleteProducts(int productsId, int userId);

        /// <summary>
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategoryId">商品類別 ID</param>
        /// <returns>商品類別</returns>
        Task<ApiResponse<IEnumerable<MallProductCategory>>> GetCategory(int? productcategoryId = null);

        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productsImgsFiles">商品圖片檔案</param>
        /// <param name="productId">商品 Id</param>
        /// <returns>新增成功的商品圖片</returns>
        Task<ApiResponse<IEnumerable<MallProductImg>>> ProductsImgUpload(IFormFile productsImgsFiles, int productId);

        /// <summary>
        /// 刪除商品圖片
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>刪除的圖片</returns>
        Task<ApiResponse<MallProductImg>> DeleteProductsImg(int productsImgId);

        /// <summary>
        /// 使用者購買商品並跳轉綠界界面
        /// </summary>
        /// <param name="Request">商品購買資訊 </param>
        /// <returns>訂單 ID</returns>
        Task<ApiResponse<int>> UserBuyProduct(ProductsBuyRequest Request);

        /// <summary>
        /// 綠界訂單創建(新增)
        /// </summary>
        /// <param name="orderId">商品購買資訊 </param>
        /// <param name="tunnelUrl">開發者通道網址</param>
        /// <returns>跳轉綠界訂單</returns>
        Task<ApiResponse<GreenPayResponse>> GetPaymentData(int orderId, int userId, string tunnelUrl);

        /// <summary>
        /// 接收綠界回傳資料(驗證)
        /// </summary>
        /// <param name="collection">綠界回傳的表單資料</param>
        /// <returns>回傳成功或失敗代號</returns>
        Task<string> SetPaymentData(IFormCollection collection);

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
