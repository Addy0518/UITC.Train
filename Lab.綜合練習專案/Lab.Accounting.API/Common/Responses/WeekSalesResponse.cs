namespace Lab.Accounting.API.Responses;

public class WeekSalesResponse
{
    /// <summary>
    /// 指定的日期的銷售額
    /// </summary>
    public double DailyRevenue { get; set; }

    /// <summary>
    /// 指定的日期
    /// </summary>
    public DateTime? OrderDate { get; set; }
}
