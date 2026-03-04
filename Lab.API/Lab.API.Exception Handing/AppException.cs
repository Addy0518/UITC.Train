using System.Net;
using System.Web.Http.ExceptionHandling;

namespace Lab.API.Exception_Handing
{
    // abstract 用抽象類別讓別人不能直接使用 AppException 通用錯誤訊息 , 只能自己在開發比如 NotFoundException
    public abstract class AppException : Exception
    {
        // 用 HttpStatusCode 決定要回傳什麼狀態碼
        public HttpStatusCode StatusCode { get; }

        // 用建構函式接一個訊息 Message 跟狀態碼
        protected AppException(
            string Message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError
        )
            : base(Message)
        {
            StatusCode = statusCode;
        }
    }

    // 用 sealed 讓函式不被繼承
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, object key)
            : base(
                $"{resourceName} with identifier '{key}' was not found.",
                HttpStatusCode.NotFound
            ) { }
    }

    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest) { }
    }

    public sealed class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict) { }
    }

    public sealed class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]> { { field, [error] } };
        }
    }
}
