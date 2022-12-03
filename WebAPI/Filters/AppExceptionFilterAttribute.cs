using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebAPI.Filters
{
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public AppExceptionFilterAttribute(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            string stackTrace = (_hostEnvironment.IsDevelopment() ? context?.Exception?.StackTrace : String.Empty) ?? String.Empty;
            string message = ex.Message;
            string error = "General error";

            Serilog.Log.Error(ex, error, stackTrace);

            IActionResult actionResult = new ObjectResult(
                new
                {
                    Error = error,
                    Message = message,
                    stackTrace = stackTrace
                })
            {
                StatusCode = 500
            };

            if(context?.Result != null)
            {
                context.Result = actionResult;
            }  
        }
    }
}
