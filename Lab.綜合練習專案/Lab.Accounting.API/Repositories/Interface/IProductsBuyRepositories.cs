using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsBuyRepositories
    {
        /// <summary>
        /// 查看單一訂單
        /// </summary>
        /// <param name="orderId">購買資訊</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>訂單 ID</returns>
        Task<MallOrder> GetOrder(int orderId, int userId);

        /// <summary>
        /// 商品購買
        /// </summary>
        /// <param name="order">購買資訊</param>
        /// <returns>訂單 ID</returns>
        Task<int> BuyProducts(MallOrder order);

        /// <summary>
        /// 商品付款
        /// </summary>
        /// <param name="shippingStatus">運送狀態</param>
        /// <param name="accountPrice">最終金額</param>
        /// <param name="paidTime">付款時間</param>
        /// <returns>影響列數</returns>
        Task<int> PaidProducts(int shippingStatus, decimal accountPrice, DateTime paidTime);
    }
}
