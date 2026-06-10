using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories.Interface;

public interface IDashBoradRepository
{
    /// <summary>
    /// 查看賣家總銷售額
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>總銷售額</returns>
    Task<double> GetTotalRevenue(int sellerUserId);

    /// <summary>
    /// 查看賣家本月份銷售額
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>本月份銷售額</returns>
    Task<double> GetMonthlyRevenue(int sellerUserId);

    /// <summary>
    /// 查看賣家近七天的個別銷售額
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>近七天個別銷售額</returns>
    Task<IEnumerable<WeekSalesResponse>> GetWeekSales(int sellerUserId);

    /// <summary>
    /// 查看賣家庫存少於五的商品
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>商品</returns>
    Task<IEnumerable<Infrastructures.Data.Entities.Product>> GetLowStockProducts(int sellerUserId);

    /// <summary>
    /// 查看賣家銷量最好的前五的商品
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>商品</returns>
    Task<IEnumerable<TopSellingResponse>> GetTopSellingProducts(int sellerUserId);

    /// <summary>
    /// 查看賣家的所有商品評分分布
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>評分分布</returns>
    Task<IEnumerable<DashBoardRateResponse>> GetRateDistribution(int sellerUserId);
}
