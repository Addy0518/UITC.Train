namespace Lab.Accounting.API.Infrastructures.Entities;

public class TokenBlackList
{
    /// <summary>
    /// TokenID
    /// </summary>
    public int TokenId { get; set; }

    /// <summary>
    /// Jti 識別碼
    /// </summary>
    public string Jti { get; set; }

    /// <summary>
    /// 過期時間
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// 登出時間
    /// </summary>
    public DateTime? LogoutDate { get; set; }
}
