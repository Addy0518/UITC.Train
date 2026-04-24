using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsBuyRepositories(DBConnecting connecting) : IProductsBuyRepositories
    {
        /// <summary>
        /// 查看單一訂單
        /// </summary>
        /// <param name="orderId">購買資訊</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>訂單資訊</returns>
        public async Task<MallOrder> GetOrder(int orderId, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var addBoughtProductsql =
                @"Select * From MallOrder
                  Where OrderId = @OrderId
                  And UserId = @UserId";

            return await conn.QueryFirstOrDefaultAsync<MallOrder>(
                addBoughtProductsql,
                new { OrderId = orderId, UserId = userId }
            );
        }

        /// <summary>
        /// 商品購買
        /// </summary>
        /// <param name="order">購買資訊</param>
        /// <returns>訂單 ID</returns>
        public async Task<int> BuyProducts(MallOrder order)
        {
            using var conn = connecting.CreateConnecting();

            var addBoughtProductsql =
                @"INSERT INTO MallOrder
                            (OrderNumber,
                             UserId,
                             ProductsId,
                             BoughtQuantity,
                             UnitPrice,
                             BoughtTime,
                             ShippingAddress,
                             ShippingStatus)

                VALUES      (@OrderNumber,
                             @UserId,
                             @ProductsId,
                             @BoughtQuantity,
                             @UnitPrice,
                             @BoughtTime,
                             @ShippingAddress,
                             @ShippingStatus)
                 Select 
                            Cast(
                            Scope_Identity() as int
                            );";

            return await conn.ExecuteAsync(addBoughtProductsql, order);
        }

        /// <summary>
        /// 商品付款
        /// </summary>
        /// <param name="shippingStatus">運送狀態</param>
        /// <param name="accountPrice">最終金額</param>
        /// <param name="paidTime">付款時間</param>
        /// <returns>影響列數</returns>
        public async Task<int> PaidProducts(int shippingStatus, decimal accountPrice, DateTime paidTime)
        {
            using var conn = connecting.CreateConnecting();

            var addBoughtProductsql =
                @"Update MallOrder
                  Set ShippingStatus = COALESCE(@ShippingStatus, ShippingStatus),
                      AccountPrice = COALESCE(@AccountPrice, AccountPrice),
                      PaidTime = COALESCE(@PaidTime, PaidTime)
                  Where OrderId = @OrderId";

            return await conn.ExecuteAsync(
                addBoughtProductsql,
                new
                {
                    ShippingStatus = shippingStatus,
                    AccountPrice = accountPrice,
                    PaidTime = paidTime,
                }
            );
        }
    }
}
