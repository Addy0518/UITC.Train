namespace Lab.Accounting.API.Infrastructures.Data.Entities;

public class ProductRate
{
    /// <summary>
    /// 商品評價 ID
    /// </summary>
    public int? ProductsRateId { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 訂單 ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 商品 ID
    /// </summary>
    public int ProductsId { get; set; }

    /// <summary>
    /// 評分
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// 評論
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 賣家回覆內容
    /// </summary>
    public string? SellerReply { get; set; }

    /// <summary>
    /// 賣家回覆時間
    /// </summary>
    public DateTime? SellerReplyTime { get; set; }

    /// <summary>
    /// 發表時間
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
