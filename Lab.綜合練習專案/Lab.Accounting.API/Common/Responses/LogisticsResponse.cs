namespace Lab.Accounting.API.Common.Responses;

public class LogisticsResponse
{
    /// <summary>
    /// 物流單 ID
    /// </summary>
    public int LogisticsId { get; set; }

    /// <summary>
    /// 訂單 ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 物流方式 (CVS: 超商 / Home: 宅配)
    /// </summary>
    public string LogisticsType { get; set; } = string.Empty;

    /// <summary>
    /// 物流子類型 (UNIMART: 7-11 / FAMIC2C: 全家 / HILIFEC2C: 萊爾富 / TCAT: 黑貓宅急便)
    /// </summary>
    public string LogisticsSubType { get; set; } = string.Empty;

    /// <summary>
    /// 超商門市代號（宅配為 null）
    /// </summary>
    public string? StoreCode { get; set; }

    /// <summary>
    /// 超商門市名稱（宅配為 null）
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// 超商門市地址（宅配為 null）
    /// </summary>
    public string? StoreAddress { get; set; }

    /// <summary>
    /// 收件人姓名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收件人電話
    /// </summary>
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收件人地址（超商取貨為 null）
    /// </summary>
    public string? ReceiverAddress { get; set; }

    /// <summary>
    /// 綠界物流追蹤編號（建立物流訂單成功後才有值）
    /// </summary>
    public string? LogisticsTrackingNo { get; set; }

    /// <summary>
    /// 物流狀態英文代碼（Created / Shipped / InTransit / Delivered / PickedUp / Cancelled）
    /// </summary>
    public LogisticsStatusEnum LogisticsStatus { get; set; }

    /// <summary>
    /// 物流資料建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 賣家出貨時間（確認出貨後才有值）
    /// </summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 送達門市或配達時間（綠界通知後才有值）
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// 買家完成取件時間（超商取貨且取件後才有值）
    /// </summary>
    public DateTime? PickedUpAt { get; set; }
}
