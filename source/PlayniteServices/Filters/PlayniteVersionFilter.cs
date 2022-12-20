using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Filters
{
    public class PlayniteVersionFilter : ActionFilterAttribute
    {
        private readonly UpdatableAppSettings appSettings;

        public PlayniteVersionFilter(UpdatableAppSettings settings)
        {
            appSettings = settings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (appSettings.Settings.RestrictPlayniteVersion && appSettings.Settings.RestrictedPlayniteVersions.HasItems())
            {
                var allowRequest = false;
                if (context.HttpContext.Request.Headers.TryGetValue("Playnite-Version", out var headerVer) && !headerVer.ToString().IsNullOrWhiteSpace())
                {
                    if (appSettings.Settings.RestrictedPlayniteVersions.Contains(headerVer!))
                    {
                        allowRequest = true;
                    }
                }

                if (!allowRequest)
                {
                    context.Result = new JsonResult(new ErrorResponse("Bad request."));
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
