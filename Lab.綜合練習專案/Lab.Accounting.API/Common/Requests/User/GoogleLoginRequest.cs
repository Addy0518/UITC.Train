namespace Lab.Accounting.API.Common.Requests;

public class GoogleLoginRequest
{
    /// <summary>
    /// Google 回傳的 id_token
    /// </summary>
    [Required(ErrorMessage = "Google Token 不能為空!")]
    public string IdToken { get; set; } = string.Empty;
}
