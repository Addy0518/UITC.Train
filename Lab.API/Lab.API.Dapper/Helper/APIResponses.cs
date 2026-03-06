namespace Lab.API.Dapper.Helper
{
    public class APIResponses<T>
    {
        // 預設是成功的狀態
        public CodeStatus CodeStatus { get; set; } = CodeStatus.Success;
        public string Message { get; set; } = string.Empty;

        // 泛型讓他放啥類型回傳都可以
        public T ReturnData { get; set; } = default!;
    }
}
