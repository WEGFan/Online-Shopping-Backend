using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using HttpStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using StatusCodes = OnlineShoppingBackend.Utils.StatusCodes;

namespace OnlineShoppingBackend.Utils
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private ILogger<GlobalExceptionFilter> logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
            logger.LogError($"{ex.Message}\r\n{ex.StackTrace}");
            context.Result = new JsonResult(Return.Error(ex.Message, HttpStatusCodes.Status500InternalServerError))
            {
                StatusCode = HttpStatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
