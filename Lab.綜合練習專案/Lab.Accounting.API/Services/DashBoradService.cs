namespace Lab.Accounting.API.Services;

public class DashboradService(IDashBoradRepository dashBoradRepository) : IDashBoradService
{
    /// <summary>
    /// 查看賣家所有數據
    /// </summary>
    /// <param name="sellerUserId">賣家 ID</param>
    /// <returns>賣家數據</returns>
    public async Task<ApiResponse<DashBoardResponse>> GetDashboard(int sellerUserId)
    {
        var totalRevenueTask = dashBoradRepository.GetTotalRevenue(sellerUserId);
        var monthlyRevenueTask = dashBoradRepository.GetMonthlyRevenue(sellerUserId);
        var weekSalesTask = dashBoradRepository.GetWeekSales(sellerUserId);
        var lowStockTask = dashBoradRepository.GetLowStockProducts(sellerUserId);
        var topSellingTask = dashBoradRepository.GetTopSellingProducts(sellerUserId);
        var rateDistributionTask = dashBoradRepository.GetRateDistribution(sellerUserId);

        // 同時發出所有查詢
        await Task.WhenAll(totalRevenueTask, monthlyRevenueTask, weekSalesTask, lowStockTask, topSellingTask);

        var result = new DashBoardResponse
        {
            TotalRevenue = totalRevenueTask.Result,
            MonthlyRevenue = monthlyRevenueTask.Result,
            WeekSales = weekSalesTask.Result,
            LowStockProducts = lowStockTask.Result,
            TopSellingProducts = topSellingTask.Result,
            RateDistribution = rateDistributionTask.Result,
        };

        return ApiResponseHelper.Success(result);
    }
}
