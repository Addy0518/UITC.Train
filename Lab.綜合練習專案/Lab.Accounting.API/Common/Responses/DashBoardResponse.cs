namespace Lab.Accounting.API.Responses;

public class DashBoardResponse
{
    /// <summary>
    /// 總銷售額
    /// </summary>
    public double TotalRevenue { get; set; }

    /// <summary>
    /// 本月份銷售額
    /// </summary>
    public double MonthlyRevenue { get; set; }

    /// <summary>
    /// 近七天的個別銷售額
    /// </summary>
    public IEnumerable<WeekSalesResponse> WeekSales { get; set; }

    /// <summary>
    /// 庫存少於五的商品
    /// </summary>
    public IEnumerable<Infrastructures.Data.Entities.Product> LowStockProducts { get; set; }

    /// <summary>
    /// 銷量最好的前五的商品
    /// </summary>
    public IEnumerable<TopSellingResponse> TopSellingProducts { get; set; }

    /// <summary>
    /// 賣家的所有商品評分分布
    /// </summary>
    public IEnumerable<DashBoardRateResponse> RateDistribution { get; set; }
}
