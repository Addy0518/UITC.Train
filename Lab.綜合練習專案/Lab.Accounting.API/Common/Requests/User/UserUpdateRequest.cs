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
    [MaxLength(50, ErrorMessage = "{0} 不能超過 {200} 個字!")]
    public string? UserAddress { get; set; }

    /// <summary>
    /// 收件人郵遞區號 ( 宅配 )
    /// </summary>
    [Display(Name = "收件人郵遞區號")]
    [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
    public string? UserZipCode { get; set; }

    /// <summary>
    /// 使用者生日
    /// </summary>
    public DateOnly? UserBirthDate { get; set; }

    /// <summary>
    /// 使用者性別
    /// </summary>
    public GenderEnum UserGender { get; set; }
}
