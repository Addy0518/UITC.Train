using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.TODO.Infrastructures.ExceptionHandler;

public class InternalServerExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var title = exception.Message;
        var details = exception.ToString();

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Status = StatusCodes.Status500InternalServerError,
            Title = title,
            Detail = details,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
        };

        problemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);
        problemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id);

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponseHelper.InternalException(problemDetails),
            cancellationToken
        );

        return true;
    }
}
