namespace Lab.Accounting.API.Infrastructures.Entities;

public class EcpayLogisticsSettings
{
    /// <summary>
    /// 綠界物流特店編號
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// 物流 HashKey
    /// </summary>
    public string HashKey { get; set; } = string.Empty;

    /// <summary>
    /// 物流 HashIV
    /// </summary>
    public string HashIV { get; set; } = string.Empty;

    /// <summary>
    /// API 基底網址（開發時填 ngrok 那串，正式上線換成真實網域）
    /// </summary>
    public string ServerBaseUrl { get; set; } = string.Empty;
}
