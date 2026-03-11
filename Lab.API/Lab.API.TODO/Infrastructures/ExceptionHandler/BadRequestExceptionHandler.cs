using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lab.API.TODO.Infrastructures.ExceptionHandler
{
    public static class BadRequestExceptionHandler
    {
        // 寫一個 middleware 捕捉驗證錯誤
        //public static BadRequestObjectResult TryHadler(ActionContext context)
        //{
        //    var errors = context.ModelState.Where(x =>
        //        x.Value.ValidationState == ModelValidationState.Invalid
        //    )
        //    .ToDictionary()
        //}
    }
}
