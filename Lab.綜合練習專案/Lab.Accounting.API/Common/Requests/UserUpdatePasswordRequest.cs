namespace Lab.Accounting.API.Common.Requests;

public class UserUpdatePasswordRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    [Display(Name = "使用者 ID")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者舊密碼
    /// </summary>
    [Display(Name = "使用者舊密碼")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [RegularExpression(
        @"^[A-Z](?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{7}$",
        ErrorMessage = "密碼總共 8 個字 , 只能輸入英文跟數字 , 第一個字要大寫"
    )]
    public string OldUserPassword { get; set; }

    /// <summary>
    /// 使用者新密碼
    /// </summary>
    [Display(Name = "使用者新密碼")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [RegularExpression(
        @"^[A-Z](?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{7}$",
        ErrorMessage = "密碼總共 8 個字 , 只能輸入英文跟數字 , 第一個字要大寫"
    )]
    public string NewUserPassword { get; set; }
}
