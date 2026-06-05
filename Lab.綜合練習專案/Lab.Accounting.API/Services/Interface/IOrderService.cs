using Lab.Accounting.API.Common.Requests.Order;
using Lab.Accounting.API.Common.Requests.Products;

namespace Lab.Accounting.API.Services;

public interface IOrderService
{
    /// <summary>
    /// 買家查看所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>訂單 ID</returns>
    Task<ApiResponse<IEnumerable<OrderResponse>>> GetUserOrder(int userId);

    /// <summary>
    /// 買家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>訂單資訊</returns>
    Task<ApiResponse<OrderResponse>> GetUserOneOrder(int orderId, int userId);

    /// <summary>
    /// 賣家查看所有訂單
    /// </summary>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>所有訂單資訊</returns>
    Task<ApiResponse<IEnumerable<OrderResponse>>> GetSellerOrder(int sellerId);

    /// <summary>
    /// 賣家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>訂單資訊</returns>
    Task<ApiResponse<OrderResponse>> GetSellerOneOrder(int orderId, int sellerId);

    /// <summary>
    /// 查看所有訂單
    /// </summary>
    /// <param name="request">訂單搜尋請求</param>
    /// <returns>所有訂單資訊</returns>
    Task<ApiResponse<IEnumerable<OrderResponse>>> GetAllOrder(OrderSearchRequest request);

    /// <summary>
    /// 改變運輸狀態
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>影響行數</returns>
    Task<ApiResponse<int>> UpdateShippingStatus(int orderId, ShippingStatusEnum shippingStatus);

    /// <summary>
    /// 使用者購買商品並跳轉綠界界面
    /// </summary>
    /// <param name="Request">商品購買資訊 </param>
    /// <returns>多筆訂單 ID</returns>
    Task<ApiResponse<List<int>>> UserBuyProduct(ProductsBuyRequest Request);

    /// <summary>
    /// 綠界訂單創建(新增)
    /// </summary>
    /// <param name="orderId">多筆訂單 ID </param>
    /// <param name="userId">使用者 ID </param>
    /// <param name="tunnelUrl">開發者通道網址</param>
    /// <returns>跳轉綠界訂單</returns>
    Task<ApiResponse<GreenPayResponse>> GetPaymentData(List<int> orderId, int userId, string tunnelUrl);

    /// <summary>
    /// 接收綠界回傳資料(驗證)
    /// </summary>
    /// <param name="collection">綠界回傳的表單資料</param>
    /// <returns>回傳成功或失敗代號</returns>
    Task<string> SetPaymentData(IFormCollection collection);

    /// <summary>
    /// 綠界訂單創建( 重新付款 )
    /// </summary>
    /// <param name="orderIds">多筆訂單 ID </param>
    /// <param name="userId">使用者 ID </param>
    /// <param name="tunnelUrl">開發者通道網址</param>
    /// <returns>跳轉綠界訂單</returns>
    Task<ApiResponse<GreenPayResponse>> GetRetryPaymentData(List<int> orderIds, int userId, string tunnelUrl);
}
