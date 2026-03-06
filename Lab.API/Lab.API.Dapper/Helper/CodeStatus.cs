using System.ComponentModel;

namespace Lab.API.Dapper.Helper
{
    // 自訂狀態碼
    public enum CodeStatus
    {
        [Description("成功")]
        Success = 2000,

        [Description("Request驗證失敗")]
        RequestError = 4000,

        [Description("查無此資料")]
        NotFound = 4001,

        [Description("內部伺服器錯誤")]
        InternalException = 5000,
    }
}
