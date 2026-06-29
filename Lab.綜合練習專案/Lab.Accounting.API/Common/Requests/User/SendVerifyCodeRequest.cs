namespace Lab.Accounting.API.Common.Requests;

public class SendVerifyCodeRequest
{
    /// <summary>
    /// 使用者帳號
    /// </summary>
    [Display(Name = "使用者帳號")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    public string UserAccount { get; set; }
}
