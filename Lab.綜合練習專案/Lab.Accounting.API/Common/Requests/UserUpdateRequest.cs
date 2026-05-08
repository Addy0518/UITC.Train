namespace Lab.Accounting.API.Common.Requests;

public class UserUpdateRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Display(Name = "使用者名稱")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [MaxLength(50, ErrorMessage = "{0} 不能超過 {1} 個字!")]
    public string? UserName { get; set; }

    /// <summary>
    /// 使用者電話
    /// </summary>
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "{0}格式不正確，請輸入09開頭的10位數字")]
    public string? UserPhone { get; set; }

    /// <summary>
    /// 使用者地址
    /// </summary>
    public string? UserAddress { get; set; }
}
