namespace Lab.Accounting.API.Responses;

public class TopSellingResponse
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    public int ProductsId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductsName { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public decimal? ProductsPrice { get; set; }

    /// <summary>
    /// 商品庫存數量
    /// </summary>
    public int? ProductsStock { get; set; }

    /// <summary>
    /// 商品總銷量
    /// </summary>
    public int? TotalSales { get; set; }
}
