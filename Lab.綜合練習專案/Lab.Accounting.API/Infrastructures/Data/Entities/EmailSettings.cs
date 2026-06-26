namespace Lab.Accounting.API.Infrastructures.Entities;

public class EmailSettings
{
    /// <summary>
    /// 寄送人名稱
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 寄送人電子郵件
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// SendGrid API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
