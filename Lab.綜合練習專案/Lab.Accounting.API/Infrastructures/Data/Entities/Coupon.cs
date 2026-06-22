namespace Lab.Accounting.API.Infrastructures.Entities;

public class Coupon
{
    /// <summary>
    /// 優惠券 ID
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// 創建者 ID
    /// </summary>
    public int CreaterId { get; set; }

    /// <summary>
    /// 優惠券序號 (唯一值)
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 優惠券名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 優惠券類型 (1: 百分比折扣, 2: 固定金額折抵...)
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 折扣數值 (打折趴數或折抵金額)
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// 最低消費門檻
    /// </summary>
    public decimal MinimunSpend { get; set; }

    /// <summary>
    /// 發行數量 ( Null 表示無限制 )
    /// </summary>
    public int? TotalQuantity { get; set; }

    /// <summary>
    /// 已領取數量
    /// </summary>
    /// </summary>
    public int ReceiveQuantity { get; set; } = 0;

    /// <summary>
    /// 折扣開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 折扣結束時間
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }
}
