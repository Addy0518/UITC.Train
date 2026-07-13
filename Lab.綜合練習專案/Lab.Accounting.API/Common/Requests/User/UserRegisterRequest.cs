namespace Lab.Accounting.API.Common.Requests;

public class UserRegisterRequest
{
    /// <summary>
    /// 使用者帳號
    /// </summary>
    [Display(Name = "使用者帳號")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [EmailAddress(ErrorMessage = "{0} 格式不對!")]
    public string UserAccount { get; set; }

    /// <summary>
    /// 使用者密碼
    /// </summary>
    [Display(Name = "使用者密碼")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [RegularExpression(
        @"^[A-Z](?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{7}$",
        ErrorMessage = "密碼總共 8 個字 , 只能輸入英文跟數字 , 第一個字要大寫"
    )]
    public string UserPassword { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Display(Name = "使用者名稱")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [MaxLength(50, ErrorMessage = "{0} 不能超過 {1} 個字!")]
    public string UserName { get; set; }

    /// <summary>
    /// 使用者電話
    /// </summary>
    [Display(Name = "使用者電話")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "請符合手機號碼格式 0912345678")]
    public string? UserPhone { get; set; }

    /// <summary>
    /// 使用者電話
    /// </summary>
    [Display(Name = "使用者地址")]
    [MaxLength(200, ErrorMessage = "{0} 不能超過 {1} 個字!")]
    public string? UserAddress { get; set; }

    /// <summary>
    /// 使用者郵遞區號
    /// </summary>
    [Display(Name = "使用者郵遞區號")]
    [MaxLength(100, ErrorMessage = "{0} 不能超過 {1} 個字!")]
    public string? UserZipCode { get; set; }
}
