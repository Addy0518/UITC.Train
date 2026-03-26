using Lab.Accounting.API.Extensions;
using Lab.Accounting.API.Responses;
using Microsoft.AspNetCore.Mvc;

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

    public static ApiResponse<ProblemDetails> InternalException(ProblemDetails errors)
    {
        return new ApiResponse<ProblemDetails>
        {
            CodeStatus = CodeStatus.InternalException,
            ReturnData = errors,
            Message = CodeStatus.InternalException.GetDescription(),
        };
    }

    public static ApiResponse<Dictionary<string, string[]>> RequestError(
        Dictionary<string, string[]> errors
    )
    {
        return new ApiResponse<Dictionary<string, string[]>>
        {
            CodeStatus = CodeStatus.RequestError,
            ReturnData = errors,
            Message = CodeStatus.RequestError.GetDescription(),
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
