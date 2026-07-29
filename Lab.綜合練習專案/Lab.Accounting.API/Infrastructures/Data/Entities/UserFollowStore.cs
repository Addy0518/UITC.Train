namespace Lab.Accounting.API.Infrastructures.Entities;

public class UserFollowStore
{
    /// <summary>
    /// 使用者優惠券 ID
    /// </summary>
    public int UserFollowStoreId { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 賣場 ID
    /// </summary>
    public int StoreId { get; set; }
}
