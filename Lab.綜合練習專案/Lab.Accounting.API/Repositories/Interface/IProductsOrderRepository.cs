using Lab.Accounting.API.Common.Requests.Order;

namespace Lab.Accounting.API.Repositories.Interface;

public interface IProductsOrderRepository
{
    /// <summary>
    /// 買家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="userId">買家 ID</param>
    /// <returns>訂單資訊</returns>
    Task<OrderResponse> GetUserOneOrder(int orderId, int userId);

    /// <summary>
    /// 賣家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>訂單資訊</returns>
    Task<OrderResponse> GetSellerOneOrder(int orderId, int sellerId);

    /// <summary>
    /// 查看買家所有訂單 ( 訂單編號查詢 )
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>多筆訂單資訊</returns>
    Task<IEnumerable<Order>> GetOrderByOrderNumber(string orderNumber);

    /// <summary>
    /// 買家查看所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>所有訂單資訊</returns>
    Task<IEnumerable<OrderResponse>> GetUserOrder(int userId);

    /// <summary>
    /// 賣家查看所有訂單
    /// </summary>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>所有訂單資訊</returns>
    Task<IEnumerable<OrderResponse>> GetSellerOrder(int sellerId);

    /// <summary>
    /// 查看所有訂單
    /// </summary>
    /// <param name="request">訂單搜尋請求</param>
    /// <returns>所有訂單資訊</returns>
    Task<IEnumerable<OrderResponse>> GetAllOrder(OrderSearchRequest request);

    /// <summary>
    /// 改變運輸狀態
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>影響行數</returns>
    Task<int> UpdateShippingStatus(int orderId, ShippingStatusEnum shippingStatus);

    /// <summary>
    /// 商品購買
    /// </summary>
    /// <param name="order">購買資訊</param>
    /// <returns>訂單 ID</returns>
    Task<int> BuyProducts(Order order);

    /// <summary>
    /// 商品付款
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <param name="shippingStatus">運送狀態</param>
    /// <param name="paidType">付款方式</param>
    /// <param name="paidTime">付款時間</param>
    /// <returns>影響列數</returns>
    Task<int> PaidProducts(string orderNumber, int shippingStatus, string paidType, DateTime paidTime);

    /// <summary>
    /// 商品重新付款
    /// </summary>
    /// <param name="orderIds">所有訂單 ID</param>
    /// <param name="newOrderNumber">新訂單編號</param>
    /// <returns>訂單 ID</returns>
    Task<int> RetryPaidProducts(List<int> orderIds, string newOrderNumber);
}
