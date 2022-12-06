using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters
{
    /// <summary>
    /// Exception filter to log errors.
    /// </summary>
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        /// <summary>
        /// Initialize new instance of AppExceptionFilterAttribute
        /// </summary>
        /// <param name="hostEnvironment">Provides information about web hosting</param>
        public AppExceptionFilterAttribute(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Exception handler
        /// </summary>
        /// <param name="context">Exception context <see cref="ExceptionContext"/></param>
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
