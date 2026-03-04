using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab.API.Filter
{
    public class MyLogging : Attribute, IActionFilter
    {
        private readonly string _colorname;

        public MyLogging(string colorname)
        {
            _colorname = colorname;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Filter Excuted before");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("Filter Excuted after");
        }
    }
}
