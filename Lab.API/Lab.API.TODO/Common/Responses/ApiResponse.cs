namespace Lab.API.TODO.Common.Responses;

// 統一回傳訊息
public class ApiResponse<T>
{
    public CodeStatus CodeStatus { get; set; } = CodeStatus.Success;
    public string Message { get; set; } = string.Empty;
    public T ReturnData { get; set; } = default!;
}
