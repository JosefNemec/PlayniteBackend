using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Playnite;

namespace Playnite.Backend;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
    private static readonly ILogger logger = LogManager.GetLogger();

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.HttpContext.Request.Path == "/playnite/users")
        {
            base.OnException(context);
            return;
        }

        logger.Error(context.Exception, $"Request failed: {context.HttpContext.Request.Method}, {context.HttpContext.Request.Path}");
        if (context.HttpContext.Request.Method == "POST")
        {
            if (context.HttpContext.Request.ContentLength > 0)
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(context.HttpContext.Request.Body))
                {
                    logger.Error(await reader.ReadToEndAsync());
                }
            }
            else
            {
                logger.Error("POST request with no content.");
            }
        }

        context.Result = new JsonResult(new ErrorResponse(context.Exception));
        base.OnException(context);
    }
}
