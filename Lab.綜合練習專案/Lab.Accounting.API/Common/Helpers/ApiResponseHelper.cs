using NPOI.SS.Formula.Functions;

namespace Lab.Accounting.API.Helpers;

public static class ApiResponseHelper
{
    // 用靜態方法讓整個專案都能引用
    public static ApiResponse<T> Success<T>(T data, string message = "")
    {
        return new ApiResponse<T>
        {
            CodeStatus = CodeStatus.Success,
            ReturnData = data,
            Message = message,
        };
    }

    public static ApiResponse<T> InternalException(ProblemDetails errors)
    {
        return new ApiResponse<T>
        {
            CodeStatus = CodeStatus.InternalException,
            Message = CodeStatus.InternalException.GetDescription(),
            Error500 = errors,
        };
    }

    public static ApiResponse<T> RequestError<T>(Dictionary<string, string[]> errors)
    {
        return new ApiResponse<T>
        {
            CodeStatus = CodeStatus.RequestError,
            ReturnData = default,
            Message = CodeStatus.RequestError.GetDescription(),
            Error400 = errors,
        };
    }

    public static ApiResponse<T> NotFound<T>()
    {
        return new ApiResponse<T>
        {
            CodeStatus = CodeStatus.NotFound,
            ReturnData = default,
            Message = CodeStatus.NotFound.GetDescription(),
        };
    }
}
