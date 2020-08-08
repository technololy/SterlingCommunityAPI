using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingCommunityAPI.FilterAttribute
{
    public class DecryptActionFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Request.Body;

            base.OnActionExecuting(context);
        }
    }
}
