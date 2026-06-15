namespace Lab.Accounting.API.Infrastructures.Entities;

public class UserCoupon
{
    /// <summary>
    /// 使用者優惠券 ID
    /// </summary>
    public int UserCouponId { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 優惠券 ID
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// 領取時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 使用時間 (NULL 代表未使用)
    /// </summary>
    public DateTime? UsedTime { get; set; }
}
