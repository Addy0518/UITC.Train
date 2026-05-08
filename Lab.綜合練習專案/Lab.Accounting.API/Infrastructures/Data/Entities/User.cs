namespace Lab.Accounting.API.Infrastructures.Entities;

public class User
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者帳號
    /// </summary>
    public string UserAccount { get; set; }

    /// <summary>
    /// 使用者密碼
    /// </summary>
    public string UserPassword { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 使用者電話
    /// </summary>
    public string? UserPhone { get; set; }

    /// <summary>
    /// 使用者地址
    /// </summary>
    public string? UserAddress { get; set; }

    /// <summary>
    /// 使用者大頭照
    /// </summary>
    public string? UserHeadshot { get; set; }

    /// <summary>
    /// 使用者權限
    /// </summary>
    public string? UserRole { get; set; }

    /// <summary>
    /// 購物車 ID
    /// </summary>
    public int ShoppingCarId { get; set; }
}
