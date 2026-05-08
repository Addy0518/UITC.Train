namespace Lab.Accounting.API.Services;

public interface IOrderService
{
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
}
