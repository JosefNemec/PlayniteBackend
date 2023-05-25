using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlayniteServices;

public class ServiceKeyFilter : ActionFilterAttribute
{
    private readonly UpdatableAppSettings appSettings;

    public ServiceKeyFilter(UpdatableAppSettings settings)
    {
        appSettings = settings;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var allowRequest = false;
        if (context.HttpContext.Request.Headers.TryGetValue("Service-Key", out var headerVer) &&
            appSettings.Settings.ServiceKey == headerVer)
        {
            allowRequest = true;
        }

        if (!allowRequest)
        {
            context.Result = new JsonResult(new ErrorResponse("Bad request."));
        }

        base.OnActionExecuting(context);
    }
}
