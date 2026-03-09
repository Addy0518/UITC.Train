namespace Lab.API.Dapper.Helper
{
    public class APIResponseHelper
    {
        public static APIResponses<T> Success<T>(T data, string message = "")
        {
            return new APIResponses<T>
            {
                CodeStatus = CodeStatus.Success,
                ReturnData = data,
                Message = message,
            };
        }
    }
}
